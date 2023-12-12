using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportManagement
{ 
    public class Buyer
    {
        public List<Contract> GetContractsFromMarketPlaceDatabase()
        {
            List<Contract> list = new List<Contract>();
            ContractMarketplace marketplace = new ContractMarketplace();
            list = marketplace.GetContracts();
            return list;
        }
    }
}
