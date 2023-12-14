using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Filename: CarrierCity.cs
 * Project: Transport Management System
 * Date: December 15, 2023
 * Author: Bhuwan Shrestha, Ahmed Alemleh, Smaran Adhikari
 * Description: It serves as a controller for the role of Buyer.
 */
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
