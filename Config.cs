using System.Collections.Generic;
using Rocket.API;

namespace Raxul257.HealthStation
{
    public class Config : IRocketPluginConfiguration
    {
        public ulong HealthStationRange;
        public bool UseUconomy;
        public List<Station> HealthStations;

        public void LoadDefaults()
        {
            HealthStationRange = 5;
            UseUconomy = true;
            HealthStations = new List<Station> { };
        }
    }
}
