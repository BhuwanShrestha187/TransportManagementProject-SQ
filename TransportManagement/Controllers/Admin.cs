using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportManagement
{
    public class Admin
    {
        DAL dal = new DAL();
       
        
        /*
         * Passes the fields to be changed to the DAL to change the Database
         */
        public void UpdateConnectionString(string fieldToChange, string newData)
        {
            dal.UpdateDatabaseConnectionString(fieldToChange, newData);
        }

        /*
         * Update LogPath 
         */

        public int UpdateLogPath(string newPath)
        {
           int result = Logger.ChangeLogDirectory(newPath);

            if (result == 0)
            {
                return 0;  //Successfully changed 
            }

            else
            {
                return 1; //False
            }
        }


        //*************************** Working related to the rate field **************************************

        public double GetFTLRate()
        {
            return dal.GetCurrentFTLRate();
        }

        public double GetLTLRate()
        {
            return dal.GetCurrentLTLRate();
        }
        public bool UpdateFTLRate(double newFTLRate)
        {
            return dal.UpdateFTLRate(newFTLRate);
        }

        public bool UpdateLTLRate(double newFTLRate)
        {
            return dal.UpdateLTLRate(newFTLRate);
        }
    }
}
