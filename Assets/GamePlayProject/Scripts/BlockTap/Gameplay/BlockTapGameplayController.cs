using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BlockTap.Config;
using BlockTap.Importing;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BlockTap.Gameplay
{
    public class BlockTapGameplayController : MonoBehaviour
    {
        [Header("Scene Views")]
        [SerializeField] private BlockTapHudView hudView;
        [SerializeField] private BlockTapBoardView boardView;

        private BlockTapLevelDatabaseSO levelDatabase;
        private BlockTapGameplayConfigSO gameplayConfig;
        private BlockTapVisualConfigSO visualConfig;
        private BlockTapAudioConfigSO audioConfig;
        private BlockTapPrefabConfigSO prefabConfig;

        private BoardGenerator boardGenerator;
        private BoardSelectionResolver selectionResolver;
        private BoardCollapseResolver collapseResolver;
        private LevelGoalTracker goalTracker;
        private LevelProgressionController progressionController;
        private PropController propController;
        private BoardAnimationPlayer animationPlayer;
        private AudioCueController audioCueController;

        private readonly Queue<BlockTapBlockView> pooledBlocks = new Queue<BlockTapBlockView>();
        private const float CollectFlightStagger = 0.02f;
        private const float BombShockwaveDelayPerCell = 0.04f;
        private const float BombShockwaveStrength = 0.46f;
        private const float BombShockwaveMinStrength = 0.2f;
        private const float BombShockwavePushDuration = 0.2f;
        private const float BombShockwaveReturnDuration = 0.3f;
        private BlockTapBlockView[,] grid;
        private List<int> targetColors;
        private List<int> dropColorPool;
        private int effectiveEliminationCount;
        private bool inputLocked;
        private bool levelTransitioning;
        private Coroutine reloadBoardRoutine;
        private Coroutine initialBoardReadyRoutine;
        private Coroutine levelCompleteRoutine;
        private RectTransform activeBombIcon;
        public static BlockTapGameplayController Instance { get; private set; }

        public void Initialize(
            BlockTapLevelDatabaseSO levelDatabase,
            BlockTapGameplayConfigSO gameplayConfig,
            BlockTapVisualConfigSO visualConfig,
            BlockTapAudioConfigSO audioConfig,
            BlockTapPrefabConfigSO prefabConfig)
        {
            this.levelDatabase = levelDatabase;
            this.gameplayConfig = gameplayConfig;
            this.visualConfig = visualConfig;
            this.audioConfig = audioConfig;
            this.prefabConfig = prefabConfig;
            Instance = this;
        }

        public bool TryGetBlockWorldPosition(int row, int column, out Vector3 worldPosition)
        {
            worldPosition = Vector3.zero;

            if (gameplayConfig == null || boardView == null || boardView.BlockContainer == null)
            {
                return false;
            }

            if (column < 0 || column >= gameplayConfig.columns || row < 0 || row >= gameplayConfig.rows)
            {
                return false;
            }

            if (grid != null)
            {
                var block = grid[column, row];
                if (block != null)
                {
                    worldPosition = block.RectTransform.position;
                    return true;
                }
            }

            var localPosition = GetAnchoredPosition(new BlockTapCoordinate(column, row));
            worldPosition = boardView.BlockContainer.TransformPoint(localPosition);
            return true;
        }

        public bool TryGetClickableBlockWorldPosition(out Vector3 worldPosition)
        {
            worldPosition = Vector3.zero;

            if (grid == null || gameplayConfig == null || boardView == null || boardView.BlockContainer == null)
            {
                return false;
            }

            var resolver = selectionResolver ?? new BoardSelectionResolver(gameplayConfig.columns, gameplayConfig.rows);
            BlockTapBlockView bestBlock = null;
            var centerColumn = (gameplayConfig.columns - 1) * 0.5f;
            var centerRow = (gameplayConfig.rows - 1) * 0.5f;
            var bestDistance = float.MaxValue;

            for (var row = 0; row < gameplayConfig.rows; row++)
            {
                for (var column = 0; column < gameplayConfig.columns; column++)
                {
                    var block = grid[column, row];
                    if (block == null)
                    {
                        continue;
                    }

                    var selection = resolver.Resolve(grid, block);
                    if (selection.Count < gameplayConfig.minMatchCount)
                    {
                        continue;
                    }

                    var distance = (column - centerColumn) * (column - centerColumn) + (row - centerRow) * (row - centerRow);
                    if (distance >= bestDistance)
                    {
                        continue;
                    }

                    bestDistance = distance;
                    bestBlock = block;
                }
            }

            if (bestBlock == null)
            {
                return false;
            }

            worldPosition = boardView.BlockContainer.TransformPoint(GetAnchoredPosition(bestBlock.Coordinate));
            return true;
        }

        private void Start()
        {
            EnsureConfigs();
            EnsureSceneBindings();

            if (hudView == null || boardView == null)
            {
                enabled = false;
                return;
            }

            var bombPrefab = prefabConfig != null ? prefabConfig.bombEffectPrefab : null;

            boardGenerator = new BoardGenerator(gameplayConfig);
            selectionResolver = new BoardSelectionResolver(gameplayConfig.columns, gameplayConfig.rows);
            collapseResolver = new BoardCollapseResolver();
            goalTracker = new LevelGoalTracker();
            progressionController = new LevelProgressionController(levelDatabase);
            propController = new PropController();
            propController.Initialize(gameplayConfig.startingShuffleProps, gameplayConfig.startingBombProps);
            animationPlayer = new BoardAnimationPlayer(
                visualConfig,
                hudView.HudRoot,
                boardView.BoardRoot,
                boardView.EffectsRoot,
                boardView.FlyingContainer,
                bombPrefab);
            audioCueController = new AudioCueController(transform, audioConfig);

            hudView.ShuffleButton.onClick.AddListener(OnShufflePressed);
            hudView.BombButton.onClick.AddListener(OnBombPressed);
            hudView.RestartButton.onClick.AddListener(OnRestartPressed);
            hudView.NextButton.onClick.AddListener(OnNextPressed);
            audioCueController.PlayBgm();
            LoadLevel(false);
        }

        private void LoadLevel(bool animateTransition)
        {
            if (reloadBoardRoutine != null)
            {
                StopCoroutine(reloadBoardRoutine);
                reloadBoardRoutine = null;
            }

            if (initialBoardReadyRoutine != null)
            {
                StopCoroutine(initialBoardReadyRoutine);
                initialBoardReadyRoutine = null;
            }

            if (levelCompleteRoutine != null)
            {
                StopCoroutine(levelCompleteRoutine);
                levelCompleteRoutine = null;
            }

            inputLocked = true;
            levelTransitioning = animateTransition;
            BridgingUtil.CannotShowGuide();
            animationPlayer?.StopBombPulse(hudView?.BombButton != null ? hudView.BombButton.GetComponent<RectTransform>() : null);
            ClearBombSelectionVisuals();

            var level = progressionController.GetCurrentLevel();
            if (level != null && level.pass == 0)
            {
                Random.InitState(1);
            }
            var requestedTargetCount = ResolveTargetColorCount(level);
            targetColors = boardGenerator.PickTargetColors(requestedTargetCount);
            dropColorPool = boardGenerator.BuildDropColorPool(targetColors);
            goalTracker.Configure(level, targetColors);
            propController.ResetSelectionState();
            effectiveEliminationCount = 0;

            hudView.SetLevelText($"{progressionController.CurrentLevelNumber}");
            hudView.SetMessage("Find linked colors");
            hudView.RebuildGoals(targetColors, goalTracker.RemainingByColor, goalTracker.TargetByColor, ResolveGoalSprite);
            RefreshPropLabels();

            if (animateTransition)
            {
                reloadBoardRoutine = StartCoroutine(ReloadBoardAnimated());
            }
            else
            {
                ClearBoard();
                BuildBoard(false);
                initialBoardReadyRoutine = StartCoroutine(UnlockInitialBoardAfterSpawn());
            }
        }

        private IEnumerator UnlockInitialBoardAfterSpawn()
        {
            var delay = GetSpawnAnimationDuration();
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }

            BridgingUtil.CanShowGuide();
            inputLocked = false;
            levelTransitioning = false;
            initialBoardReadyRoutine = null;
        }

        private IEnumerator ReloadBoardAnimated()
        {
            yield return animationPlayer.AnimateBoardShiftOut().WaitForCompletion();
            ClearBoard();
            BuildBoard(false);
            yield return animationPlayer.AnimateBoardShiftIn().WaitForCompletion();
            
            inputLocked = false;
            levelTransitioning = false;
            reloadBoardRoutine = null;
        }

        private void BuildBoard(bool animateAsShuffle)
        {
            grid = new BlockTapBlockView[gameplayConfig.columns, gameplayConfig.rows];
            var boardWidth = gameplayConfig.columns * visualConfig.boardCellSize + (gameplayConfig.columns - 1) * visualConfig.boardSpacing;
            var boardHeight = gameplayConfig.rows * visualConfig.boardCellSize + (gameplayConfig.rows - 1) * visualConfig.boardSpacing;
            boardView.SetBoardSize(new Vector2(boardWidth, boardHeight));

            for (var column = 0; column < gameplayConfig.columns; column++)
            {
                for (var row = 0; row < gameplayConfig.rows; row++)
                {
                    var block = CreateBlock(column, row, boardGenerator.PickColor(dropColorPool));
                    var finalPosition = GetAnchoredPosition(block.Coordinate);
                    if (animateAsShuffle)
                    {
                        animationPlayer.AnimateShuffleSpawn(block, finalPosition, column, row);
                    }
                    else
                    {
                        animationPlayer.AnimateSpawn(block, finalPosition, column, row);
                    }

                    grid[column, row] = block;
                }
            }
        }

        private BlockTapBlockView CreateBlock(int column, int row, int colorIndex)
        {
            var block = GetBlock();
            var coordinate = new BlockTapCoordinate(column, row);
            block.Setup(colorIndex, coordinate, ResolveBlockSprite(colorIndex));
            block.RectTransform.SetParent(boardView.BlockContainer, false);
            block.RectTransform.sizeDelta = Vector2.one * visualConfig.boardCellSize;
            return block;
        }

        private BlockTapBlockView GetBlock()
        {
            if (pooledBlocks.Count > 0)
            {
                var pooled = pooledBlocks.Dequeue();
                ResetBlockViewState(pooled);
                pooled.gameObject.SetActive(true);
                pooled.Clicked += OnBlockClicked;
                return pooled;
            }

            GameObject go;
            if (boardView.BlockPrefab != null)
            {
                go = Instantiate(boardView.BlockPrefab.gameObject);
            }
            else if (prefabConfig != null && prefabConfig.blockCellPrefab != null)
            {
                go = Instantiate(prefabConfig.blockCellPrefab);
            }
            else
            {
                go = new GameObject("BlockCell", typeof(RectTransform), typeof(UnityEngine.UI.Image), typeof(BlockTapBlockView));
            }

            go.name = "BlockCell";
            var view = go.GetComponent<BlockTapBlockView>();
            view.Clicked += OnBlockClicked;
            return view;
        }

        private void ReturnBlock(BlockTapBlockView block)
        {
            block.Clicked -= OnBlockClicked;
            ResetBlockViewState(block);
            block.transform.SetParent(transform, false);
            block.gameObject.SetActive(false);
            pooledBlocks.Enqueue(block);
        }

        private void ClearBoard()
        {
            if (grid == null)
            {
                return;
            }

            for (var column = 0; column < grid.GetLength(0); column++)
            {
                for (var row = 0; row < grid.GetLength(1); row++)
                {
                    if (grid[column, row] != null)
                    {
                        ReturnBlock(grid[column, row]);
                        grid[column, row] = null;
                    }
                }
            }
        }

        private void OnBlockClicked(BlockTapBlockView block)
        {
            if (inputLocked || levelTransitioning)
            {
                return;
            }

            audioCueController.PlayTap();

            if (propController.BombSelectionActive)
            {
                StartCoroutine(ExecuteBombSequence(block));
                return;
            }

            var selection = selectionResolver.Resolve(grid, block);
            if (selection.Count < gameplayConfig.minMatchCount)
            {
                hudView.SetMessage("Need at least 2 linked blocks");
                return;
            }

            StartCoroutine(ResolveSelection(selection, true));
        }

        private IEnumerator ResolveSelection(List<BlockTapBlockView> selection, bool countRhythm)
        {
            inputLocked = true;
            var willComplete = PredictionComplete(selection);
            var collectRequests = new List<CollectAnimationRequest>();
            var collectedBlocks = new HashSet<BlockTapBlockView>();

            foreach (var block in selection)
            {
                if (goalTracker.ConsumeColor(block.ColorIndex))
                {
                    hudView.UpdateGoal(block.ColorIndex, goalTracker.GetCollected(block.ColorIndex));
                    if (goalTracker.IsTargetColor(block.ColorIndex))
                    {
                        var goalView = FindGoalView(block.ColorIndex);
                        if (goalView != null)
                        {
                            collectedBlocks.Add(block);
                            collectRequests.Add(new CollectAnimationRequest(
                                block,
                                goalView,
                                ResolveBlockSprite(block.ColorIndex)));
                        }
                    }
                }
            }

            for (var index = 0; index < selection.Count; index++)
            {
                var block = selection[index];
                if (block == null || collectedBlocks.Contains(block))
                {
                    continue;
                }

                animationPlayer?.PlayEliminationEffect(block);
            }

            var orderedCollectRequests = BuildCollectAnimationOrder(collectRequests);

            for (var index = 0; index < orderedCollectRequests.Count; index++)
            {
                var request = orderedCollectRequests[index];
                animationPlayer.AnimateCollect(
                    request.Block,
                    request.GoalView,
                    request.Sprite,
                    index * CollectFlightStagger,
                    audioCueController.PlayCollect);
            }

            audioCueController.PlayEliminate();
            yield return RemoveBlocks(selection, collectedBlocks);

            if (orderedCollectRequests.Count > 0)
            {
                var collectAnimationDuration = (orderedCollectRequests.Count - 1) * CollectFlightStagger + visualConfig.collectFlyDuration;
                StartCoroutine(FinalizeCollectedBlocksAfterDelay(orderedCollectRequests, collectAnimationDuration));
            }

            if (countRhythm)
            {
                effectiveEliminationCount++;
                if (!SaveDataUtils.GameData.customTutorialEnd)
                {
                    BridgingUtil.NewPlayerGuideEnd();
                    UIModule.Instance.ClosePage(UIPageIds.UI_TeachFinterMove);
                }
                if (effectiveEliminationCount % gameplayConfig.effectiveEliminationsPerRhythmBeat == 0)
                {
                    hudView.SetMessage("Rhythm beat reached");
                }
            }

            if (goalTracker.IsComplete())
            {
                if (levelCompleteRoutine != null)
                {
                    StopCoroutine(levelCompleteRoutine);
                }
                Real_GetRewardPanelUtil.OpenGetRewardPanel(DoubleGetRewardPanel.E_UseScene.WinPanel, (a) =>
                {
                    levelCompleteRoutine = StartCoroutine(AdvanceToNextLevelAfterDelay());
                });
                
                yield break;
            }
            else if (countRhythm)
            {
                NumbericalStatistics.CheckShowGetReward();
            }

            yield return CollapseBoard(selection);
            inputLocked = false;
        }

        private IEnumerator AdvanceToNextLevelAfterDelay()
        {
            inputLocked = true;
            levelTransitioning = true;
            PlayGameSfx("Win");
            hudView.SetMessage("Level clear");
            progressionController.Advance();
            yield return new WaitForSeconds(gameplayConfig.nextLevelDelay);
            levelCompleteRoutine = null;
            LoadLevel(true);
        }

        private IEnumerator RemoveBlocks(List<BlockTapBlockView> selection, HashSet<BlockTapBlockView> collectedBlocks)
        {
            var completed = 0;
            foreach (var block in selection)
            {
                grid[block.Coordinate.Column, block.Coordinate.Row] = null;
                var isTargetBlock = goalTracker.IsTargetColor(block.ColorIndex);
                if (isTargetBlock)
                {
                    if (!collectedBlocks.Contains(block))
                    {
                        ReturnBlock(block);
                    }

                    completed++;
                    continue;
                }

                animationPlayer.AnimateRemoval(block, false, () =>
                {
                    ReturnBlock(block);
                    completed++;
                });
            }

            while (completed < selection.Count)
            {
                yield return null;
            }
        }

        private IEnumerator FinalizeCollectedBlocks(List<CollectAnimationRequest> collectRequests)
        {
            for (var index = 0; index < collectRequests.Count; index++)
            {
                var block = collectRequests[index].Block;
                if (block == null)
                {
                    continue;
                }

                ReturnBlock(block);
            }

            yield break;
        }

        private IEnumerator FinalizeCollectedBlocksAfterDelay(List<CollectAnimationRequest> collectRequests, float delay)
        {
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }

            yield return FinalizeCollectedBlocks(collectRequests);
        }

        private IEnumerator CollapseBoard(List<BlockTapBlockView> selection)
        {
            var eliminateMap = collapseResolver.BuildEliminateMap(selection);
            if (eliminateMap.Count == 0)
            {
                yield break;
            }

            var boardTweens = new List<Tween>();

            foreach (var column in collapseResolver.GetAffectedColumns(eliminateMap))
            {
                var eliminatedRows = eliminateMap[column];
                var nextColumnState = new BlockTapBlockView[gameplayConfig.rows];

                for (var row = 0; row < gameplayConfig.rows; row++)
                {
                    var block = grid[column, row];
                    grid[column, row] = null;
                    if (block == null)
                    {
                        continue;
                    }

                    var targetRow = collapseResolver.GetDroppedRow(row, eliminatedRows);
                    var targetCoordinate = new BlockTapCoordinate(column, targetRow);
                    block.SetCoordinate(targetCoordinate);
                    nextColumnState[targetRow] = block;

                    if (targetRow != row)
                    {
                        boardTweens.Add(animationPlayer.AnimateMove(block, GetAnchoredPosition(targetCoordinate)));
                    }
                }

                for (var index = 0; index < eliminatedRows.Count; index++)
                {
                    var temporaryRow = gameplayConfig.rows + index;
                    var targetRow = collapseResolver.GetDroppedRow(temporaryRow, eliminatedRows);
                    var targetCoordinate = new BlockTapCoordinate(column, targetRow);
                    var block = CreateBlock(column, temporaryRow, boardGenerator.PickColor(dropColorPool));
                    block.SetCoordinate(targetCoordinate);
                    nextColumnState[targetRow] = block;
                    boardTweens.Add(animationPlayer.AnimateSpawn(block, GetAnchoredPosition(targetCoordinate), column, targetRow, true));
                }

                for (var row = 0; row < gameplayConfig.rows; row++)
                {
                    grid[column, row] = nextColumnState[row];
                }
            }

            yield return WaitForTweensCompletion(boardTweens);
        }

        private static IEnumerator WaitForTweensCompletion(List<Tween> tweens)
        {
            if (tweens == null || tweens.Count == 0)
            {
                yield break;
            }

            while (true)
            {
                var hasRunningTween = false;
                for (var index = 0; index < tweens.Count; index++)
                {
                    var tween = tweens[index];
                    if (tween != null && tween.IsActive() && !tween.IsComplete())
                    {
                        hasRunningTween = true;
                        break;
                    }
                }

                if (!hasRunningTween)
                {
                    yield break;
                }

                yield return null;
            }
        }

        private IEnumerator ExecuteBombSequence(BlockTapBlockView targetBlock)
        {
            inputLocked = true;
            propController.ConsumeBombSelection();
            animationPlayer.StopBombPulse(hudView.BombButton.GetComponent<RectTransform>());
            var screenPosition = targetBlock.RectTransform.position;
            if (activeBombIcon != null)
            {
                var iconTween = animationPlayer.AnimateBombIconToTarget(activeBombIcon, screenPosition);
                if (iconTween != null)
                {
                    yield return iconTween.WaitForCompletion();
                }

                animationPlayer.ReleaseFlyingBombIcon(activeBombIcon);
                activeBombIcon = null;
            }

            animationPlayer.AnimateBombBurst(screenPosition, Color.white);
            PlayGameSfx("bomb");
            hudView.SetMessage($"Bomb removed color {targetBlock.ColorIndex + 1}");
            RefreshPropLabels();

            var selection = new List<BlockTapBlockView>();
            for (var column = 0; column < gameplayConfig.columns; column++)
            {
                for (var row = 0; row < gameplayConfig.rows; row++)
                {
                    var block = grid[column, row];
                    if (block != null && block.ColorIndex == targetBlock.ColorIndex)
                    {
                        selection.Add(block);
                    }
                }
            }

            yield return PlayBombShockwave(targetBlock, selection);

            if (selection.Count > 0)
            {
                yield return ResolveSelection(selection, false);
                yield break;
            }

            inputLocked = false;
        }

        private bool PredictionComplete(List<BlockTapBlockView> selection)
        {
            var remaining = new Dictionary<int, int>();
            foreach (var colorIndex in goalTracker.OrderedTargetColors)
            {
                remaining[colorIndex] = goalTracker.GetRemaining(colorIndex);
            }

            foreach (var block in selection)
            {
                if (remaining.ContainsKey(block.ColorIndex))
                {
                    remaining[block.ColorIndex] = Mathf.Max(0, remaining[block.ColorIndex] - 1);
                }
            }

            foreach (var pair in remaining)
            {
                if (pair.Value > 0)
                {
                    return false;
                }
            }

            return true;
        }

        private BlockTapGoalView FindGoalView(int colorIndex)
        {
            foreach (var goalView in hudView.GoalViews)
            {
                if (goalView.ColorIndex == colorIndex)
                {
                    return goalView;
                }
            }

            return null;
        }

        private Vector2 GetAnchoredPosition(BlockTapCoordinate coordinate)
        {
            var boardWidth = gameplayConfig.columns * visualConfig.boardCellSize + (gameplayConfig.columns - 1) * visualConfig.boardSpacing;
            var boardHeight = gameplayConfig.rows * visualConfig.boardCellSize + (gameplayConfig.rows - 1) * visualConfig.boardSpacing;
            var originX = -boardWidth * 0.5f + visualConfig.boardCellSize * 0.5f;
            var originY = -boardHeight * 0.5f + visualConfig.boardCellSize * 0.5f;

            return new Vector2(
                originX + coordinate.Column * (visualConfig.boardCellSize + visualConfig.boardSpacing),
                originY + coordinate.Row * (visualConfig.boardCellSize + visualConfig.boardSpacing));
        }

        private float GetSpawnAnimationDuration()
        {
            if (gameplayConfig == null || visualConfig == null)
            {
                return 0f;
            }

            var maxColumnDelay = Mathf.Max(0, gameplayConfig.columns - 1) * visualConfig.spawnColumnDelay;
            var maxRowDelay = Mathf.Max(0, gameplayConfig.rows - 1) * visualConfig.spawnRowDelay;
            return maxColumnDelay + maxRowDelay + visualConfig.spawnDuration;
        }

        private Sprite ResolveBlockSprite(int colorIndex)
        {
            if (visualConfig.blockSprites == null || visualConfig.blockSprites.Length == 0)
            {
                return null;
            }

            var clampedIndex = Mathf.Clamp(colorIndex, 0, visualConfig.blockSprites.Length - 1);
            return visualConfig.blockSprites[clampedIndex];
        }

        private Sprite ResolveGoalSprite(int colorIndex)
        {
            if (visualConfig.goalSprites != null && visualConfig.goalSprites.Length > 0)
            {
                var clampedIndex = Mathf.Clamp(colorIndex, 0, visualConfig.goalSprites.Length - 1);
                if (visualConfig.goalSprites[clampedIndex] != null)
                {
                    return visualConfig.goalSprites[clampedIndex];
                }
            }

            return ResolveBlockSprite(colorIndex);
        }

        public void OnShufflePressed()
        {
            if (inputLocked)
            {
                return;
            }

            if (!propController.TryUseShuffle())
            {
                AddPropCount(PropController.PropType.Shuffle);
                return;
            }
            PlayGameSfx("refresh");
            hudView.SetMessage("Board reshuffled");
            RefreshPropLabels();
            ClearBombSelectionVisuals();
            ReshuffleBoardPreserveProgress();
        }

        private void OnClickProp(E_ItemType itemType)
        {
            var _propUseTimes = NumbericalStatistics._propUseTimes;
            if (_propUseTimes.TryGetValue(itemType, out var curTimes))
            {
                if (curTimes >= TableUtils.Global.PropUseMaxTimes)
                {
                    UIUtils.ShowLanguageTips("Tips_PropReachMaxTimes");
                    return;
                }
            }

            if (ItemUtils.GetItemCount(itemType) > 0)
            {
                if (PropUtils.UseProp(itemType))
                {
                    if (ItemUtils.TryReduceItem(itemType))
                    {
                        LogLogger.LogInfo($"使用了道具:{itemType}");

                        if (!_propUseTimes.ContainsKey(itemType))
                        {
                            _propUseTimes.Add(itemType, 0);
                        }
                        switch (itemType)
                        {
                            case E_ItemType.GameProp_Back:
                                audioCueController.PlayProp();
                                hudView.SetMessage("Board reshuffled");
                                RefreshPropLabels();
                                ClearBombSelectionVisuals();
                                ReshuffleBoardPreserveProgress();
                                break;
                            case E_ItemType.GameProp_ClearArea:
                                hudView.SetMessage("Select a color to bomb");
                                animationPlayer.AnimateBombPulse(hudView.BombButton.GetComponent<RectTransform>());
                                RefreshPropLabels();
                                LaunchBombReadyIcon();
                                break;
                        }

                        _propUseTimes[itemType]++;
                        StatisticModule.Instance.UseItem(itemType, SaveDataUtils.GameData.playerSelectedLv, _propUseTimes[itemType]);
                    }
                }
            }
            else
            {
                LogLogger.LogInfo($"道具数量不足，无法使用:{itemType}");
                UIModule.Instance.OpenPage(UIPageIds.AddPropPanel, itemType);
            }
        }

        public void OnBombPressed()
        {
            if (inputLocked)
            {
                return;
            }

            if (propController.BombSelectionActive)
            {
                hudView.SetMessage("Select a color to bomb");
                return;
            }

            if (!propController.TryArmBomb())
            {
                AddPropCount(PropController.PropType.Bomb);
                return;
            }

            hudView.SetMessage("Select a color to bomb");
            animationPlayer.AnimateBombPulse(hudView.BombButton.GetComponent<RectTransform>());
            RefreshPropLabels();
            LaunchBombReadyIcon();


        }

        public void OnRestartPressed()
        {
            if (inputLocked)
            {
                return;
            }

            hudView.SetMessage("Level restarted");
            LoadLevel(false);
        }

        public void OnNextPressed()
        {
            if (inputLocked)
            {
                return;
            }

            progressionController.Advance();
            LoadLevel(true);
        }

        private static void PlayGameSfx(string soundName)
        {
            if (string.IsNullOrEmpty(soundName))
            {
                return;
            }

            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                System.Type audioManagerType;
                try
                {
                    audioManagerType = assembly.GetTypes().FirstOrDefault(type => type != null && type.Name == "AudioManager");
                }
                catch (System.Reflection.ReflectionTypeLoadException exception)
                {
                    audioManagerType = exception.Types.FirstOrDefault(type => type != null && type.Name == "AudioManager");
                }

                if (audioManagerType == null)
                {
                    continue;
                }

                var instance = audioManagerType
                    .GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    ?.GetValue(null);
                if (instance == null)
                {
                    return;
                }

                var playMethod = audioManagerType.GetMethod(
                    "Play",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                    null,
                    new[] { typeof(string) },
                    null);
                if (playMethod != null)
                {
                    playMethod.Invoke(instance, new object[] { soundName });
                }

                return;
            }
        }

        private void RefreshPropLabels()
        {
            hudView.SetPropCounts(propController.ShuffleCount, propController.BombCount);
        }

        private void AddPropCount(PropController.PropType propType)
        {
            propController.AddProp(propType, 1);
            switch (propType)
            {
                case PropController.PropType.Shuffle:
                    hudView.SetMessage("Got 1 shuffle");
                    break;
                case PropController.PropType.Bomb:
                    hudView.SetMessage("Got 1 bomb");
                    break;
            }

            RefreshPropLabels();
        }

        private void LaunchBombReadyIcon()
        {
            ClearBombSelectionVisuals(false);
            activeBombIcon = animationPlayer.CreateFlyingBombIcon(
                hudView.GetBombIconSprite(),
                hudView.GetBombIconRectTransform(),
                hudView.GetBombIconSize());
            animationPlayer.AnimateBombIconToReady(activeBombIcon);
        }

        private void ClearBombSelectionVisuals(bool clearSelectionState = true)
        {
            animationPlayer?.StopBombPulse(hudView != null && hudView.BombButton != null
                ? hudView.BombButton.GetComponent<RectTransform>()
                : null);

            if (clearSelectionState)
            {
                propController?.ResetSelectionState();
            }

            if (activeBombIcon != null)
            {
                animationPlayer?.ReleaseFlyingBombIcon(activeBombIcon);
                activeBombIcon = null;
            }
        }

        private void ReshuffleBoardPreserveProgress()
        {
            inputLocked = true;
            ClearBoard();
            BuildBoard(true);
            inputLocked = false;
        }

        private IEnumerator PlayBombShockwave(BlockTapBlockView centerBlock, List<BlockTapBlockView> removedBlocks)
        {
            if (centerBlock == null || grid == null)
            {
                yield break;
            }

            var longestDuration = 0f;
            var centerCoordinate = centerBlock.Coordinate;
            var cellStep = visualConfig.boardCellSize + visualConfig.boardSpacing;
            var maxDistance = Mathf.Sqrt(
                (gameplayConfig.columns - 1) * (gameplayConfig.columns - 1) +
                (gameplayConfig.rows - 1) * (gameplayConfig.rows - 1));

            for (var column = 0; column < gameplayConfig.columns; column++)
            {
                for (var row = 0; row < gameplayConfig.rows; row++)
                {
                    var block = grid[column, row];
                    if (block == null)
                    {
                        continue;
                    }

                    var offset = new Vector2(
                        column - centerCoordinate.Column,
                        row - centerCoordinate.Row);
                    var distance = offset.magnitude;
                    var normalized = distance > 0.01f ? offset / distance : Vector2.zero;
                    var strength01 = 1f - distance / Mathf.Max(1f, maxDistance);
                    var pushDistance = distance > 0.01f
                        ? cellStep * Mathf.Lerp(BombShockwaveMinStrength, BombShockwaveStrength, strength01)
                        : 0f;
                    var pushOffset = normalized * pushDistance;
                    var delay = distance * BombShockwaveDelayPerCell;
                    var basePosition = GetAnchoredPosition(block.Coordinate);
                    var tween = animationPlayer.AnimateBombShockwave(block, basePosition, pushOffset, delay);
                    if (tween == null)
                    {
                        continue;
                    }

                    longestDuration = Mathf.Max(
                        longestDuration,
                        delay + BombShockwavePushDuration + BombShockwaveReturnDuration);
                }
            }

            if (longestDuration > 0f)
            {
                yield return new WaitForSeconds(longestDuration);
            }
        }

        private static void ResetBlockViewState(BlockTapBlockView block)
        {
            if (block == null)
            {
                return;
            }

            block.transform.DOKill();
            block.RectTransform.DOKill();
            block.transform.localScale = Vector3.one;
            block.RectTransform.anchoredPosition = Vector2.zero;
            block.RectTransform.localRotation = Quaternion.identity;
            block.ResetEffectState();

            var image = block.GetComponent<Image>();
            if (image != null)
            {
                image.DOKill();
                var color = image.color;
                color.a = 1f;
                image.color = color;
                image.raycastTarget = true;
            }
        }

        private void EnsureConfigs()
        {
            levelDatabase = levelDatabase != null ? levelDatabase : BlockTapDefaultContent.LoadOrCreateDatabase();
            gameplayConfig = gameplayConfig != null ? gameplayConfig : BlockTapDefaultContent.CreateGameplayConfig();
            visualConfig = visualConfig != null ? visualConfig : BlockTapDefaultContent.CreateVisualConfig();
            audioConfig = audioConfig != null ? audioConfig : BlockTapDefaultContent.CreateAudioConfig();
            prefabConfig = prefabConfig != null ? prefabConfig : BlockTapDefaultContent.CreatePrefabConfig();
        }

        private void EnsureSceneBindings()
        {
            hudView = hudView != null ? hudView : GetComponentInChildren<BlockTapHudView>(true);
            boardView = boardView != null ? boardView : GetComponentInChildren<BlockTapBoardView>(true);

            if (hudView == null)
            {
                Debug.LogError("BlockTapGameplayController requires a BlockTapHudView in the scene.", this);
            }

            if (boardView == null)
            {
                Debug.LogError("BlockTapGameplayController requires a BlockTapBoardView in the scene.", this);
            }
        }

        private int ResolveTargetColorCount(BlockTapLevelConfigSO level)
        {
            if (level == null)
            {
                return 1;
            }

            var collectCount = level.collect != null ? level.collect.Length : 0;
            var configValue = level.type > 0 ? level.type : level.num;
            var resolved = configValue > 0 ? configValue : collectCount;
            if (collectCount > 0)
            {
                resolved = Mathf.Min(resolved, collectCount);
            }

            return Mathf.Clamp(resolved, 1, gameplayConfig.totalColors);
        }

        private List<CollectAnimationRequest> BuildCollectAnimationOrder(List<CollectAnimationRequest> requests)
        {
            var ordered = new List<CollectAnimationRequest>(requests.Count);
            var rows = requests
                .GroupBy(request => request.Block.Coordinate.Row)
                .OrderByDescending(group => group.Key);

            foreach (var rowGroup in rows)
            {
                var rowRequests = rowGroup.OrderBy(request => request.Block.Coordinate.Column).ToList();
                if (rowRequests.Count == 0)
                {
                    continue;
                }

                var leftMost = rowRequests[0];
                var rightMost = rowRequests[rowRequests.Count - 1];
                var leftDistance = GetCollectDistanceScore(leftMost);
                var rightDistance = GetCollectDistanceScore(rightMost);

                if (rightDistance < leftDistance)
                {
                    rowRequests.Reverse();
                }

                ordered.AddRange(rowRequests);
            }

            return ordered;
        }

        private static float GetCollectDistanceScore(CollectAnimationRequest request)
        {
            return (request.Block.RectTransform.position - request.GoalView.RectTransform.position).sqrMagnitude;
        }

        private readonly struct CollectAnimationRequest
        {
            public CollectAnimationRequest(BlockTapBlockView block, BlockTapGoalView goalView, Sprite sprite)
            {
                Block = block;
                GoalView = goalView;
                Sprite = sprite;
            }

            public BlockTapBlockView Block { get; }
            public BlockTapGoalView GoalView { get; }
            public Sprite Sprite { get; }
        }
    }
}



