using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportation_Management_System
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
