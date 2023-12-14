/*
 * Filename: Buyer.cs
 * Project: Transport Management System
 * Date: December 15, 2023
 * Author: Bhuwan Shrestha, Ahmed Alemleh, Smaran Adhikari
 * Description: It serves as a controller for the role of Planner.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportManagement
{
    public class Planner
    {
        public List<Order> FetchOrders(int orderStatus)
        {
            List<Order> orderList;

            DAL db = new DAL();

            if (orderStatus == 0)
            {
                orderList = db.GetActiveOrders();
            }
            else if (orderStatus == 1)
            {
                orderList = db.GetCompletedOrders();
            }
            else
            {
                orderList = db.GetAllOrders();
            }

            return orderList;
        }
    }

}
