using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Policy;
using MySql.Data.MySqlClient; 

namespace TransportManagement
{
    public enum UserRole
    {
        Buyer,
        Planner,
        Admin
    }
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
            LoadConnectionString();
        }

        public void UpdateDatabaseConnectionString(string fieldToChange, string newData)
        {
            List<string> availableFields = new List<string> { "Server", "User", "Port", "Password", "DatabaseName" };

            if (!availableFields.Contains(fieldToChange))
            {
                throw new KeyNotFoundException($"Error. We couldn't find any field called {fieldToChange} in the database connection string.");
            }

            string oldData = ConfigurationManager.AppSettings.Get(fieldToChange);

            // If the value doesn't change, don't do anything
            if (oldData == newData)
            {
                return;
            }

            // Update the config file
            var configMap = new ExeConfigurationFileMap { ExeConfigFilename = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

            config.AppSettings.Settings[fieldToChange].Value = newData;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            Logger.Log($"{fieldToChange} database connection string changed from {oldData} to {newData}", LogLevel.Information);

            LoadConnectionString();
        }
        private void LoadConnectionString()
        {
            try
            {
                Server = ConfigurationManager.AppSettings.Get("Server");
                User = ConfigurationManager.AppSettings.Get("User");
                Port = ConfigurationManager.AppSettings.Get("Port");
                Password = ConfigurationManager.AppSettings.Get("Password");
                DatabaseName = ConfigurationManager.AppSettings.Get("DatabaseName");

                if (Server == "" || User == "" || Port == "" || Password == "" || DatabaseName == "")
                {
                    throw new Exception("We couldn't retrieve the information about the database. Please check your config file.");
                }
            }

            catch (Exception ex) 
            {
                Logger.Log(ex.Message, LogLevel.Critical);
                throw; 
            }   
        }

       


        //Populate the connection string to establish connection to the database. 
        public string ConnectionString()
        {
            return $"server={Server};user={User};database={DatabaseName};port={Port};password={Password}; convert zero datetime=True";

        }

        /*
         * Check if username is valid in the database or not. 
         */
        public bool CheckUserName(string username)
        {
            bool existingUser = false;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
            {         
                conn.Open();
                Logger.Log("Connected to the SQL" ,LogLevel.Information);
                
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
                
            }
            return existingUser;
        }


        /*
         * Check password is valid or not. 
         */

        public bool CheckUserPassword(string username, string password)
        {
            bool isValid = false;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
            {
               
                    conn.Open();

                

                try
                {
                    string query = $"SELECT * FROM Users WHERE Username='{username}'"; //Query to send to the database
                    MySqlCommand cmd = new MySqlCommand(query, conn);                   //Command
                    MySqlDataReader rdr = cmd.ExecuteReader();                          // Reading data fromthe databaase

                    //If there is data in the database
                    if (rdr.HasRows)
                    {
                        while ((rdr.Read()))
                        {
                            string passwordDB = rdr["Password"].ToString();
                            if (password == passwordDB)
                            {
                                isValid = true;
                                
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
          
            }
            return isValid;
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
                 conn.Open();     
                try
                {
                    string query = $"SELECT UserType FROM Users WHERE Username='{username}'"; //Query to send to the database
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
               
            }
            return userType;
        }







    }
}
