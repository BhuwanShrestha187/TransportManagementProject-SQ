/* -- FILEHEADER COMMENT --
    FILE		:	Buyer.cs
    PROJECT		:	Transportation Management System
    PROGRAMMER	:  * Ana De Oliveira
                   * Icaro Ryan Oliveira Souza
                   * Lazir Pascual
                   * Rohullah Noory
    DATE		:	2021-12-07
    DESCRIPTION	:	This file contains the source for the Buyer class which inherits from User class.

*/

using System;
using System.Collections.Generic;
using TransportManagement;

namespace TransportManagement
{
    ///
    /// \class Buyer
    ///
    /// \brief The purpose of this class is to represent the Buyer User
    ///
    /// This class represents the role of a buyer User, which represents a User
    /// who requesting Customer contracts from the Contract Marketplace and generating
    /// an initial Order or contract
    ///
    /// \author <i>Team Blank</i>
    ///
    public class Buyer
    {
        ///
        /// \brief Fetch all contracts from the contract marketplace
        ///
        /// \return A list of all contracts from the marketplace
        ///
        public List<Contract> FetchContracts()
        {
            ContractMarketplace cmp = new ContractMarketplace();
            List<Contract> cons = cmp.GetContracts();
            return cons;
        }

        ///
        /// \brief Create order based on the contract fetched from the marketplace
        ///
        /// \param contract  - <b>Contract</b> - The selected contract for the order to be created
        ///
        /// \return Order object
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

        ///
        /// \brief Return a list of active, completed or all orders in our system.
        ///
        /// \param orderStatus  - <b>int</b> - 0 for active, 1 for completed, 2 for all orders.
        ///
        /// \return A list of orders.
        ///
        public List<Order> GetOrders(int orderStatus)
        {
            List<Order> orderList = new List<Order>();

            DAL db = new DAL();

            if (orderStatus == 0)
            {
                orderList = db.GetActiveOrders();
            }
            else if (orderStatus == 1)
            {
                orderList = db.GetCompletedOrders();
            }
            else if (orderStatus == 2)
            {
                orderList = db.GetAllOrders();
            }

            return orderList;
        }

        ///
        /// \brief Inserts a new invoice in the Invoice table
        ///
        /// \param orderObj  - <b>Order</b> - An Order object with all its information
        /// \return invoice -   <b>Invoice</b>  -   An Invoice object with all its information
        ///
        /// \return An invoice object
       
    }
}