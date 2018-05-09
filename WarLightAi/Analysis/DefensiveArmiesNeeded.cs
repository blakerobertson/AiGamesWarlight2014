using System;
using System.Collections.Generic;
using System.Linq;
using WarLightAi.Bot;
using WarLightAi.Main;

namespace WarLightAi.Analysis
{
    public static class DefensiveArmiesNeeded
    {
        public const float confidence = 0.90f;

        public static List<Tuple<Region, int>> RankRegionsByNeed(Map visibleMap, string myName, int armiesAvailable)
        {
            var myRegions = new List<Tuple<Region, int>>();

            foreach (var region in visibleMap.Regions)
            {
                if (region.OwnedByPlayer(myName))
                {
                    int armiesNeeded = DefensiveArmiesNeeded.For(region.Neighbors, region.Armies, armiesAvailable);
                    var myRegion = new Tuple<Region, int>(region, armiesNeeded);
                    myRegions.Add(myRegion);
                }
            }

            myRegions.Sort((a, b) => b.Item2.CompareTo(a.Item2));
            return myRegions;
        }

        public static int For(List<Region> neighbors, int existingArmies, int maxArmiesAvailable)
        {
            List<Tuple<Region, int>> enemyNeighbors = neighbors.Where(x => x.PlayerName == GameState.GetOpponentPlayerName)
                    .Select(x => new Tuple<Region, int>(x, x.Armies))
                    .ToList();

            if (enemyNeighbors.Count == 0)
                return 0;

            int remainingDefenderArmies;
            int neededArmies = -1;
            int attackingBonusArmies = Constants.BaseNewArmiesPerTurn;
            enemyNeighbors[0] = new Tuple<Region, int>(enemyNeighbors[0].Item1, enemyNeighbors[0].Item2 + attackingBonusArmies);

            do
            {
                neededArmies++;
                var allRemainingArmies = RemainingArmiesAfterFullScaleAssault(existingArmies + neededArmies, enemyNeighbors);
                remainingDefenderArmies = allRemainingArmies[-1];
            } while (remainingDefenderArmies < 1 && neededArmies < maxArmiesAvailable);

            if (remainingDefenderArmies == 0)
                neededArmies = int.MaxValue; // we can't give it enough armies to survive, so it may as well need infinity

            return neededArmies;
        }

        public static Dictionary<int,int> RemainingArmiesAfterFullScaleAssault(int defendingArmies, List<Tuple<Region,int>> attackingRegionArmies)
        {
            var remainingArmies = new Dictionary<int, int>();

            foreach (var attackingRegion in attackingRegionArmies)
            {
                BattleResult result = BattleAnalysis.AnalyzeBattle(confidence, defendingArmies, attackingRegion.Item2);
                defendingArmies = result.DefenderRemainingArmies;
                remainingArmies.Add(attackingRegion.Item1.Id, result.AttackerRemainingArmiesLow);

                // no need for any other regions to participate if the enemy is destroyed
                if (defendingArmies == 0)
                    break;
            }

            remainingArmies.Add(-1,defendingArmies);
            return remainingArmies;
        }

        private static int RemainingArmiesAfterFullScaleAssault(int defendingArmies, List<Region> attackingRegions, int attackingBonusArmies)
        {
            foreach (var neighbor in attackingRegions)
            {
                BattleResult result = BattleAnalysis.AnalyzeBattle(confidence, defendingArmies, neighbor.Armies + attackingBonusArmies);
                defendingArmies = result.DefenderRemainingArmies;
                attackingBonusArmies = 0; // we only want the bonus applied once so set to zero after first pass
            }

            return defendingArmies;
        }
    }
}
