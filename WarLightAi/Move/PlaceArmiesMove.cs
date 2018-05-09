using System;
using WarLightAi.Main;

namespace WarLightAi.Move
{

/**
 * This Move is used in the first part of each round. It represents what Region is increased
 * with how many armies.
 */

	public class PlaceArmiesMove : Move {
		
		private Region region;
		private int armies;
		
		public PlaceArmiesMove(string playerName, Region region, int armies)
		{
			base.PlayerName = playerName;
			this.region = region;
			this.armies = armies;
		}

		public int Armies
		{
			set { armies = value; }
			get { return armies; }
		}

		public Region Region
		{
			get { return region; }
		}

        /// <summary>
        /// This is called by the BotParser to actually send our placement to the engine
        /// </summary>
		public String String
		{
            get
            {
                if (string.IsNullOrEmpty(base.IllegalMove))
                    return base.PlayerName + " place_armies " + region.Id + " " + armies;
                else
                    return base.PlayerName + " illegal_move " + base.IllegalMove;
            }
		}

	    public void Commit()
	    {
            // this part is necessary because the engine doesn't update our state again until after all attack/transfers happen
	        region.Armies += this.armies; 
	    }
	}
}