using System;
using System.Collections.Generic;
using System.Linq;
using WarLightAi.Analysis;
using WarLightAi.Bot;
using WarLightAi.Main;
using WarLightAi.Move;

namespace WarLightAi.Decisions
{
    public class PickArmyMovements
    {
        private static float _confidence = 0.90f;

        public List<AttackTransferMove> BasedOn(GameState state)
        {
            var attackTransferMoves = new List<AttackTransferMove>();
            var conditionalAttacks = new ConditionalAttackList();

            var regions = state.VisibleMap.Regions;
            foreach (var fromRegion in regions)
            {
                if (fromRegion.Armies > 1 && fromRegion.OwnedByPlayer(GameState.MyPlayerName))
                {
                    var neighbors = fromRegion.Neighbors;
                    if (fromRegion.Armies > 1 && neighbors.Any(x => x.PlayerName != GameState.MyPlayerName))
                    {
                        ConsiderAttacks(fromRegion, neighbors, attackTransferMoves, conditionalAttacks);
                    }
                    else
                    {
                        MoveToLowerIsolationLevel(fromRegion, neighbors, attackTransferMoves);
                    }
                }
            }

            conditionalAttacks.ResolveConditionalAttacks(attackTransferMoves);

            return attackTransferMoves;
        }

        private static void ConsiderAttacks(Region fromRegion, List<Region> neighbors, List<AttackTransferMove> attackTransferMoves, ConditionalAttackList conditionalAttacks)
        {
            neighbors.Sort((a, b) => b.StrategicValue.CompareTo(a.StrategicValue));
            Region conditionalAttackTarget = null;
                
            foreach (var toRegion in neighbors)
            {
                if (toRegion.PlayerName != GameState.MyPlayerName)
                {
                    var attackingArmies = BattleAnalysis.DetermineMaximumSafeAttack(fromRegion, toRegion, GameState.MyPlayerName, _confidence);

                    // it's not safe to attack this guy alone, but if we have armies left and others will attack too, maybe it will be
                    if (attackingArmies == 0 && conditionalAttackTarget == null)
                        conditionalAttackTarget = toRegion;

                    // don't attack with just one army, it's pretty much always a waste
                    if (attackingArmies > 1)
                    {
                        attackTransferMoves.Add(new AttackTransferMove(GameState.MyPlayerName, fromRegion, toRegion, attackingArmies));
                        fromRegion.Armies -= attackingArmies; // TODO: this isn't entirely true - maybe revisit it in the future
                    }
                }

                if (fromRegion.Armies <= 1)
                    break;
            }

            if (fromRegion.Armies > 2 && conditionalAttackTarget != null)
                conditionalAttacks.Add(fromRegion, conditionalAttackTarget, fromRegion.Armies - 1);
        }

        private void MoveToLowerIsolationLevel(Region fromRegion, List<Region> neighbors, List<AttackTransferMove> attackTransferMoves)
        {
            var neighborsMinIsolationLevel = neighbors.Min(x => x.IsolationLevel);
            var neighborWithMinIsolation = neighbors.First(x => x.IsolationLevel == neighborsMinIsolationLevel);
            attackTransferMoves.Add(new AttackTransferMove(GameState.MyPlayerName, fromRegion, neighborWithMinIsolation, (fromRegion.Armies - 1)));
        }
    }
}
