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

       
        public City DepotCity { set; get; }

        public int FTLAval { set; get; }

        public int LTLAval { set; get; }

   
        public CarrierCity()
        { }

        public CarrierCity(Carrier newCarrier, City newDepot, int newFTL, int newLTL)
        {
            Carrier = newCarrier;
            DepotCity = newDepot;
            FTLAval = newFTL;
            LTLAval = newLTL;
        }
    }
}
