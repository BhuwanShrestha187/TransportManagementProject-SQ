using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportation_Management_System
{
    public enum JobType
    {
        FTL,
        LTL
    }

    public enum VanType
    {
        DryVan,
        Reefer
    }
    public class Contract
    {
        public string ClientName { get; set; }

        /// JobType mentioned in the contract
        public JobType JobType { get; set; }

        /// Quantity of product in contract
        public int Quantity { get; set; }

        /// Origin city of the contract
        public City Origin { get; set; }

        /// Destination city of the contract
        public City Destination { get; set; }

        /// VanType needed for the contract
        public VanType VanType { get; set; }
    }
}
