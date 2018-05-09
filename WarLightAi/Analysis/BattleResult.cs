using System;
using WarLightAi.Main;
using WarLightAi.Math;

namespace WarLightAi.Analysis
{
    public struct BattleResult
    {
        private readonly float confidence;
        private readonly int defenderArmies;
        private readonly int attackerArmies;

        public BattleResult(float confidence, int defenderArmies, int attackerArmies)
        {
            if (confidence <= 0.5 || confidence >= 1)
                throw new ArgumentOutOfRangeException("confidence", "The confidence must be between 0.5 and 1, exclusive.");

            confidence += ((1 - confidence) / 2); // this compensates for the fact that we only check in one direction at any given time

            this.confidence = confidence;
            this.defenderArmies = defenderArmies;
            this.attackerArmies = attackerArmies;
            _attackerRemainingArmiesLow = null;
            _attackerRemainingArmies = null;
            _attackerRemainingArmiesHigh = null;
            _defenderRemainingArmiesLow = null;
            _defenderRemainingArmies = null;
            _defenderRemainingArmiesHigh = null;
        }

        private int? _attackerRemainingArmiesLow;
        public int AttackerRemainingArmiesLow
        {
            get
            {
                if (_attackerRemainingArmiesLow == null)
                    _attackerRemainingArmiesLow = GetAttackerRemainingArmiesLow();
                return _attackerRemainingArmiesLow.Value;
            }
        }
        private int GetAttackerRemainingArmiesLow()
        {
            int expectedAttackerDeathsHigh = BinomialDistribution.SuccessesHighEstimate(defenderArmies, Constants.DefenderKillChance, confidence);
            return System.Math.Max(attackerArmies - expectedAttackerDeathsHigh, 0);
        }

        private int? _attackerRemainingArmies;
        public int AttackerRemainingArmies
        {
            get
            {
                if (_attackerRemainingArmies == null)
                    _attackerRemainingArmies = GetAttackerRemainingArmies();
                return _attackerRemainingArmies.Value;
            }
        }
        private int GetAttackerRemainingArmies()
        {
            int expectedAttackerDeaths = (int)System.Math.Floor(defenderArmies * Constants.DefenderKillChance);
            return System.Math.Max(attackerArmies - expectedAttackerDeaths, 0);
        }

        private int? _attackerRemainingArmiesHigh;
        public int AttackerRemainingArmiesHigh
        {
            get
            {
                if (_attackerRemainingArmiesHigh == null)
                    _attackerRemainingArmiesHigh = GetAttackerRemainingArmiesHigh();
                return _attackerRemainingArmiesHigh.Value;
            }
        }
        private int GetAttackerRemainingArmiesHigh()
        {
            int expectedAttackerDeathsLow = BinomialDistribution.SuccessesLowEstimate(defenderArmies, Constants.DefenderKillChance, confidence);
            return System.Math.Max(attackerArmies - expectedAttackerDeathsLow, 0);
        }
        
        private int? _defenderRemainingArmiesLow;
        public int DefenderRemainingArmiesLow
        {
            get
            {
                if (_defenderRemainingArmiesLow == null)
                    _defenderRemainingArmiesLow = GetDefenderRemainingArmiesLow();
                return _defenderRemainingArmiesLow.Value;
            }
        }
        private int GetDefenderRemainingArmiesLow()
        {
            int expectedDefenderDeathsHigh = BinomialDistribution.SuccessesHighEstimate(attackerArmies, Constants.AttackerKillChance, confidence);
            return System.Math.Max(defenderArmies - expectedDefenderDeathsHigh, 0);
        }
        
        private int? _defenderRemainingArmies;
        public int DefenderRemainingArmies
        {
            get
            {
                if (_defenderRemainingArmies == null)
                    _defenderRemainingArmies = GetDefenderRemainingArmies();
                return _defenderRemainingArmies.Value;
            }
        }
        private int GetDefenderRemainingArmies()
        {
            int expectedDefenderDeaths = (int)System.Math.Floor(attackerArmies * Constants.AttackerKillChance);
            return System.Math.Max(defenderArmies - expectedDefenderDeaths, 0);
        }
        
        private int? _defenderRemainingArmiesHigh;
        public int DefenderRemainingArmiesHigh
        {
            get
            {
                if (_defenderRemainingArmiesHigh == null)
                    _defenderRemainingArmiesHigh = GetDefenderRemainingArmiesHigh();
                return _defenderRemainingArmiesHigh.Value;
            }
        }
        private int GetDefenderRemainingArmiesHigh()
        {
            int expectedDefenderDeathsLow = BinomialDistribution.SuccessesLowEstimate(attackerArmies, Constants.AttackerKillChance, confidence);
            return System.Math.Max(defenderArmies - expectedDefenderDeathsLow, 0);
        }
        
        

        public bool AttackerExpectedToWin()
        {
            return (AttackerRemainingArmies >= 1 && DefenderRemainingArmies < 1);
        }

        public bool AttackerExpectedToWinAtAll()
        {
            return (AttackerRemainingArmiesHigh >= 1 && DefenderRemainingArmiesLow < 1);
        }

        public bool AttackerExpectedToWinForSure()
        {
            return (AttackerRemainingArmiesLow >= 1 && DefenderRemainingArmiesHigh < 1);
        }

        public bool DefenderExpectedToSurvive()
        {
            return !AttackerExpectedToWin();
        }

        public bool DefenderExpectedToSurviveAtAll()
        {
            return !AttackerExpectedToWinForSure();
        }

        public bool DefenderExpectedToSurviveForSure()
        {
            return !AttackerExpectedToWinAtAll();
        }
    }
}
