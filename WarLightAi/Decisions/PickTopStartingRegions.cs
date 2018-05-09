using System;
using System.Collections.Generic;
using System.Linq;
using WarLightAi.Analysis;
using WarLightAi.Main;

namespace WarLightAi.Decisions
{
    public class PickTopStartingRegions
    {
        private const int numberToPick = 6;

        public List<Region> From(List<Region> availableChoices, Map fullMap)
        {
            var chosen = new List<Region>(numberToPick);

            Dictionary<int, List<Region>> choicesBySuperRegion = OrganizeRegionsBySuperRegion(availableChoices);
            
            var superRegionPrefs = StrategicMap.SuperRegionsByValue;
            int superRegionIndex = 0;
            int superRegionId = superRegionPrefs[superRegionIndex].Id;

            for (var i = 0; i < numberToPick; i++)
            {
                while (choicesBySuperRegion[superRegionId].Count == 0)
                {
                    superRegionIndex++;
                    superRegionId = superRegionPrefs[superRegionIndex].Id;
                }

                var chosenRegion = choicesBySuperRegion[superRegionId].First();
                chosen.Add(chosenRegion);
                choicesBySuperRegion[superRegionId].Remove(chosenRegion);
            }

            return chosen;
        }

        private Dictionary<int, List<Region>> OrganizeRegionsBySuperRegion(IEnumerable<Region> regions)
        {
            var organizedRegions = new Dictionary<int, List<Region>>();

            foreach (var region in regions)
            {
                if (!organizedRegions.ContainsKey(region.SuperRegion.Id))
                    organizedRegions.Add(region.SuperRegion.Id, new List<Region>());
               organizedRegions[region.SuperRegion.Id].Add(region);
            }

            return organizedRegions;
        }
    }
}
