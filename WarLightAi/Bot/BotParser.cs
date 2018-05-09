using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WarLightAi.Move;

namespace WarLightAi.Bot
{

    public class BotParser
    {

        readonly IBot bot;

        GameState currentState;

        public BotParser(IBot bot)
        {
            IncreaseInputBufferSize(1024);

            this.bot = bot;
            this.currentState = new GameState();
        }

        private static void IncreaseInputBufferSize(int bufferSize)
        {
            byte[] inputBuffer = new byte[bufferSize];
            Stream inputStream = Console.OpenStandardInput(inputBuffer.Length);
            Console.SetIn(new StreamReader(inputStream, Console.InputEncoding, false, inputBuffer.Length));
        }

        public void Run()
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line == null)
                    break;
                line = line.Trim();

                if (line.Length == 0)
                    continue;

                String[] parts = line.Split(' ');
                if (parts[0] == "pick_starting_regions")
                {
                    // Pick which regions you want to start with
                    currentState.SetPickableStartingRegions(parts);
                    var preferredStartingRegions = bot.GetPreferredStartingRegions(currentState, long.Parse(parts[1]));
                    var output = new StringBuilder();
                    foreach (var region in preferredStartingRegions)
                        output.Append(region.Id + " ");

                    Console.WriteLine(output);
                }
                else if (parts.Length == 3 && parts[0] == "go")
                {
                    // We need to do a move
                    var output = new StringBuilder();
                    if (parts[1] == "place_armies")
                    {
                        // Place armies
                        List<PlaceArmiesMove> placeArmiesMoves = bot.GetPlaceArmiesMoves(currentState, long.Parse(parts[2]));
                        foreach (var move in placeArmiesMoves)
                        {
                            move.Commit();
                            output.Append(move.String + ",");
                        }
                    }
                    else if (parts[1] == "attack/transfer")
                    {
                        // attack/transfer
                        var attackTransferMoves = bot.GetAttackTransferMoves(currentState, long.Parse(parts[2]));
                        foreach (var move in attackTransferMoves)
                            output.Append(move.String + ",");
                    }
                    if (output.Length > 0)
                        Console.WriteLine(output);
                    else
                        Console.WriteLine("No moves");
                }
                else if (parts.Length == 3 && parts[0] == "settings")
                {
                    // Update settings
                    currentState.UpdateSettings(parts[1], parts[2]);
                }
                else if (parts[0] == "setup_map")
                {
                    // Initial full map is given
                    currentState.SetupMap(parts);
                }
                else if (parts[0] == "update_map")
                {
                    // All visible regions are given
                    currentState.UpdateMap(parts);
                }
                else if (parts[0] == "opponent_moves")
                {
                    // All visible opponent moves are given
                    currentState.ReadOpponentMoves(parts);
                }
                else
                {
                    Console.Error.WriteLine("Unable to parse line \"" + line + "\"");
                }
            }
        }

    }

}