using BlockTap.Config;
using BlockTap.Gameplay;
using BlockTap.Importing;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private BlockTapGameplayController gameplayController;

    [Header("Optional Config Assets")]
    [SerializeField] private BlockTapLevelDatabaseSO levelDatabase;
    [SerializeField] private BlockTapGameplayConfigSO gameplayConfig;
    [SerializeField] private BlockTapVisualConfigSO visualConfig;
    [SerializeField] private BlockTapAudioConfigSO audioConfig;
    [SerializeField] private BlockTapPrefabConfigSO prefabConfig;

    public static GameManager Instance;
    public BlockTapGameplayController GameplayController => gameplayController;

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        EnsureConfigs();
        BootstrapGameplay();
        DOTween.SetTweensCapacity(500,125);
    }

    private void EnsureConfigs()
    {
        levelDatabase = levelDatabase != null ? levelDatabase : BlockTapDefaultContent.LoadOrCreateDatabase();
        gameplayConfig = gameplayConfig != null ? gameplayConfig : BlockTapDefaultContent.CreateGameplayConfig();
        visualConfig = visualConfig != null ? visualConfig : BlockTapDefaultContent.CreateVisualConfig();
        audioConfig = audioConfig != null ? audioConfig : BlockTapDefaultContent.CreateAudioConfig();
        prefabConfig = prefabConfig != null ? prefabConfig : BlockTapDefaultContent.CreatePrefabConfig();
    }

    private void BootstrapGameplay()
    {
        gameplayController = gameplayController != null ? gameplayController : GetComponent<BlockTapGameplayController>();
        if (gameplayController == null)
        {
            gameplayController = gameObject.AddComponent<BlockTapGameplayController>();
        }

        gameplayController.Initialize(levelDatabase, gameplayConfig, visualConfig, audioConfig, prefabConfig);
    }

    public void RePlayGame()
    {
        BootstrapGameplay();
    }
}
