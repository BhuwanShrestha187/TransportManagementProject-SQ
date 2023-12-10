using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportManagement
{
        public enum City
        {
            Null = -1,
            Windsor,
            London,
            Hamilton,
            Toronto,
            Oshawa,
            Belleville,
            Kingston,
            Ottawa,
        }
  
        public class Trip
        {
            /// The trip ID for the Trip
            public long TripID { get; set; }

            /// The order ID for the Trip
            public long OrderID { get; set; }

            /// The carrier ID for the Trip
            public long CarrierID { get; set; }

            /// The starting city for the transport
            public City OriginCity { get; set; }

            /// The destination city for the transport
            public City DestinationCity { get; set; }

            /// The total distance for the transport
            public int TotalDistance { get; set; }

            /// The total number of days needed for the trip
            public double TotalTime { get; set; }

            /// The JobType for the trip
            public JobType JobType { get; set; }

            /// The VanType for the trip
            public VanType VanType { get; set; }
        }
    }

