using System;
using System.Collections.Generic;
using System.Linq;
using WarLightAi.Analysis;
using WarLightAi.Bot;
using WarLightAi.Main;

namespace WarLightAi.Move
{
    public class ConditionalAttackList
    {
        private readonly Dictionary<Region, List<Tuple<Region, int>>> _conditionalAttacks;

        public ConditionalAttackList()
        {
            _conditionalAttacks = new Dictionary<Region, List<Tuple<Region, int>>>();
        }

        public void Add(Region attackingRegion, Region defendingRegion, int conditionalAttackingArmies)
        {
            if (!_conditionalAttacks.ContainsKey(defendingRegion))
                _conditionalAttacks.Add(defendingRegion, new List<Tuple<Region, int>>());

            _conditionalAttacks[defendingRegion].Add(new Tuple<Region, int>(attackingRegion, conditionalAttackingArmies));
        }

        public void ResolveConditionalAttacks(List<AttackTransferMove> committedAttackTransferMoves)
        {
            foreach (var attack in _conditionalAttacks)
            {
                var target = attack.Key;
                var attackerList = attack.Value;
                attackerList.Sort((a,b) => b.Item2.CompareTo(a.Item2));

                Dictionary<int, int> remainingArmies = DefensiveArmiesNeeded.RemainingArmiesAfterFullScaleAssault(target.Armies, attackerList);
                if (ConditionalAttackIsSafe(target, attackerList, remainingArmies))
                {
                    CommitAttack(attack, remainingArmies.Keys.ToList(), committedAttackTransferMoves);
                }
            }
        }

        private bool ConditionalAttackIsSafe(Region target, List<Tuple<Region, int>> attackerList, Dictionary<int, int> remainingArmies)
        {
            bool attackIsSafe = true;
            var defenderArmiesRemaining = remainingArmies[-1];
            foreach (var attacker in attackerList)
            {
                if (!attackIsSafe)
                    break;

                var region = attacker.Item1;
                if (!remainingArmies.ContainsKey(region.Id))
                    continue;

                List<Region> unconqueredNeighbors;
                int armiesRemainingOnAttackingRegion = (attacker.Item1.Armies - attacker.Item2);
                if (defenderArmiesRemaining == 0)
                {
                    unconqueredNeighbors = attacker.Item1.Neighbors.Where(x => x.PlayerName == GameState.GetOpponentPlayerName && x.Id != target.Id).ToList();
                }
                else
                {
                    unconqueredNeighbors = attacker.Item1.Neighbors.Where(x => x.PlayerName == GameState.GetOpponentPlayerName).ToList();
                    armiesRemainingOnAttackingRegion += remainingArmies[region.Id];
                }

                var targetActualArmies = target.Armies;
                target.Armies = defenderArmiesRemaining;
                if (DefensiveArmiesNeeded.For(unconqueredNeighbors, armiesRemainingOnAttackingRegion, Constants.SafeNumberOfArmiesNeeded) > Constants.SafeNumberOfArmiesNeeded)
                {
                    attackIsSafe = false;
                }
                target.Armies = targetActualArmies;
            }

            return attackIsSafe;
        }

        private void CommitAttack(KeyValuePair<Region, List<Tuple<Region, int>>> attack, List<int> participatingAttackers, List<AttackTransferMove> committedAttackTransferMoves)
        {
            var targetRegion = attack.Key;
            
            foreach (var attackPart in attack.Value)
            {
                var attackingRegion = attackPart.Item1;
                if (participatingAttackers.Contains(attackingRegion.Id))
                {
                    var attackingArmies = attackPart.Item2;
                    committedAttackTransferMoves.Add(new AttackTransferMove(GameState.MyPlayerName, attackingRegion,
                        targetRegion, attackingArmies));
                }
            }
        }
    }
}
