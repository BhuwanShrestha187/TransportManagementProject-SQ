using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportManagement
{
    public class Route
    {
        public City Destination { set; get; }

        public int Distance { set; get; }

        public double Time { set; get; }

        public City West { set; get; }

  
        public Route WestPtr { set; get; }

        public City East { set; get; }

   
        public Route EastPtr { set; get; }

        public Route()
        { }

   
        public Route(City newDestination, int newDistance, double newTime, City newWest, City newEast)
        {
            Destination = newDestination;
            Distance = newDistance;
            Time = newTime;
            West = newWest;
            East = newEast;
        }
    }
}
