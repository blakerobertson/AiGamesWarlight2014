using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WarLightAi.Analysis;
using WarLightAi.Main;

namespace WarLightAiTests
{
    [TestClass]
    public class DetermineRegionArmyNeedTests
    {
        
        private string _myName = "me";
        private string _enemyName = "them";

        private TestGameState _gameState;

        [TestInitialize]
        public void Setup()
        {
            _gameState = new TestGameState();
        }

        [TestMethod]
        public void For_NeedsAFewArmies_ReturnsNeededNumber()
        {
            var testRegion = _gameState.AddRegion(_myName, 6);

            var neighbor1 = _gameState.AddRegion(_enemyName, 5);
            testRegion.AddNeighbor(neighbor1);

            var neighbor2 = _gameState.AddRegion(_enemyName, 5);
            testRegion.AddNeighbor(neighbor2);

            var result = DefensiveArmiesNeeded.For(testRegion.Neighbors, testRegion.Armies, 5);

            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void For_NeutralRegionIsStacked_IgnoresNeutralRegion()
        {
            var testRegion = _gameState.AddRegion(_myName, 6);

            var neighbor1 = _gameState.AddRegion(_enemyName, 5);
            testRegion.AddNeighbor(neighbor1);

            var neighbor2 = _gameState.AddRegion(_enemyName, 5);
            testRegion.AddNeighbor(neighbor2);

            var neighbor3 = _gameState.AddRegion(Constants.NeutralPlayerName, 100);
            testRegion.AddNeighbor(neighbor3);

            var result = DefensiveArmiesNeeded.For(testRegion.Neighbors, testRegion.Armies, 5);

            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void For_AgainstImpossibleOdds_ReturnsIntMax()
        {
            var testRegion = _gameState.AddRegion(_myName, 300);

            var neighbor1 = _gameState.AddRegion(_enemyName, 1000);
            testRegion.AddNeighbor(neighbor1);

            var neighbor2 = _gameState.AddRegion(_enemyName, 1000);
            testRegion.AddNeighbor(neighbor2);

            var neighbor3 = _gameState.AddRegion(_enemyName, 1000);
            testRegion.AddNeighbor(neighbor3);

            var result = DefensiveArmiesNeeded.For(testRegion.Neighbors, testRegion.Armies, 2);

            Assert.AreEqual(int.MaxValue, result);
        }

        [TestMethod]
        public void For_LargeArmies_ReturnsIntMax()
        {
            var testRegion = _gameState.AddRegion(_myName, 198);

            var neighbor1 = _gameState.AddRegion(_enemyName, 85);
            testRegion.AddNeighbor(neighbor1);

            var neighbor2 = _gameState.AddRegion(_enemyName, 110);
            testRegion.AddNeighbor(neighbor2);

            var neighbor3 = _gameState.AddRegion(_enemyName, 98);
            testRegion.AddNeighbor(neighbor3);

            var neighbor4 = _gameState.AddRegion(_enemyName, 148);
            testRegion.AddNeighbor(neighbor4);

            var result = DefensiveArmiesNeeded.For(testRegion.Neighbors, testRegion.Armies, 5);

            Assert.AreEqual(int.MaxValue, result);
        }

        [TestMethod]
        public void For_AgainstLargeOdds_ReturnsIntMax()
        {
            var testRegion = _gameState.AddRegion(_myName, 50);

            var neighbor1 = _gameState.AddRegion(_enemyName, 100);
            testRegion.AddNeighbor(neighbor1);

            var neighbor2 = _gameState.AddRegion(_enemyName, 100);
            testRegion.AddNeighbor(neighbor2);

            var result = DefensiveArmiesNeeded.For(testRegion.Neighbors, testRegion.Armies, 2);

            Assert.AreEqual(int.MaxValue, result);
        }

        [TestMethod]
        public void RankRegionsByNeed_TwoRegions_ReturnsListSortedDesc()
        {
            var testRegion = _gameState.AddRegion(_myName, 6);

            var neighbor1 = _gameState.AddRegion(_enemyName, 5);
            testRegion.AddNeighbor(neighbor1);

            var neighbor2 = _gameState.AddRegion(_enemyName, 5);
            testRegion.AddNeighbor(neighbor2);

            var testRegion2 = _gameState.AddRegion(_myName, 6);

            var neighbor21 = _gameState.AddRegion(_enemyName, 1);
            testRegion2.AddNeighbor(neighbor21);

            var testRegion3 = _gameState.AddRegion(_myName, 6);

            var neighbor31 = _gameState.AddRegion(_enemyName, 6);
            testRegion3.AddNeighbor(neighbor31);

            var result = DefensiveArmiesNeeded.RankRegionsByNeed(_gameState.FullMap, _myName, 5);

            Assert.AreEqual(testRegion.Id, result[0].Item1.Id);
            Assert.AreEqual(testRegion3.Id, result[1].Item1.Id);
            Assert.AreEqual(testRegion2.Id, result[2].Item1.Id);
        }
    }
}
