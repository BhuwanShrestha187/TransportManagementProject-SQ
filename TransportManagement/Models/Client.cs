/*
 * Filename: Buyer.cs
 * Project: Transport Management System
 * Date: December 15, 2023
 * Author: Bhuwan Shrestha, Ahmed Alemleh, Smaran Adhikari
 * Description: It serves as a controller for the role of Clients.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace TransportManagement
{
    public class Client
    {
        public int ClientID {  get; set; }
        public string ClientName {  get; set; }
        public int IsActive {  get; set; }

        public Client(string name) 
        {
            ClientName = name; 
        }

        public Client() { }

    }
}
