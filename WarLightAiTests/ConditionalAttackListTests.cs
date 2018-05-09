using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WarLightAi.Main;
using WarLightAi.Move;

namespace WarLightAiTests
{
    [TestClass]
    public class ConditionalAttackListTests
    {
        private TestGameState _gameState;
        private ConditionalAttackList _attackList;
        private List<AttackTransferMove> _committedMoves;

        [TestInitialize]
        public void Setup()
        {
            _gameState = new TestGameState();
            _attackList = new ConditionalAttackList();
            _committedMoves = new List<AttackTransferMove>();
        }

        [TestMethod]
        public void ResolveConditionalAttacks_LoneTargetWithThreeWeakAttackers_NoMoves()
        {
            var targetRegion = _gameState.AddRegion(TestGameState.EnemyName, 10);
            var attacker1 = _gameState.AddRegion(TestGameState.MyPlayerName, 5);
            var attacker2 = _gameState.AddRegion(TestGameState.MyPlayerName, 5);
            var attacker3 = _gameState.AddRegion(TestGameState.MyPlayerName, 5);
            targetRegion.AddNeighbor(attacker1);
            targetRegion.AddNeighbor(attacker2);
            targetRegion.AddNeighbor(attacker3);

            _attackList.Add(attacker1, targetRegion, attacker1.Armies - 1);
            _attackList.Add(attacker2, targetRegion, attacker2.Armies - 1);
            _attackList.Add(attacker3, targetRegion, attacker3.Armies - 1);

            _attackList.ResolveConditionalAttacks(_committedMoves);

            Assert.AreEqual(0, _committedMoves.Count);
        }

        [TestMethod]
        public void ResolveConditionalAttacks_LoneTargetWithThreeAttackers_CommitsMoves()
        {
            var targetRegion = _gameState.AddRegion(TestGameState.EnemyName, 10);
            var attacker1 = _gameState.AddRegion(TestGameState.MyPlayerName, 8);
            var attacker2 = _gameState.AddRegion(TestGameState.MyPlayerName, 8);
            var attacker3 = _gameState.AddRegion(TestGameState.MyPlayerName, 8);
            targetRegion.AddNeighbor(attacker1);
            targetRegion.AddNeighbor(attacker2);
            targetRegion.AddNeighbor(attacker3);

            _attackList.Add(attacker1, targetRegion, attacker1.Armies - 1);
            _attackList.Add(attacker2, targetRegion, attacker2.Armies - 1);
            _attackList.Add(attacker3, targetRegion, attacker3.Armies - 1);

            _attackList.ResolveConditionalAttacks(_committedMoves);

            Assert.AreEqual(3, _committedMoves.Count);
        }

        [TestMethod]
        public void ResolveConditionalAttacks_LoneTargetWithThreeAttackers_CommitsMovesInDescendingOrder()
        {
            var targetRegion = _gameState.AddRegion(TestGameState.EnemyName, 10);
            var attacker1 = _gameState.AddRegion(TestGameState.MyPlayerName, 6);
            var attacker2 = _gameState.AddRegion(TestGameState.MyPlayerName, 8);
            var attacker3 = _gameState.AddRegion(TestGameState.MyPlayerName, 7);
            targetRegion.AddNeighbor(attacker1);
            targetRegion.AddNeighbor(attacker2);
            targetRegion.AddNeighbor(attacker3);

            _attackList.Add(attacker1, targetRegion, attacker1.Armies - 1);
            _attackList.Add(attacker2, targetRegion, attacker2.Armies - 1);
            _attackList.Add(attacker3, targetRegion, attacker3.Armies - 1);

            _attackList.ResolveConditionalAttacks(_committedMoves);

            Assert.AreEqual(3, _committedMoves.Count);
            Assert.AreEqual(attacker2.Id, _committedMoves[0].FromRegion.Id);
            Assert.AreEqual(attacker3.Id, _committedMoves[1].FromRegion.Id);
            Assert.AreEqual(attacker1.Id, _committedMoves[2].FromRegion.Id);
        }

        [TestMethod]
        public void ResolveConditionalAttacks_LoneTargetWithThreeAttackerdsvcdss_CommitsMovesInDescendingOrder()
        {
            var targetRegion = _gameState.AddRegion(TestGameState.EnemyName, 167);
            var attacker1 = _gameState.AddRegion(TestGameState.MyPlayerName, 105);
            var attacker2 = _gameState.AddRegion(TestGameState.MyPlayerName, 100);
            var attacker3 = _gameState.AddRegion(TestGameState.MyPlayerName, 100);
            targetRegion.AddNeighbor(attacker1);
            targetRegion.AddNeighbor(attacker2);
            targetRegion.AddNeighbor(attacker3);

            _attackList.Add(attacker1, targetRegion, attacker1.Armies - 1);
            _attackList.Add(attacker2, targetRegion, attacker2.Armies - 1);
            _attackList.Add(attacker3, targetRegion, attacker3.Armies - 1);

            _attackList.ResolveConditionalAttacks(_committedMoves);

            Assert.AreEqual(3, _committedMoves.Count);
        }

        [TestMethod]
        public void ResolveConditionalAttacks_TargetWithThreeAttackersWithOtherEnemies_NoMoves()
        {
            var targetRegion = _gameState.AddRegion(TestGameState.EnemyName, 10);
            var otherEnemy = _gameState.AddRegion(TestGameState.EnemyName, 10);
            var attacker1 = _gameState.AddRegion(TestGameState.MyPlayerName, 6);
            var attacker2 = _gameState.AddRegion(TestGameState.MyPlayerName, 8);
            var attacker3 = _gameState.AddRegion(TestGameState.MyPlayerName, 7);
            targetRegion.AddNeighbor(attacker1);
            targetRegion.AddNeighbor(attacker2);
            targetRegion.AddNeighbor(attacker3);
            otherEnemy.AddNeighbor(attacker1);
            otherEnemy.AddNeighbor(attacker2);
            otherEnemy.AddNeighbor(attacker3);

            _attackList.Add(attacker1, targetRegion, attacker1.Armies - 1);
            _attackList.Add(attacker2, targetRegion, attacker2.Armies - 1);
            _attackList.Add(attacker3, targetRegion, attacker3.Armies - 1);

            _attackList.ResolveConditionalAttacks(_committedMoves);

            Assert.AreEqual(0, _committedMoves.Count);
        }

        [TestMethod]
        public void ResolveConditionalAttacks_TargetWithTwoAttackersWithOneWeakEnemy_NoMoves()
        {
            var targetRegion = _gameState.AddRegion(TestGameState.EnemyName, 35);
            var otherEnemy = _gameState.AddRegion(TestGameState.EnemyName, 1);
            var attacker1 = _gameState.AddRegion(TestGameState.MyPlayerName, 22);
            var attacker2 = _gameState.AddRegion(TestGameState.MyPlayerName, 27);
            targetRegion.AddNeighbor(attacker1);
            targetRegion.AddNeighbor(attacker2);
            otherEnemy.AddNeighbor(attacker2);

            _attackList.Add(attacker1, targetRegion, attacker1.Armies - 1);
            _attackList.Add(attacker2, targetRegion, attacker2.Armies - 1);

            _attackList.ResolveConditionalAttacks(_committedMoves);

            Assert.AreEqual(0, _committedMoves.Count);
        }

        [TestMethod]
        public void ResolveConditionalAttacks_TargetWithThreeAttackersAndOneWithOtherEnemy_NoMoves()
        {
            var targetRegion = _gameState.AddRegion(TestGameState.EnemyName, 10);
            var otherEnemy = _gameState.AddRegion(TestGameState.EnemyName, 10);
            var attacker1 = _gameState.AddRegion(TestGameState.MyPlayerName, 6);
            var attacker2 = _gameState.AddRegion(TestGameState.MyPlayerName, 8);
            var attacker3 = _gameState.AddRegion(TestGameState.MyPlayerName, 7);
            targetRegion.AddNeighbor(attacker1);
            targetRegion.AddNeighbor(attacker2);
            targetRegion.AddNeighbor(attacker3);
            otherEnemy.AddNeighbor(attacker3);

            _attackList.Add(attacker1, targetRegion, attacker1.Armies - 1);
            _attackList.Add(attacker2, targetRegion, attacker2.Armies - 1);
            _attackList.Add(attacker3, targetRegion, attacker3.Armies - 1);

            _attackList.ResolveConditionalAttacks(_committedMoves);

            Assert.AreEqual(0, _committedMoves.Count);
        }

        [TestMethod]
        public void ResolveConditionalAttacks_TargetWithFourAttackersButOnlyThreeNeeded_LastAttackerDoesNotParticipate()
        {
            var targetRegion = _gameState.AddRegion(TestGameState.EnemyName, 10);
            var attacker1 = _gameState.AddRegion(TestGameState.MyPlayerName, 6);
            var attacker2 = _gameState.AddRegion(TestGameState.MyPlayerName, 8);
            var attacker3 = _gameState.AddRegion(TestGameState.MyPlayerName, 7);
            var attacker4 = _gameState.AddRegion(TestGameState.MyPlayerName, 2);
            var otherEnemy = _gameState.AddRegion(TestGameState.EnemyName, 100);
            targetRegion.AddNeighbor(attacker1);
            targetRegion.AddNeighbor(attacker2);
            targetRegion.AddNeighbor(attacker3);
            targetRegion.AddNeighbor(attacker4);
            otherEnemy.AddNeighbor(attacker4);

            _attackList.Add(attacker1, targetRegion, attacker1.Armies - 1);
            _attackList.Add(attacker2, targetRegion, attacker2.Armies - 1);
            _attackList.Add(attacker3, targetRegion, attacker3.Armies - 1);
            _attackList.Add(attacker4, targetRegion, attacker4.Armies - 1);

            _attackList.ResolveConditionalAttacks(_committedMoves);

            Assert.AreEqual(3, _committedMoves.Count);
        }

        [TestMethod]
        public void ResolveConditionalAttacks_TargetWithThreeAttackersAndNeutrals_CommitsAttacks()
        {
            var targetRegion = _gameState.AddRegion(TestGameState.EnemyName, 10);
            var neutral1 = _gameState.AddRegion(Constants.NeutralPlayerName, 10);
            var neutral2 = _gameState.AddRegion(Constants.NeutralPlayerName, 10);
            var attacker1 = _gameState.AddRegion(TestGameState.MyPlayerName, 8);
            var attacker2 = _gameState.AddRegion(TestGameState.MyPlayerName, 8);
            var attacker3 = _gameState.AddRegion(TestGameState.MyPlayerName, 8);
            targetRegion.AddNeighbor(attacker1);
            targetRegion.AddNeighbor(attacker2);
            targetRegion.AddNeighbor(attacker3);
            neutral1.AddNeighbor(attacker1);
            neutral1.AddNeighbor(attacker2);
            neutral1.AddNeighbor(attacker3);
            neutral2.AddNeighbor(attacker1);
            neutral2.AddNeighbor(attacker2);
            neutral2.AddNeighbor(attacker3);

            _attackList.Add(attacker1, targetRegion, attacker1.Armies - 1);
            _attackList.Add(attacker2, targetRegion, attacker2.Armies - 1);
            _attackList.Add(attacker3, targetRegion, attacker3.Armies - 1);

            _attackList.ResolveConditionalAttacks(_committedMoves);

            Assert.AreEqual(3, _committedMoves.Count);
        }
    }
}
