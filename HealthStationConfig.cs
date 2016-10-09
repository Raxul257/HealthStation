using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;
using System.Xml.Serialization;

namespace LuxarPL.HealthStation
{
    public class HealthStationConfig : IRocketPluginConfiguration
    {
        public ulong StationRange;
        public double Cooldown;
        public bool UseUconomy;

        [XmlArrayItem("HealthStations")]
        [XmlArray(ElementName = "HealthStation")]
        public List<Station> stations;

        public void LoadDefaults()
        {
            StationRange = 5;
            Cooldown = 120;
            UseUconomy = true;
            stations = new List<Station> { };
        }
    }
}
