using System;
using System.Collections.Generic;

namespace WarLightAi.Main
{



    public class Region
    {
        public int Id { get; private set; }
        public String PlayerName { get; set; }
        public List<Region> Neighbors { get; private set; }
        public SuperRegion SuperRegion { get; private set; }
        public int Armies { get; set; }
        public int? IsolationLevel { get; set; }
        public int StrategicValue { get; set; }

        public Region(int id, SuperRegion superRegion)
        {
            this.Id = id;
            this.SuperRegion = superRegion;
            this.Neighbors = new List<Region>();
            this.PlayerName = Constants.UnknownPlayerName;
            this.Armies = 0;

            superRegion.AddSubRegion(this);
        }

        public Region(int id, SuperRegion superRegion, String playerName, int armies)
        {
            this.Id = id;
            this.SuperRegion = superRegion;
            this.Neighbors = new List<Region>();
            this.PlayerName = playerName;
            this.Armies = armies;

            superRegion.AddSubRegion(this);
        }

        public void AddNeighbor(Region neighbor)
        {
            if (!Neighbors.Contains(neighbor))
            {
                Neighbors.Add(neighbor);
                neighbor.AddNeighbor(this);
            }
        }

        /**
         * @param region a Region object
         * @return True if this Region is a neighbor of given Region, false otherwise
         */
        public bool IsNeighbor(Region region)
        {
            if (Neighbors.Contains(region))
                return true;
            return false;
        }

        /**
         * @param playerName A string with a player's name
         * @return True if this region is owned by given playerName, false otherwise
         */
        public bool OwnedByPlayer(String playerName)
        {
            if (playerName == this.PlayerName)
                return true;
            return false;
        }
    }

}