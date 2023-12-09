using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Policy;
using MySql.Data.MySqlClient; 

namespace TransportManagement.Models
{
    public class DAL
    {
        //The main purpose of the DAL is to play with the database. So declare the interaction tools at first
        private String Server {  get; set; } //Server of the database
        private string User {  get; set; }  //Username to connect to
        private string Port { get; set; }   //Port to connect to the databse
        private string Password { get; set; }   //Password to connect to the database
        private string DatabaseName { get; set; }   //Database name to connect to the database

        public DAL() 
        {
            
        }

        private void LoadConnectionString()
        {
            try
            {
                Server = ConfigurationManager.AppSettings["Server"];
                User = ConfigurationManager.AppSettings["User"];
                Port = ConfigurationManager.AppSettings["Port"];
                Password = ConfigurationManager.AppSettings["Password"];
                DatabaseName = ConfigurationManager.AppSettings["DatabaseName"];
                
                if(Server == null || User == null || Port == null || Password == null || DatabaseName == null) 
                {
                    throw new Exception("Could not retrieve the information about the database for establishing connection!!"); 
                }
            }

            catch (Exception ex) 
            {
                Logger.Log(ex.Message, LogLevel.Fatal);
                throw; 
            }   
        }

        //Populate the connection string to establish connection to the database. 
        public string ConnectionString()
        {
            string connectionString = $"server={Server};user={User}; port={Port};password={Password};database={DatabaseName}";
            return connectionString;
        }

        /*
         * Check if username is valid in the database or not. 
         */
        public bool CheckUserName(string username)
        {
            bool existingUser = false;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
            {
                if (conn == null)
                {
                    conn.Open();
                }
                try
                {
                    string query = $"SELECT * FROM Users WHERE Username='{username}'"; //Query to send to the database
                    MySqlCommand cmd = new MySqlCommand(query, conn);                   //Command
                    MySqlDataReader rdr = cmd.ExecuteReader();                          // Reading data fromthe databaase

                    //If there is data in the database
                    if(rdr.HasRows)
                    {
                        while(rdr.Read())
                        {
                            string usernameDB = rdr["Username"].ToString();
                            if(username == usernameDB)
                            {
                                existingUser = true;
                                break; 
                            }
                        }
                    }
                }

                catch(Exception ex)
                {
                    Logger.Log(ex.Message, LogLevel.Error);
                    throw; 
                }

                conn.Close(); 
                return existingUser;
            }
            
        }


        /*
         * Check password is valid or not. 
         */

        public bool CheckUserPassword(string password)
        {
            bool isValid = false;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
            {
                if (conn == null)
                {
                    conn.Open();
                }
                try
                {
                    string query = $"SELECT * FROM Users WHERE Username='{password}'"; //Query to send to the database
                    MySqlCommand cmd = new MySqlCommand(query, conn);                   //Command
                    MySqlDataReader rdr = cmd.ExecuteReader();                          // Reading data fromthe databaase

                    //If there is data in the database
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            string passwordDB = rdr["Password"].ToString();
                            if (password == passwordDB)
                            {
                                isValid = true;
                                break;
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    Logger.Log(ex.Message, LogLevel.Error);
                    throw;
                }

                conn.Close();
                return isValid;
            }
        }


        /*
         *  After verifying the credentials, the new window should be opened if it is Admin, Buyer or Planner.
         *  So they are stored as UserType the database. UserType is stored as 0 1 or 2. 0-> buyer, 1-> Planner, 2-> Admin
         */

        public  string GetUserType(string username)
        {
            string userType = null; 
            using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
            {
                if (conn == null)
                {
                    conn.Open();
                }
                try
                {
                    string query = $"SELECT * FROM Users WHERE Username='{username}'"; //Query to send to the database
                    MySqlCommand cmd = new MySqlCommand(query, conn);                   //Command
                    MySqlDataReader rdr = cmd.ExecuteReader();                          // Reading data fromthe databaase

                    //If there is data in the database
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            int userTypeID = Convert.ToInt32(rdr["UserType"].ToString());        
                            switch(userTypeID)
                            {
                                case 0:
                                    userType = "Buyer";
                                    break;

                                case 1:
                                    userType = "Planner";
                                    break;

                                case 2:
                                    userType = "Admin";
                                    break;

                                default: break;            
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    Logger.Log(ex.Message, LogLevel.Error);
                    throw;
                }

                conn.Close();
                return userType;
            }
        }







    }
}
