using System.Collections.Generic;
using WarLightAi.Main;
using WarLightAi.Move;

namespace WarLightAi.Bot
{

    public interface IBot
    {

        List<Region> GetPreferredStartingRegions(GameState state, long timeOut);

        List<PlaceArmiesMove> GetPlaceArmiesMoves(GameState state, long timeOut);

        List<AttackTransferMove> GetAttackTransferMoves(GameState state, long timeOut);

    }

}