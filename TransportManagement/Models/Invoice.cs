/*
 * Filename: DAL.cs
 * Project: Transport Management System
 * Date: December 15, 2023
 * Author: Bhuwan Shrestha, Ahmed Alemleh, Smaran Adhikari
 * Description: It serves as a model for the role for generating invoice..
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportManagement
{
    public class Invoice
    {
        public long OrderID { set; get; }
        public string ClientName { set; get; }
        public decimal TotalAmount { set; get; }
        public int PalletQuantity { set; get; }
    
        public City Origin { set; get; }
        public City Destination { set; get; }
        public int TotalKM { set; get; }
        public double Days { set; get; }
        public DateTime CompletedDate { get; set; }
    }
}
