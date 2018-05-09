using System.Collections.Generic;
using System.Linq;
using WarLightAi.Decisions;
using WarLightAi.Main;

namespace WarLightAi.Analysis
{
    public static class BattleAnalysis
    {
        /// <param name="confidence">Probability that the true result will fall within the low and high numbers in the predicted result</param>
        /// <param name="defenderArmies"></param>
        /// <param name="attackerArmies"></param>
        /// <returns></returns>
        public static BattleResult AnalyzeBattle(float confidence, int defenderArmies, int attackerArmies)
        {
            var result = new BattleResult(confidence, defenderArmies, attackerArmies);

            return result;
        }

        public static int MaximumArmiesNecessaryToConquer(float confidenceForDefiniteWin, float confidenceForPossibleWin, int defenderArmies, int maxAttackerArmiesAvailable)
        {
            int armiesNeeded = 0;
            bool defeated;
            do
            {
                armiesNeeded = System.Math.Min(armiesNeeded + 2, maxAttackerArmiesAvailable);
                BattleResult result = AnalyzeBattle(confidenceForDefiniteWin, defenderArmies, armiesNeeded);
                defeated = result.AttackerExpectedToWinForSure();
            } while (!defeated && armiesNeeded < maxAttackerArmiesAvailable);

            // Putting this here so that we don't throw armies away on battles we absolutely cannot win
            if (!defeated)
            {
                BattleResult result = AnalyzeBattle(confidenceForPossibleWin, defenderArmies, maxAttackerArmiesAvailable);
                armiesNeeded = (result.AttackerExpectedToWinAtAll()) ? maxAttackerArmiesAvailable : int.MaxValue;
            }

            return armiesNeeded;
        }

        public static int DetermineMaximumSafeAttack(Region fromRegion, Region toRegion, string myName, float confidence)
        {
            // this assumption is because we don't know if the enemy has placed reinforcements here or not, but they often have
            var assumedEnemyArmies = (toRegion.PlayerName == Constants.NeutralPlayerName) ? toRegion.Armies : toRegion.Armies + Constants.BaseNewArmiesPerTurn;

            // start from the largest number of armies we could need to win, up to the number we actually have
            var attackingArmies = MaximumArmiesNecessaryToConquer(0.99f, 0.95f, assumedEnemyArmies, fromRegion.Armies - 1);

            //// make that number smaller until its actually safe for us to do
            //if (!AttackIsSafe(fromRegion, attackingArmies, toRegion, assumedEnemyArmies, myName, confidence))
            //    attackingArmies /= 2;

            if (attackingArmies == int.MaxValue || !AttackIsSafe(fromRegion, attackingArmies, toRegion, assumedEnemyArmies, myName, confidence))
                attackingArmies = 0;

            return attackingArmies;
        }

        public static bool AttackIsSafe(Region fromRegion, int attackingArmies, Region toRegion, int defendingArmies, string myName, float confidence)
        {
            var nonAttackingArmies = fromRegion.Armies - attackingArmies;
            BattleResult attackResult = AnalyzeBattle(confidence, defendingArmies, attackingArmies);

            List<Region> unconqueredNeighbors;
            int expectedArmiesLeftToDefend;
            if (attackResult.AttackerExpectedToWinForSure())
            {
                unconqueredNeighbors = fromRegion.Neighbors.Where(x => x.Id != toRegion.Id).ToList();
                expectedArmiesLeftToDefend = nonAttackingArmies;
            }
            else if (attackResult.AttackerExpectedToWin())
            {
                unconqueredNeighbors = fromRegion.Neighbors;
                expectedArmiesLeftToDefend = nonAttackingArmies;
            }
            else
            {
                unconqueredNeighbors = fromRegion.Neighbors;
                expectedArmiesLeftToDefend = nonAttackingArmies + attackResult.AttackerRemainingArmiesLow;
            }

            var toRegionActualArmies = toRegion.Armies;
            toRegion.Armies = attackResult.DefenderRemainingArmies;
            bool attackIsSafe = (DefensiveArmiesNeeded.For(unconqueredNeighbors, expectedArmiesLeftToDefend, Constants.SafeNumberOfArmiesNeeded) <= Constants.SafeNumberOfArmiesNeeded
                && DefensiveArmiesNeeded.For(toRegion.Neighbors, attackResult.AttackerRemainingArmiesLow, Constants.SafeNumberOfArmiesNeeded) <= Constants.SafeNumberOfArmiesNeeded);
            toRegion.Armies = toRegionActualArmies;

            return attackIsSafe;
        }
    }
}
