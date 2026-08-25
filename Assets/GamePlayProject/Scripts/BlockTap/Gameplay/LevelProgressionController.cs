using BlockTap.Config;

namespace BlockTap.Gameplay
{
    public class LevelProgressionController
    {
        private readonly BlockTapLevelDatabaseSO database;
        private int currentIndex => SaveDataUtils.GameData.playerSelectedLv;

        public LevelProgressionController(BlockTapLevelDatabaseSO database)
        {
            this.database = database;
        }

        public int CurrentLevelNumber
        {
            get => SaveDataUtils.GameData.playerSelectedLv;
        }

        public BlockTapLevelConfigSO GetCurrentLevel()
        {
            return database.GetLevel(currentIndex);
        }

        public void RestartLevel()
        {
        }

        public void Advance()
        {
            // currentIndex = database.Count > 0 ? (currentIndex + 1) % database.Count : 0;
        }
    }
}
