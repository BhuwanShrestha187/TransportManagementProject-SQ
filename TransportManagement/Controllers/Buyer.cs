
using System;
using System.Collections.Generic;
using TransportManagement;

namespace TransportManagement
{
   
    public class Buyer
    {
        DAL dal = new DAL();
        public List<Contract> FetchContracts()
        {
            ContractMarketplace cmp = new ContractMarketplace();
            List<Contract> cons = cmp.GetContracts();
            return cons;
        }
        public Order GenerateOrder(Contract contract)
        {
            // Create an order object

            Order order = new Order(contract.ClientName, contract.Origin, contract.Destination, contract.JobType, contract.Quantity, contract.VanType);

            // Check if Client exists, If it doesn't exists, create it
            DAL db = new DAL();
            if (db.FilterClientByName(order.ClientName) == null)
            {
                Client client = new Client(order.ClientName);
                db.CreateClient(client);
            }

            // Insert order in db
            try
            {
                // Insert order in database
                db.CreateOrder(order);
            }
            catch (Exception)
            {
                throw;
            }

            return order;
        }
        public List<Order> GetOrders(int orderStatus)
        {
            List<Order> orderList = new List<Order>();

            

            if (orderStatus == 0)
            {
                orderList = dal.GetActiveOrders();
            }
            else if (orderStatus == 1)
            {
                orderList = dal.GetCompletedOrders();
            }
            else if (orderStatus == 2)
            {
                orderList = dal.GetAllOrders();
            }

            return orderList;
        }

        public List<Client> GetClientsFromDatabase(int activeOrNot)
        {
            List<Client> clientList; 
            // For active clients
            if(activeOrNot == 1)
            {
                clientList = dal.GetActiveClientsFromDatabase(); 
            }

            else
            {
                clientList = dal.GetClientsFromDatabase();
            }

            return clientList;
            
        }



      

    }
}