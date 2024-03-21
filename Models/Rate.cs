using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportation_Management_System
{
    public enum RateType
    {
        FTL,
        LTL
    }
    public class Rate
    {
        public Dictionary<RateType, double> RateValuePair { get; set; }

        ///
        /// \brief This overloaded Rate class constructor is used to access a Rate class with empty attributes.
        ///
        public Rate()
        { }

        ///
        /// \brief This Rate class constructor is used to add values to the dictionary as key value pairs.
        ///
        public Rate(RateType newType, double newValue)
        {
            RateValuePair.Add(newType, newValue);
        }
    }
}
