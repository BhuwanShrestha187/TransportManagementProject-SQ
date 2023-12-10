using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Policy;
using MySql.Data.MySqlClient;
using System.Linq.Expressions;
using System.Windows.Media.Media3D;

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
        public void LoadConnectionString()
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


        //******************************* Working with the Rates ******************************************
        public double GetCurrentFTLRate()
        {
            double ftlRate = 0.0;
            string query = $"SELECT FTLRate FROM Rates";
            using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
            {
                conn.Open();
                try
                {
                    using(MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        MySqlDataReader rdr = cmd.ExecuteReader(); 
                        if(rdr.HasRows)
                        {
                            while(rdr.Read())
                            {
                                ftlRate = Convert.ToDouble(rdr["FTLRate"]); 
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    Logger.Log("Cannot retrieve the FTLRate value: " + ex.Message, LogLevel.Error);
                    throw;
                }
                

            }

            return ftlRate;
        }

        public double GetCurrentLTLRate()
        {
            double ltlRate = 0.0;
            string query = $"SELECT LTLRate FROM Rates";
            using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
            {
                conn.Open();
                try
                {
                   
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                ltlRate = Convert.ToDouble(rdr["LTLRate"]);
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    Logger.Log("Cannot retrieve the LTLRate value: " + ex.Message, LogLevel.Error);
                    throw;
                }
                

            }

            return ltlRate;
        }


        /*
         * Updates the rates in the database. 
         */

        public bool UpdateFTLRate(double newFTLRate)
        {
            string query = "UPDATE Rates SET FTLRate=@FTLRate";
            bool result = false; 
            using(MySqlConnection conn = new MySqlConnection(ConnectionString()))
            {
                conn.Open();
                using(MySqlCommand cmd = new MySqlCommand( query, conn))
                {
                    //Add the parameters to the command
                    try
                    {
                        cmd.Parameters.AddWithValue("@FTLRate", newFTLRate);
                        cmd.ExecuteNonQuery();
                        result = true; 
                    }

                    catch(Exception ex)
                    {
                        Logger.Log("Unable to update the FTLRates in the rate table." + ex.Message , LogLevel.Error);
                        throw; 
                    }
                }
            }
            return result;
        }

        public bool UpdateLTLRate(double newLTLRate)
        {
            string query = "UPDATE Rates SET LTLRate=@LTLRate";
            bool result = false;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    //Add the parameters to the command
                    try
                    {
                        cmd.Parameters.AddWithValue("@LTLRate", newLTLRate);
                        cmd.ExecuteNonQuery();
                        result = true;
                    }

                    catch (Exception ex)
                    {
                        Logger.Log("Unable to update the LTLRates in the rate table." + ex.Message, LogLevel.Error);
                        throw;
                    }
                }
            }
            return result;
        }



        //********************  Filter Cities By Carrier ******************
        public List<CarrierCity> FilterCitiesByCarrier(string carrierName)
        {
            List<CarrierCity> carrierCities = new List<CarrierCity>();
            string qSQL = "SELECT * FROM CarrierCity INNER JOIN Carriers ON CarrierCity.CarrierID = Carriers.CarrierID WHERE CarrierName=@CarrierName AND IsActive=1";

            try
            {
                string conString = ConnectionString();
                using (MySqlConnection conn = new MySqlConnection(conString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qSQL, conn))
                    {
                        cmd.Parameters.AddWithValue("@CarrierName", carrierName);

                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                // Getting the carrier of that city
                                Carrier carr = new Carrier
                                {
                                    CarrierID = int.Parse(rdr["CarrierID"].ToString()),
                                    Name = rdr["CarrierName"].ToString(),
                                    FTLRate = double.Parse(rdr["FTLRate"].ToString()),
                                    LTLRate = double.Parse(rdr["LTLRate"].ToString()),
                                    ReeferCharge = double.Parse(rdr["reefCharge"].ToString())
                                };

                                CarrierCity carrCity = new CarrierCity
                                {
                                    Carrier = carr,
                                    DepotCity = (City)Enum.Parse(typeof(City), rdr["DepotCity"].ToString(), true),
                                    FTLAval = int.Parse(rdr["FTLAval"].ToString()),
                                    LTLAval = int.Parse(rdr["LTLAval"].ToString())
                                };

                                carrierCities.Add(carrCity);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw;
            }
            return carrierCities;
        }

        //Get all carriers 
        public List<Carrier> GetAllCarriers()
        {
            string qSQL = "SELECT * FROM Carriers WHERE IsActive = 1";
            List<Carrier> carriers = new List<Carrier>();

            try
            {
                string conString = ConnectionString();

                using (MySqlConnection conn = new MySqlConnection(conString))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(qSQL, conn))
                    {
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                Carrier carr = new Carrier
                                {
                                    CarrierID = rdr.GetInt32("CarrierID"),
                                    Name = rdr.GetString("CarrierName"),
                                    FTLRate = rdr.GetDouble("FTLRate"),
                                    LTLRate = rdr.GetDouble("LTLRate"),
                                    ReeferCharge = rdr.GetDouble("reefCharge"),
                                    IsActive = rdr.GetBoolean("IsActive")
                                };
                                carriers.Add(carr);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Error in GetAllCarriers: {e.Message}", LogLevel.Error);
                Logger.Log($"StackTrace: {e.StackTrace}", LogLevel.Error);
                throw;
            }

            return carriers;
        }


        /*
         * DELETE carriers from the system. 
         */

        public void DeleteCarrierFromSystem(Carrier carrier)
        {
            string query = "DELETE FROM Carriers WHERE CarrierName=@CarrierName"; 

            try
            {
                using(MySqlConnection conn = new MySqlConnection(ConnectionString()))
                {
                    conn.Open(); 
                    using(MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CarrierName", carrier.Name);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            //The carrier with the specified name has been deleted
                            Logger.Log($"Successfully delete the carrier: {carrier.Name}", LogLevel.Information); 
                        }
                    }
                }
            }

            catch(Exception e)
            {
                Logger.Log($"Error while deleting the carrier: {carrier.Name}", LogLevel.Error); 
            }
        }







    }
}
