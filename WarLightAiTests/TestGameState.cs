using WarLightAi.Main;

namespace WarLightAiTests
{
    public class TestGameState
    {
        private int _regionId;
        public Map FullMap;
        public const string MyPlayerName = "me";
        public const string EnemyName = "them";

        public TestGameState()
        {
            _regionId = 1;
            FullMap = BuildMap();
        }

        private Map BuildMap()
        {
            var map = new Map();
            map.Add(new SuperRegion(1, 5));
            return map;
        }

        public Region AddRegion(string playerName, int armies)
        {
            var region = new Region(_regionId++, FullMap.GetSuperRegion(1));
            region.PlayerName = playerName;
            region.Armies = armies;
            FullMap.Add(region);
            return region;
        }
    }
}
