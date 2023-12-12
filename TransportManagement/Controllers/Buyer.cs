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

        public void GenerateOrder(Contract contract)
        {
            Order order = new Order
            {
                ClientName = contract.ClientName,
                JobType = contract.JobType,
                Quantity = contract.Quantity,
                Origin = contract.Origin,
                Destination = contract.Destination,
                VanType = contract.VanType,
                // Add any other necessary properties for the order
            };
        }
    }
}
