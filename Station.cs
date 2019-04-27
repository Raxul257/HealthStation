using System.Xml.Serialization;
using UnityEngine;

namespace Raxul257.HealthStation
{
    public class Station
    {
        [XmlAttribute]
        public bool Paid;

        [XmlAttribute]
        public decimal Cost;

        [XmlAttribute]
        public float X;
        [XmlAttribute]
        public float Y;
        [XmlAttribute]
        public float Z;

        public Vector3 Position => new Vector3(X, Y, Z);

        public Station(Vector3 postion, bool paid = false, decimal cost = 0)
        {
            Paid = paid;
            Cost = cost;
            X = postion.x;
            Y = postion.y;
            Z = postion.z;
        }

        public Station()
        {
        }
    }
}
