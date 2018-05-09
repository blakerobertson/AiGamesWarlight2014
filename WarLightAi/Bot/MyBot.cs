using System;
using System.Collections.Generic;
using WarLightAi.Analysis;
using WarLightAi.Decisions;
using WarLightAi.Main;
using WarLightAi.Move;

namespace WarLightAi.Bot
{

    /**
     * This is a simple bot that does random (but correct) moves.
     * This class implements the Bot interface and overrides its Move methods.
     * You can implements these methods yourself very easily now,
     * since you can retrieve all information about the match from variable “state”.
     * When the bot decided on the move to make, it returns a List of Moves. 
     * The bot is started by creating a Parser to which you Add
     * a new instance of your bot, and then the parser is started.
     */

    public class MyBot : IBot
    {
        public static void Main(String[] args)
        {
            var parser = new BotParser(new MyBot());
            parser.Run();
        }

        /**
         * A method used at the start of the game to decide which player start with what Regions. 6 Regions are required to be returned.
         * @return : a list of m (m=6) Regions starting with the most preferred Region and ending with the least preferred Region to start with 
         */
        public List<Region> GetPreferredStartingRegions(GameState state, long timeOut)
        {
            StrategicMap.AnalyzeMap(state);
            var pick = new PickTopStartingRegions();
            return pick.From(state.PickableStartingRegions, state.FullMap);
        }

        /**
         * This method is called for at first part of each round.
         * @return The list of PlaceArmiesMoves for one round
         */
        public List<PlaceArmiesMove> GetPlaceArmiesMoves(GameState state, long timeOut)
        {
            StrategicMap.AnalyzeMap(state);
            var pick = new PickArmyPlacements();
            return pick.BasedOn(state);
        }

        
        /**
         * This method is called for at the second part of each round.
         * @return The list of PlaceArmiesMoves for one round
         */
        public List<AttackTransferMove> GetAttackTransferMoves(GameState state, long timeOut)
        {
            StrategicMap.AnalyzeMap(state);
            var pick = new PickArmyMovements();
            return pick.BasedOn(state);
        }
    }
}