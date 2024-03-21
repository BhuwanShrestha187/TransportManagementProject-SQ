﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportation_Management_System
{
    public class Route
    {
        public City Destination { set; get; }

        /// The total distance in KM
        public int Distance { set; get; }

        /// The time it takes from one citu to another
        public double Time { set; get; }

        /// The city that is located to the West of destination
        public City West { set; get; }

        /// "Pointer" to the city on west
        public Route WestPtr { set; get; }

        /// The city that is located to the East of destination
        public City East { set; get; }

        // "Pointer" to the city on East
        public Route EastPtr { set; get; }

        ///
        /// \brief This overloaded Rate class constructor is used to access a Route class with empty attributes.
        ///
        public Route()
        { }

        ///
        /// \brief This Rate class constructor is used to initialize Route class properties.
        ///
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
