using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transportation_Management_System
{
    public class Clients
    {
        public int ClientID { get; set; }

        public string ClientName { get; set; }
        public int IsActive { get; set; }

        public Clients(string name)
        {
            ClientName = name;
        }

        ///
        /// \brief This Client class constructor is used to access a client with empty properties.
        ///
        public Clients()
        { }
    }
}
