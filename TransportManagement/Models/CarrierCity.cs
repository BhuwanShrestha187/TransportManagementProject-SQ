using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportManagement
{
    public class CarrierCity
    {
        public Carrier Carrier { set; get; }

        /// The city of the carrier company
        public City DepotCity { set; get; }

        /// The number of full truckloads available
        public int FTLAval { set; get; }

        /// The number of less than truckloads available
        public int LTLAval { set; get; }

        ///
        /// \brief This overloaded CarrierCity class constructor is used to access a carrier city with empty attributes.
        ///
        public CarrierCity()
        { }

        ///
        /// \brief This CarrierCity class constructor is used to initialize the properties of the carrier city.
        ///
        public CarrierCity(Carrier newCarrier, City newDepot, int newFTL, int newLTL)
        {
            Carrier = newCarrier;
            DepotCity = newDepot;
            FTLAval = newFTL;
            LTLAval = newLTL;
        }
    }
}
