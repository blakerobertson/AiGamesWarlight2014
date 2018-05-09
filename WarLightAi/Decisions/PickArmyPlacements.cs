using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using WarLightAi.Analysis;
using WarLightAi.Bot;
using WarLightAi.Main;
using WarLightAi.Move;

namespace WarLightAi.Decisions
{
    public class PickArmyPlacements
    {
        private static float confidence = 0.90f;

        public List<PlaceArmiesMove> BasedOn(GameState state)
        {
            var placeArmiesMoves = new List<PlaceArmiesMove>();

            int armiesLeft = state.StartingArmies;

            armiesLeft = PlaceDefensiveArmies(armiesLeft, state, placeArmiesMoves);
            PlaceOffensiveArmies(armiesLeft, state, placeArmiesMoves);

            return placeArmiesMoves;
        }

        private static int PlaceDefensiveArmies(int armiesLeft, GameState state, List<PlaceArmiesMove> placeArmiesMoves)
        {
            if (armiesLeft == 0)
                return armiesLeft;

            var regionsByNeed = DefensiveArmiesNeeded.RankRegionsByNeed(state.VisibleMap, GameState.MyPlayerName, armiesLeft);

            for (int i = 0; armiesLeft > 0 && i < regionsByNeed.Count; i++)
            {
                if (armiesLeft >= regionsByNeed[i].Item2 && regionsByNeed[i].Item2 > 0)
                {
                    placeArmiesMoves.Add(new PlaceArmiesMove(GameState.MyPlayerName, regionsByNeed[i].Item1, regionsByNeed[i].Item2));
                    armiesLeft -= regionsByNeed[i].Item2;
                }
            }

            return armiesLeft;
        }

        private static int PlaceOffensiveArmies(int armiesLeft, GameState state, List<PlaceArmiesMove> placeArmiesMoves)
        {
            if (armiesLeft == 0)
                return armiesLeft;

            foreach (var targetRegion in StrategicMap.UncontrolledRegionsByValue)
            {
                foreach (var offensiveRegion in targetRegion.Neighbors)
                {
                    if (offensiveRegion.PlayerName == GameState.MyPlayerName)
                    {
                        BattleResult result = BattleAnalysis.AnalyzeBattle(confidence, targetRegion.Armies, offensiveRegion.Armies + armiesLeft);
                        if (result.AttackerExpectedToWinAtAll())
                        {
                            if (offensiveRegion.Armies > Constants.DeadlockThreshold && armiesLeft > 1)
                                armiesLeft = PlaceFailSafeArmy(offensiveRegion, 1, armiesLeft, placeArmiesMoves);

                            placeArmiesMoves.Add(new PlaceArmiesMove(GameState.MyPlayerName, offensiveRegion, armiesLeft));
                            armiesLeft = 0;
                        }
                    }

                    if (armiesLeft == 0)
                        break;
                }

                if (armiesLeft == 0)
                    break;
            }

            if (armiesLeft > 0)
            {
                PlaceRemainingArmies(armiesLeft, placeArmiesMoves);
                armiesLeft = 0;
            }

            return armiesLeft;
        }

        // This gets called when all reasonable defensive armies have been placed and no region was in a position to win an attack this turn
        private static void PlaceRemainingArmies(int armiesLeft, List<PlaceArmiesMove> placeArmiesMoves)
        {
            var mostValuableRegion = StrategicMap.UncontrolledRegionsByValue.First();
            var mvpAttackerArmies = mostValuableRegion.Neighbors.Where(x => x.PlayerName == GameState.MyPlayerName).Max(x => x.Armies);
            var mvpAttacker = mostValuableRegion.Neighbors.First(x => x.PlayerName == GameState.MyPlayerName && x.Armies == mvpAttackerArmies);
            if (mvpAttacker.Armies > Constants.DeadlockThreshold && armiesLeft > 1)
            {
                int failSafeArmies = armiesLeft;
                armiesLeft = PlaceFailSafeArmy(mostValuableRegion, failSafeArmies, armiesLeft, placeArmiesMoves);
            }

            if (armiesLeft >= 1)
                placeArmiesMoves.Add(new PlaceArmiesMove(GameState.MyPlayerName, mvpAttacker, armiesLeft));
        }

        private static int PlaceFailSafeArmy(Region mostValuableRegion, int failSafeArmies, int armiesLeft, List<PlaceArmiesMove> placeArmiesMoves)
        {
            var nextMostValuable = StrategicMap.UncontrolledRegionsByValue.FirstOrDefault(x => x.SuperRegion.Id != mostValuableRegion.SuperRegion.Id);
            if (nextMostValuable != null)
            {
                var backupAttackerArmies =
                    nextMostValuable.Neighbors.Where(x => x.PlayerName == GameState.MyPlayerName).Max(x => x.Armies);
                var backupAttacker =
                    nextMostValuable.Neighbors.First(
                        x => x.PlayerName == GameState.MyPlayerName && x.Armies == backupAttackerArmies);
                placeArmiesMoves.Add(new PlaceArmiesMove(GameState.MyPlayerName, backupAttacker, failSafeArmies));
                armiesLeft -= failSafeArmies;
            }
            return armiesLeft;
        }
    }
}
