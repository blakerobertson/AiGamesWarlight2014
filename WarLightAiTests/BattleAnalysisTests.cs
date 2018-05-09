using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WarLightAi;
using WarLightAi.Analysis;
using WarLightAi.Main;

namespace WarLightAiTests
{
    [TestClass]
    public class BattleAnalysisTests
    {
        private string _myName = "me";
        private TestGameState _gameState;

        [TestInitialize]
        public void Setup()
        {
            _gameState = new TestGameState();
        }

        [TestMethod]
        public void SimulateBattle_Attackers10Defenders5Threshold75_()
        {
            var result = BattleAnalysis.AnalyzeBattle(0.75f, 5, 10);

            Assert.AreEqual(7, result.AttackerRemainingArmies);
            Assert.AreEqual(8, result.AttackerRemainingArmiesHigh);
            Assert.AreEqual(5, result.AttackerRemainingArmiesLow);
            Assert.AreEqual(0, result.DefenderRemainingArmies);
            Assert.AreEqual(1, result.DefenderRemainingArmiesHigh);
            Assert.AreEqual(0, result.DefenderRemainingArmiesLow);
        }

        [TestMethod]
        public void SimulateBattle_Attackers10Defenders5Threshold90_()
        {
            var result = BattleAnalysis.AnalyzeBattle(0.90f, 5, 10);

            Assert.AreEqual(7, result.AttackerRemainingArmies);
            Assert.AreEqual(8, result.AttackerRemainingArmiesHigh);
            Assert.AreEqual(5, result.AttackerRemainingArmiesLow);
            Assert.AreEqual(0, result.DefenderRemainingArmies);
            Assert.AreEqual(2, result.DefenderRemainingArmiesHigh);
            Assert.AreEqual(0, result.DefenderRemainingArmiesLow);
        }

        [TestMethod]
        public void SimulateBattle_Attackers10Defenders10Threshold90_()
        {
            var result = BattleAnalysis.AnalyzeBattle(0.90f, 10, 10);

            Assert.AreEqual(4, result.AttackerRemainingArmies);
            Assert.AreEqual(5, result.AttackerRemainingArmiesHigh);
            Assert.AreEqual(1, result.AttackerRemainingArmiesLow);
            Assert.AreEqual(4, result.DefenderRemainingArmies);
            Assert.AreEqual(7, result.DefenderRemainingArmiesHigh);
            Assert.AreEqual(2, result.DefenderRemainingArmiesLow);
        }

        [TestMethod]
        public void MaximumArmiesNecessaryToConquer_Attackers24Defenders1Threshold99_()
        {
            var result = BattleAnalysis.MaximumArmiesNecessaryToConquer(0.99f, 1, 24);

            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void AttackIsSafe_AttackingNeutralRegion_ReturnsTrue()
        {
            var fromRegion = _gameState.AddRegion(_myName, 2);
            var toRegion = _gameState.AddRegion(Constants.NeutralPlayerName, 2);
            fromRegion.Neighbors.Add(toRegion);
            
            var result = BattleAnalysis.AttackIsSafe(fromRegion, 1, toRegion, toRegion.Armies, _myName, 0.90f);
            
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AttackIsSafe_AttackingEnemyRegion_ReturnsFalse()
        {
            var fromRegion = _gameState.AddRegion(_myName, 2);
            var toRegion = _gameState.AddRegion(TestGameState.EnemyName, 2);
            fromRegion.Neighbors.Add(toRegion);

            var result = BattleAnalysis.AttackIsSafe(fromRegion, 1, toRegion, toRegion.Armies, _myName, 0.90f);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AttackIsSafe_AttackingNeutralRegionWithNeighbors_ReturnsTrue()
        {
            var fromRegion = _gameState.AddRegion(_myName, 2);
            var toRegion = _gameState.AddRegion(Constants.NeutralPlayerName, 2);
            var neighbor2 = _gameState.AddRegion(Constants.NeutralPlayerName, 2);
            var neighbor3 = _gameState.AddRegion(Constants.NeutralPlayerName, 2);
            fromRegion.Neighbors.Add(toRegion);
            fromRegion.Neighbors.Add(neighbor2);
            fromRegion.Neighbors.Add(neighbor3);

            var result = BattleAnalysis.AttackIsSafe(fromRegion, 1, toRegion, toRegion.Armies, _myName, 0.90f);
            
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AttackIsSafe_AttackingEnemyRegionWithNeighbors_ReturnsTrue()
        {
            var fromRegion = _gameState.AddRegion(_myName, 2);
            var toRegion = _gameState.AddRegion(TestGameState.EnemyName, 2);
            var neighbor2 = _gameState.AddRegion(Constants.NeutralPlayerName, 2);
            var neighbor3 = _gameState.AddRegion(Constants.NeutralPlayerName, 2);
            fromRegion.Neighbors.Add(toRegion);
            fromRegion.Neighbors.Add(neighbor2);
            fromRegion.Neighbors.Add(neighbor3);

            var result = BattleAnalysis.AttackIsSafe(fromRegion, 1, toRegion, toRegion.Armies, _myName, 0.90f);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DetermineMaximumSafeAttack_AttackingNeutralRegionWithNeighbors_Returns1()
        {
            var fromRegion = _gameState.AddRegion(_myName, 2);
            var toRegion = _gameState.AddRegion(Constants.NeutralPlayerName, 2);
            var neighbor2 = _gameState.AddRegion(Constants.NeutralPlayerName, 2);
            var neighbor3 = _gameState.AddRegion(Constants.NeutralPlayerName, 2);
            fromRegion.Neighbors.Add(toRegion);
            fromRegion.Neighbors.Add(neighbor2);
            fromRegion.Neighbors.Add(neighbor3);

            var result = BattleAnalysis.DetermineMaximumSafeAttack(fromRegion, toRegion, TestGameState.MyPlayerName, 0.9f);

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void DetermineMaximumSafeAttack_AttackingEnemyRegionWithNeighbors_Returns0()
        {
            var fromRegion = _gameState.AddRegion(_myName, 2);
            var toRegion = _gameState.AddRegion(TestGameState.EnemyName, 2);
            var neighbor2 = _gameState.AddRegion(Constants.NeutralPlayerName, 2);
            var neighbor3 = _gameState.AddRegion(Constants.NeutralPlayerName, 2);
            fromRegion.Neighbors.Add(toRegion);
            fromRegion.Neighbors.Add(neighbor2);
            fromRegion.Neighbors.Add(neighbor3);

            var result = BattleAnalysis.DetermineMaximumSafeAttack(fromRegion, toRegion, TestGameState.MyPlayerName, 0.9f);

            Assert.AreEqual(0, result);
        }
    }
}
