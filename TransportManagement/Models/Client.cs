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
