using System;
using WarLightAi.Main;

namespace WarLightAi.Move
{

	/**
	 * This Move is used in the second part of each round. It represents the attack or transfer of armies from
	 * fromRegion to toRegion. If toRegion is owned by the player himself, it's a transfer. If toRegion is
	 * owned by the opponent, this Move is an attack. 
	 */

	public class AttackTransferMove : Move {
		
		private Region fromRegion;
		private Region toRegion;
		private int armies;
		
		public AttackTransferMove(String playerName, Region fromRegion, Region toRegion, int armies)
		{
			base.PlayerName = playerName;
			this.fromRegion = fromRegion;
			this.toRegion = toRegion;
			this.armies = armies;
		}

		public int Armies
		{
			set { armies = value; }
			get { return armies; }
		}

		public Region FromRegion
		{
			get { return fromRegion; }
		}

		public Region ToRegion
		{
			get { return toRegion; }
		}

		public String String
		{
			get { 
				if(string.IsNullOrEmpty(base.IllegalMove))
					return base.PlayerName + " attack/transfer " + fromRegion.Id + " " + toRegion.Id + " " + armies;
				else
					return base.PlayerName + " illegal_move " + base.IllegalMove; 
			}
		}

	}
}