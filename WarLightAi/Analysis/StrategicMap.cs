using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WarLightAi.Bot;
using WarLightAi.Main;

namespace WarLightAi.Analysis
{
    public static class StrategicMap
    {
        private static List<Region> _rankedRegions; 
        private static List<SuperRegion> _rankedSuperRegions;
        private static HashSet<SuperRegion> _superRegionsInvaded;
        private static HashSet<SuperRegion> _superRegionsControlled;

        public static List<Region> UncontrolledRegionsByValue { get { return _rankedRegions; } }
        public static List<SuperRegion> SuperRegionsByValue { get { return _rankedSuperRegions; } }
        public static HashSet<SuperRegion> SuperRegionsInvaded { get { return _superRegionsInvaded; } }
        public static HashSet<SuperRegion> SuperRegionsControlled { get { return _superRegionsControlled; } }

        public static void AnalyzeMap(GameState state)
        {
            if (_rankedSuperRegions == null)
                _rankedSuperRegions = GetRankedSuperRegionList(state.FullMap);

            SetRegionIsolationLevels(state);
            _superRegionsInvaded = new HashSet<SuperRegion>();
            _superRegionsControlled = new HashSet<SuperRegion>();
            SetSuperRegionInvasionLevels(GameState.MyPlayerName, state.VisibleMap, _superRegionsInvaded, _superRegionsControlled);
            _rankedRegions = GetRankedRegionList(GameState.MyPlayerName, state.VisibleMap);
        }

        private static void SetSuperRegionInvasionLevels(string myName, Map visibleMap, HashSet<SuperRegion> superRegionsInvaded, HashSet<SuperRegion> superRegionsControlled)
        {
            foreach (var superRegion in visibleMap.SuperRegions)
            {
                var regions = superRegion.SubRegions;
                var ownedRegions = regions.Count(x => x.OwnedByPlayer(myName));

                if (ownedRegions >= 1)
                    superRegionsInvaded.Add(superRegion);
                if (ownedRegions == regions.Count)
                    superRegionsControlled.Add(superRegion);
            }
        }


        /// <summary>
        /// Sets the Isolation Levels on player-owned regions in the GameState list of visible regions
        /// </summary>
        /// <param name="state"></param>
        private static void SetRegionIsolationLevels(GameState state)
        {
            var unassignedRegions = state.VisibleMap.Regions.Where(x => x.PlayerName == GameState.MyPlayerName).ToList();

            unassignedRegions.ForEach(x => x.IsolationLevel = null);

            while (unassignedRegions.Count > 0)
            {
                for (int i = unassignedRegions.Count - 1; i >= 0; i--)
                {
                    var region = unassignedRegions[i];

                    if (region.Neighbors.All(x => x.PlayerName == GameState.MyPlayerName && x.IsolationLevel == null))
                        continue;

                    if (region.Neighbors.Any(x => x.PlayerName == GameState.GetOpponentPlayerName))
                        region.IsolationLevel = 0;
                    else if (region.Neighbors.Any(x => x.PlayerName == Constants.NeutralPlayerName))
                        region.IsolationLevel = 1;
                    else if (region.Neighbors.Any(x => x.IsolationLevel != null))
                        region.IsolationLevel = 1 + region.Neighbors.Min(x => x.IsolationLevel);

                    unassignedRegions.Remove(region);
                }
            }
        }

        private static List<Region> GetRankedRegionList(string myName, Map visibleMap)
        {
            var visibleUncontrolledRegions = visibleMap.Regions.Where(x => x.PlayerName != myName).ToList();
            foreach (var region in visibleUncontrolledRegions)
            {
                region.StrategicValue = (10 * region.SuperRegion.StrategicValue);

                if (!StrategicMap.SuperRegionsInvaded.Contains(region.SuperRegion) && (region.PlayerName != Constants.NeutralPlayerName))
                    region.StrategicValue += 10000;
                else if (region.PlayerName != Constants.NeutralPlayerName)
                    region.StrategicValue += 5;
            }

            visibleUncontrolledRegions.Sort((a,b) => b.StrategicValue.CompareTo(a.StrategicValue));
            return visibleUncontrolledRegions;
        }

        /// <summary>
        /// Ranks the super regions in order of strategic value, descending. Note that this is independent of game state.
        /// </summary>
        /// <returns></returns>
        private static List<SuperRegion> GetRankedSuperRegionList(Map fullMap)
        {
            // TODO: put the real calculation in here
            var rankedSuperRegions = fullMap.SuperRegions.ToList();

            // These are the super regions in the game state's full map - they get copied into the visible map every round update
            for (int i = 0; i < rankedSuperRegions.Count; i++)
            {
                var superRegion = rankedSuperRegions[i];
                switch (superRegion.Id)
                {
                    case 1:
                        superRegion.StrategicValue = 2; // North America
                        break;
                    case 2:
                        superRegion.StrategicValue = 6; // South America
                        break;
                    case 3:
                        superRegion.StrategicValue = 5; // Europe
                        break;
                    case 4:
                        superRegion.StrategicValue = 3; // Africa
                        break;
                    case 5:
                        superRegion.StrategicValue = 1; // Asia
                        break;
                    case 6:
                        superRegion.StrategicValue = 4; // Australia
                        break;
                }
            }

            rankedSuperRegions.Sort((a,b) => b.StrategicValue.CompareTo(a.StrategicValue));

            return rankedSuperRegions;
        }
    }
}
