/*
 * Filename: DAL.cs
 * Project: Transport Management System
 * Date: December 15, 2023
 * Author: Bhuwan Shrestha, Ahmed Alemleh, Smaran Adhikari
 * Description: It serves as a model for the role for Data access..
 */

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
using System.Security.Cryptography;

using GoogleEnum = Google.Protobuf.WellKnownTypes.Enum;
using SystemEnum = System.Enum;
using System.Data.SqlClient;
using System.Windows;
using System.Diagnostics;
using System.IO;
using static MySql.Data.MySqlClient.MySqlBackup;
using System.IO.Abstractions;
using System.Xml.Linq;
using TransportManagement;
using MySqlX.XDevAPI;
using System.Runtime.Remoting;

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

        public bool DeleteCarrierFromSystem(Carrier carrier)
        {
            bool deleted = false; 
            string query = "DELETE FROM Carriers WHERE CarrierName=@CarrierName";
            
            try
            {
                using(MySqlConnection conn = new MySqlConnection(ConnectionString()))
                {
                    conn.Open();
                  
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CarrierName", carrier.Name);
                        int rowsAffected = cmd.ExecuteNonQuery();
                       
                        if (rowsAffected > 0)
                        {
                            //The carrier with the specified name has been deleted
                            conn.Close();
                            deleted = true;
                        }
                    }
                }
            }

            catch(Exception e)
            {
                Logger.Log($"Error while deleting the carrier: {carrier.Name}. Exception: {e.Message}", LogLevel.Error); 
            }
            return deleted;
        }


        /*
         * From the database, fetch the carrierID by name
         */

        public int GetCarrierIDByName (string name)
        {
            string query = "SELECT CarrierID FROM Carriers WHERE CarrierName=@CarrierName";
            try
            {
                using(MySqlConnection conn = new MySqlConnection(ConnectionString()))
                {
                    conn.Open();
                    using(MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CarrierName", name);
                        object result = cmd.ExecuteScalar(); 
                        if(result != null && result != DBNull.Value)
                        {
                            conn.Close();
                            return Convert.ToInt32(result); 
                        }
                    }
                }
            }

            catch(Exception e )
            {
                Logger.Log($"Cannot extract the CarrierID by Name. {e.Message}", LogLevel.Error);
            }

            return -1; //Indicates failure
        }

        public bool DeleteCarrierCity(CarrierCity carrierCity)
        {
            string query = "DELETE FROM CarrierCity WHERE CarrierID=@CarrierID AND DepotCity=@DepotCity";
            try
            {
                using(MySqlConnection conn = new MySqlConnection(ConnectionString()))
                {
                    conn.Open(); 
                    using(MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CarrierID", carrierCity.Carrier.CarrierID);
                        cmd.Parameters.AddWithValue("@DepotCity", carrierCity.DepotCity.ToString());    
                        cmd.ExecuteNonQuery(); 
                        conn.Close();
                        return true;
                    }
                   
                }
            }

            catch(Exception e)
            {
                Logger.Log($"City deletion Failed!!", LogLevel.Error);
            }

            return false;
        }


        /*
         * Update the carrier info. 
         */

        public void UpdateCarrier(Carrier newCarrier)
        {
            string sql = "UPDATE Carriers SET CarrierName=@CarrierName, FTLRate=@FTLRate, LTLRate=@LTLRate, ReefCharge=@ReefCharge WHERE CarrierID=@CarrierID";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@CarrierName", newCarrier.Name);
                        cmd.Parameters.AddWithValue("@FTLRate", newCarrier.FTLRate);
                        cmd.Parameters.AddWithValue("@LTLRate", newCarrier.LTLRate);
                        cmd.Parameters.AddWithValue("@ReefCharge", newCarrier.ReeferCharge);
                        cmd.Parameters.AddWithValue("@CarrierID", newCarrier.CarrierID);

                        // Execute the insertion and check the number of rows affected
                        // An exception will be thrown if the column is repeated
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw;
            }
        }

        /*
         * Update Carrier City
         */

        public void UpdateCarrierCity(CarrierCity newCarrierCity, City oldCity)
        {
            string sql = "UPDATE CarrierCity SET DepotCity=@DepotCity, FTLAval=@FTLAval, LTLAval=@LTLAval WHERE CarrierID=@CarrierID AND DepotCity=@OldDepotCity";

            try
            {
                List<CarrierCity> CarrierCities = FilterCitiesByCarrier(newCarrierCity.Carrier.Name);
                // Check if current carrier already contains the new depot city, if so, don't duplicate
                if ((CarrierCities.FindIndex(carrierCity => carrierCity.DepotCity == newCarrierCity.DepotCity) >= 0) && oldCity != newCarrierCity.DepotCity)
                {
                    throw new ArgumentException($"Carrier city {newCarrierCity.DepotCity} already exists.");
                }

                using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@OldDepotCity", oldCity.ToString());
                        cmd.Parameters.AddWithValue("@CarrierID", newCarrierCity.Carrier.CarrierID);
                        cmd.Parameters.AddWithValue("@DepotCity", newCarrierCity.DepotCity.ToString());
                        cmd.Parameters.AddWithValue("@FTLAval", newCarrierCity.FTLAval);
                        cmd.Parameters.AddWithValue("@LTLAval", newCarrierCity.LTLAval);

                        // Execute the insertion and check the number of rows affected
                        // An exception will be thrown if the column is repeated
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw;
            }
        }



        //Remove Carrier City.
        public void RemoveCarrierCity(CarrierCity carrierCity)
        {
            string sql = "DELETE FROM CarrierCity WHERE CarrierID=@CarrierID AND DepotCity=@DepotCity";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@CarrierID", carrierCity.Carrier.CarrierID);
                        cmd.Parameters.AddWithValue("@DepotCity", carrierCity.DepotCity.ToString());

                        // Execute the insertion and check the number of rows affected
                        // An exception will be thrown if the column is repeated
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw;
            }
        }



        public void CreateCarrierCity(CarrierCity carrierCity)
        {
            string sql = "INSERT INTO CarrierCity (CarrierID, DepotCity, FTLAval, LTLAval) VALUES (@CarrierID, @DepotCity, @FTLAval, @LTLAval)";

            try
            {
                List<CarrierCity> CarrierCities = FilterCitiesByCarrier(carrierCity.Carrier.Name);
                // Check if current carrier already contains the new depot city, if so, don't duplicate
                if ((CarrierCities.FindIndex(_carrierCity => _carrierCity.DepotCity == carrierCity.DepotCity) >= 0))
                {
                    throw new ArgumentException($"Carrier city {carrierCity.DepotCity} already exists.");
                }

                using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@CarrierID", carrierCity.Carrier.CarrierID);
                        cmd.Parameters.AddWithValue("@DepotCity", carrierCity.DepotCity.ToString());
                        cmd.Parameters.AddWithValue("@FTLAval", carrierCity.FTLAval);
                        cmd.Parameters.AddWithValue("@LTLAval", carrierCity.LTLAval);

                        // Execute the insertion and check the number of rows affected
                        // An exception will be thrown if the column is repeated
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw new ArgumentException($"Carrier Depot city \"{carrierCity.DepotCity}\" already exists.");
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw;
            }
        }

        public long CreateCarrier(Carrier carrier)
        {
            string sql = "INSERT INTO Carriers (CarrierName, FTLRate, LTLRate, reefCharge) VALUES (@CarrierName, @FTLRate, @LTLRate, @reefCharge)";

            long id;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@CarrierName", carrier.Name);
                        cmd.Parameters.AddWithValue("@FTLRate", carrier.FTLRate);
                        cmd.Parameters.AddWithValue("@LTLRate", carrier.LTLRate);
                        cmd.Parameters.AddWithValue("@reefCharge", carrier.ReeferCharge);

                        // Execute the insertion and check the number of rows affected
                        // An exception will be thrown if the column is repeated
                        cmd.ExecuteNonQuery();

                        id = cmd.LastInsertedId;
                    }
                }
            }
            catch (MySqlException e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw new ArgumentException($"Carrier \"{carrier.Name}\" already exists.");
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw;
            }

            return id;       // Get the ID of the inserted item
        }


      
        public List<Route> GetRouteDataFromDatabase()
        {
            string query = "SELECT Destination, Distance, Time, West, East FROM Route";
            Logger.Log("GetRouteDatabase called", LogLevel.Information);
            List<Route> routeData = new List<Route>();

            try
            {

                using (MySqlConnection connection = new MySqlConnection(ConnectionString()))
                {
                    connection.Open();


                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Route routeItem = new Route
                                {
                                    Destination = reader["Destination"].ToString(),
                                    Distance = Convert.ToInt32(reader["Distance"]),
                                    Time = Convert.ToDecimal(reader["Time"]),
                                    West = reader["West"].ToString(),
                                    East = reader["East"].ToString()
                                };

                                routeData.Add(routeItem);
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Logger.Log($"Could not retrieve the routes. {e.Message}", LogLevel.Error);
            }

            return routeData;
        }



        public bool UpdateRouteInDatabase(string destination, int distance, decimal time, string west, string east)
        {
            bool result = false;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString()))
                {
                    connection.Open();

                    // Assuming your Route table has a primary key column named ID, and you have the ID of the record you want to update
                    // Replace IDColumnName with your actual primary key column name
                    int routeId = GetRouteIdFromDatabase(destination);

                    string query = "UPDATE Route SET Destination = @Destination, Distance = @Distance, Time = @Time, West = @West, East = @East WHERE RouteID = @RouteId";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Destination", destination);
                        command.Parameters.AddWithValue("@Distance", distance);
                        command.Parameters.AddWithValue("@Time", time);
                        command.Parameters.AddWithValue("@West", west);
                        command.Parameters.AddWithValue("@East", east);
                        command.Parameters.AddWithValue("@RouteId", routeId);

                        command.ExecuteNonQuery();
                        result = true;
                    }
                }
            }
            catch(Exception e )
            {
                Logger.Log($"Error while updating the routes data!! {e.Message}", LogLevel.Error);
            }
            return result;
        }

        public int GetRouteIdFromDatabase(string destination)
        {
       
            using (MySqlConnection connection = new MySqlConnection(ConnectionString()))
            {
                connection.Open();

                string query = "SELECT RouteID FROM Route WHERE Destination = @Destination";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Destination", destination);
                    var result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            return -1; // Return a default value or handle the case where the ID is not found
        }



        public bool ProcessBackUp(string backUpPath)
        {
            bool backup = false;
            string mysqldumpPath = @"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe";
            string command = $@"--host=localhost --user=root --password=Leoayan@24 --databases TMS --result-file={backUpPath}";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = mysqldumpPath,
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = command
            };
            try
            {
                // Start the process
                using (Process process = new Process { StartInfo = psi })
                {
                    process.Start();

                    // Wait for the process to exit
                    process.WaitForExit();

                    // Check for errors
                    if (process.ExitCode == 0)
                    {
                        backup = true;
                        Logger.Log("Backup COmpleted Successfully", LogLevel.Information);
                    }
                    else
                    {
                        backup = false;
                        Logger.Log("Back up Failed", LogLevel.Error);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Back Up Failed!! {e.Message}", LogLevel.Error);
            }
            return backup;
        }

        public void CreateClient(Client client )
        {
            string query = "INSERT INTO Clients (ClientName) VALUES (@ClientName)";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                       
                        cmd.Parameters.AddWithValue("@ClientName", client.ClientName);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw new ArgumentException($"Client {client.ClientName} already exists.");
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw;
            }
        }


        public Client FilterClientByName(string name)
        {
            string sql = "SELECT ClientID, ClientName FROM Clients WHERE ClientName=@ClientName";
            Client client = null;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@ClientName", name);

                        MySqlDataReader rdr = cmd.ExecuteReader();

                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                client = new Client
                                {
                                    ClientID = int.Parse(rdr["ClientID"].ToString()),
                                    ClientName = rdr["ClientName"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw new ArgumentException($"Unable to filter the clients by name. {e.Message}");
            }

            return client;
        }



        public void CreateOrder(Order order)
        {
            string sql = "INSERT INTO Orders (ClientID, Origin, Destination, JobType, Quantity, VanType, OrderCreationDate) " +
                "VALUES (@ClientID, @Origin, @Destination, @JobType, @Quantity, @VanType, @OrderCreationDate)";

            try
            {
                // Get the client from the database and raise an error if it doesn't exist
                Client client;
                if ((client = FilterClientByName(order.ClientName)) == null)
                {
                    throw new KeyNotFoundException($"Client {order.ClientName} does not exist in the database.");
                }

                using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        order.OrderAcceptedDate = DateTime.Now;

                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@ClientID", client.ClientID);
                        cmd.Parameters.AddWithValue("@Origin", order.Origin.ToString());
                        cmd.Parameters.AddWithValue("@Destination", order.Destination.ToString());
                        cmd.Parameters.AddWithValue("@JobType", order.JobType);
                        cmd.Parameters.AddWithValue("@Quantity", order.Quantity);
                        cmd.Parameters.AddWithValue("@VanType", order.VanType);
                        cmd.Parameters.AddWithValue("@OrderCreationDate", order.OrderAcceptedDate);

                        // Execute the insertion and check the number of rows affected
                        // An exception will be thrown if the column is repeated
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw new ArgumentException($"Something went wrong while creating the order. {e.Message}");
            }
            catch (KeyNotFoundException e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw;
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw;
            }
        }



        public List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();
            try
            {
              
                using (MySqlConnection con = new MySqlConnection(ConnectionString()))
                {
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM Orders" +
                         " INNER JOIN Clients ON Orders.ClientID = Clients.ClientID", con);

                    con.Open();
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            Order newOrder = new Order
                            {
                                OrderID = int.Parse(rdr["OrderID"].ToString()),
                                ClientName = rdr["ClientName"].ToString()
                            };
                        

                            newOrder.Origin = (City)Enum.Parse(typeof(City), rdr["Origin"].ToString(), true);                      
                            newOrder.Destination = (City)Enum.Parse(typeof(City), rdr["Destination"].ToString(), true);
                            if (int.TryParse(rdr["JobType"].ToString(), out int jobType))
                            {
                                newOrder.JobType = (JobType)jobType;
                            }

                            newOrder.VanType = (VanType)int.Parse(rdr["VanType"].ToString());

                            newOrder.Quantity = int.Parse(rdr["Quantity"].ToString());

                            newOrder.IsCompleted = int.Parse(rdr["IsCompleted"].ToString());

                            if (DateTime.TryParse(rdr["OrderCreationDate"].ToString(), out DateTime dt2))
                            {
                                newOrder.OrderAcceptedDate = dt2;
                            }

                            if (rdr["OrderCompletedDate"] != DBNull.Value && DateTime.TryParse(rdr["OrderCompletedDate"].ToString(), out DateTime dt3))
                            {
                                newOrder.OrderCompletionDate = dt3;
                            }


                            orders.Add(newOrder);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Error in GetALlOrders: {e.Message}", LogLevel.Error);
                throw new ArgumentException($"Unable to fetch all orders. {e.Message}");
            }

            return orders;
        }

        public List<Order> GetActiveOrders()
        {
            List<Order> orders = new List<Order>();
            try
            {
               
                using (MySqlConnection con = new MySqlConnection(ConnectionString()))
                {
                    Logger.Log("\n*************************Pending Orders Started Here***********************", LogLevel.Information);
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM Orders " +
                         "INNER JOIN Clients ON Orders.ClientID = Clients.ClientID WHERE IsCompleted=0", con);
                    con.Open();
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            Order newOrder = new Order
                            {
                                OrderID = int.Parse(rdr["OrderID"].ToString()),
                                ClientName = rdr["ClientName"].ToString()
                            };
                           

                            newOrder.Origin = (City)Enum.Parse(typeof(City), rdr["Origin"].ToString(), true);
                            newOrder.Destination = (City)Enum.Parse(typeof(City), rdr["Destination"].ToString(), true);
                            newOrder.JobType = (JobType)int.Parse(rdr["JobType"].ToString());
                            newOrder.VanType = (VanType)int.Parse(rdr["VanType"].ToString());
                            newOrder.Quantity = int.Parse(rdr["Quantity"].ToString());
                            newOrder.IsCompleted = int.Parse(rdr["IsCompleted"].ToString());
                            if (DateTime.TryParse(rdr["OrderCreationDate"].ToString(), out DateTime dt2))
                            {
                                newOrder.OrderAcceptedDate = dt2;
                            }

                            if (DateTime.TryParse(rdr["OrderCompletedDate"].ToString(), out DateTime dt3))
                            {
                                newOrder.OrderCompletionDate = dt3;
                            }

                            orders.Add(newOrder);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw new ArgumentException($"Unable to fetch all active orders. {e.Message}");
            }

            return orders;
        }


        public List<Order> GetCompletedOrders()
        {
            List<Order> orders = new List<Order>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConnectionString()))
                {
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM Orders " +
                         "INNER JOIN Clients ON Orders.ClientID = Clients.ClientID WHERE IsCompleted=1", con);
                    con.Open();
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            Order newOrder = new Order
                            {
                                OrderID = int.Parse(rdr["OrderID"].ToString()),
                                ClientName = rdr["ClientName"].ToString()
                            };
                            if (DateTime.TryParse(rdr["OrderDate"].ToString(), out DateTime dt))
                            {
                                newOrder.OrderCreationDate = dt;
                            }

                            newOrder.Origin = (City)Enum.Parse(typeof(City), rdr["Origin"].ToString(), true);
                            newOrder.Destination = (City)Enum.Parse(typeof(City), rdr["Destination"].ToString(), true);
                            newOrder.JobType = (JobType)int.Parse(rdr["JobType"].ToString());
                            newOrder.VanType = (VanType)int.Parse(rdr["VanType"].ToString());
                            newOrder.Quantity = int.Parse(rdr["Quantity"].ToString());
                            newOrder.IsCompleted = int.Parse(rdr["IsCompleted"].ToString());
                            if (DateTime.TryParse(rdr["OrderCreationDate"].ToString(), out DateTime dt2))
                            {
                                newOrder.OrderAcceptedDate = dt2;
                            }

                            if (DateTime.TryParse(rdr["OrderCompletedDate"].ToString(), out DateTime dt3))
                            {
                                newOrder.OrderCompletionDate = dt3;
                            }

                            orders.Add(newOrder);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw new ArgumentException($"Unable to fetch all completed orders. {e.Message}");
            }

            return orders;
        }



        public List<Client> GetActiveClientsFromDatabase()
        {
            string query = "SELECT * FROM Clients WHERE IsActive=1"; 
            List<Client> clients= new List<Client>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader(); 
                    if(rdr.HasRows)
                    {
                        while(rdr.Read())
                        {
                            Client newclient = new Client
                            {
                                ClientName = rdr["ClientName"].ToString(),
                                ClientID = int.Parse(rdr["ClientID"].ToString()),
                                IsActive = int.Parse(rdr["IsActive"].ToString())
                            };

                            clients.Add(newclient);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Log($"An error occured while fetching active clients. {ex.Message}", LogLevel.Error);
            }
            
            return clients;
        }


        // Get All clients
        public List<Client> GetClientsFromDatabase()
        {
            string query = "SELECT * FROM Clients";
            List<Client> clients = new List<Client>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString()))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            Client newclient = new Client
                            {
                                ClientName = rdr["ClientName"].ToString(),
                                ClientID = int.Parse(rdr["ClientID"].ToString()),
                                IsActive = int.Parse(rdr["IsActive"].ToString())
                            };

                            clients.Add(newclient);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Log($"An error occured while fetching active clients. {ex.Message}", LogLevel.Error);
            }

            return clients;
        }




    }
}
