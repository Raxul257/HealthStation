using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;
using System.Xml.Serialization;
using UnityEngine;

namespace LuxarPL.HealthStation
{
    public class Station
    {
        [XmlAttribute("Pay_for_heal")]
        public bool Pay;

        [XmlAttribute("Cost")]
        public decimal Cost;

        [XmlAttribute("X")]
        public float X;

        [XmlAttribute("Y")]
        public float Y;

        [XmlAttribute("Z")]
        public float Z;

        public Vector3 Position
        {
            get { return new Vector3(X, Y, Z); }
        }

        public Station(bool pay, float x, float y, float z, decimal cost = 0)
        {
            Pay = pay;
            Cost = cost;
            X = x;
            Y = y;
            Z = z;
        }

        public Station()
        {

        }
    }
}
