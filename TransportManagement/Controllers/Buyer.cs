using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportManagement
{ 
    public class Buyer
    {
        DAL dal = new DAL();
        public List<Contract> GetContractsFromMarketPlaceDatabase()
        {
            List<Contract> list = new List<Contract>();
            ContractMarketplace marketplace = new ContractMarketplace();
            list = marketplace.GetContracts();
            return list;
        }

        public Order GenerateOrder(Contract contract)
        {
            // Create an order object

            Order order = new Order(contract.ClientName, contract.Origin, contract.Destination, contract.JobType, contract.Quantity, contract.VanType);

            // Check if Client exists, If it doesn't exists, create it
            DAL db = new DAL();
            if (db.GetClientByName(order.ClientName) == null)
            {
                Client client = new Client(order.ClientName);
                db.CreateClient(client);
            }

            // Insert order in db
            try
            {
                // Insert order in database
                db.SaveOrderToDatabase(order);
            }
            catch (Exception)
            {
                throw;
            }

            return order;
        }
        public void GetOrdersFromDatabase(string quantity)
        {
            //if (quantity == "All")
            //{
            //    dal.GetAllOrders();
            //}

            if (quantity == "Active")
            {
                dal.GetAllActiveOrders();
            }

            //if (quantity == "Completed")
            //{
            //    dal.GetCompletedOrders();
            //}
        }
    }
}
