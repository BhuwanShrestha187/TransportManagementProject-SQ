using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportation_Management_System
{
    internal class Planner : User
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

        ///
        /// \brief Determine whether an order has been assigned to a carrier or not
        ///
        /// \param currentOrder  - <b>Order</b> - selected order
        ///
        /// \return Returns true if carrier has been assigned, else false
        ///
        public bool CarrierAssigned(Order currentOrder)
        {
            DAL db = new DAL();
            return db.IsCarriedAssigned(currentOrder);
        }

        ///
        /// \brief Used to select carriers from targeted cities to complete an Order. This
        /// adds a "trip" to the order for each carrier selected
        ///
        /// \param order  - <b>Order</b> - Order to select the invoic
        /// \param currentCarrier  - <b>Carrier</b> - Selected carrier
        ///
        ///
        /// \return Returns void
        ///
        public void SelectOrderCarrier(Order currentOrder, int carrierID)
        {
            DAL db = new DAL();
            Trip trip = new Trip();
            TripManager tm = new TripManager();
            db.StartOrder(currentOrder);
            trip.CarrierID = carrierID;
            trip.OrderID = currentOrder.OrderID;
            trip.OriginCity = currentOrder.Origin;
            trip.DestinationCity = currentOrder.Destination;
            trip.JobType = currentOrder.JobType;
            trip.VanType = currentOrder.VanType;
            tm.CalculateDistanceAndTime(trip);
            db.CreateTrip(trip);
        }

        ///
        /// \brief Used to confirm and finalize an order. Completed orders are marked for follow up from the buyer.
        ///
        /// \return Returns void
        ///
        public void CompleteOrder(Order order)
        {
            DAL db = new DAL();
            db.CompleteOrder(order);
        }

        ///
        /// \brief Used to get the total time of a trip based on an order
        ///
        /// \return totalTime -  double
        ///
        public double GetTotalTime(Order order)
        {
            DAL db = new DAL();
            double totalTime = db.GetTotalTimeFromTrip(order);
            return totalTime;
        }

        ///
        /// \brief Used to get all carriers with the specified origin city and job type.
        ///
        /// \param originCity - <b>string</b> - origin city of the carrier.
        /// \param jobType - <b>JobType</b> - job type of the carrier.
        ///
        /// \return List of CarrierCity objects
        ///
        public List<CarrierCity> GetCarriers(string originCity, JobType jobType)
        {
            DAL db = new DAL();
            List<CarrierCity> carriersCities = db.FilterCarriersByCity((City)Enum.Parse(typeof(City), originCity, true), Convert.ToInt32(jobType));
            return carriersCities;
        }

        ///
        /// \brief Used to display a list of invoices based on time period
        ///
        /// \param timeperiod  - <b>bool</b> - true = past 2 weeks only, false = all time.
        ///
        /// \return Returns summmary report of invoice data
        ///
        public List<Invoice> GenerateSummaryReport(bool timePeriod)
        {
            DAL db = new DAL();

            List<Invoice> invoices = db.FilterInvoicesByTime(timePeriod);

            return invoices;
        }
    }
}
