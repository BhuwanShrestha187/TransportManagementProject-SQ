using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportManagement
{
    public class Order
    {
        public int OrderID { get; set; }

        public string ClientName { get; set; }


        public DateTime OrderCreationDate { get; set; }
        public DateTime OrderAcceptedDate { get; set; }
        public DateTime OrderCompletionDate { get; set; }
        public int IsCompleted { get; set; }

        public City Origin { get; set; }
        public City Destination { get; set; }

        public JobType JobType { get; set; }
        public VanType VanType { get; set; }
        public int Quantity { get; set; }
        public int InvoiceGenerated { get; set; }
        public Order(string clientName, City origin, City destination, JobType jobType, int quantity, VanType vanType)
        {
            ClientName = clientName;
            Origin = origin;
            Destination = destination;
            JobType = jobType;
            Quantity = quantity;
            VanType = vanType;
            IsCompleted = 0;
            InvoiceGenerated = 0;
        }

       
        public Order()
        { }
    }
}

