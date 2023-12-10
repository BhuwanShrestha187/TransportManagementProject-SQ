using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportManagement
{
    public  class Carrier
    {
        public int CarrierID { get; set; }
        public string Name { get; set; }
        public double FTLRate { get; set; }
        public double LTLRate { get; set; }
        public double ReeferCharge {  get; set; }
        public bool IsActive {  get; set; }

        public Carrier(string newName, double newFTL, double newLTL, double newReefer)
        {
            Name = newName;
            FTLRate = newFTL;
            LTLRate = newLTL;
            ReeferCharge = newReefer;
        }

        public Carrier() { }
    }
}
