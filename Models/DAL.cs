using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Documents;



namespace Transportation_Management_System

{

    public enum UserRole
    {
        Buyer,
        Planner,
        Admin
    }
    /*
     * The purpose of this class is to act as the Data Access Layer for the system. 
     * It will manage all the communication between the controller to the database. 
     * Therefore each query or change to database need to be managed by the DAL class. 
     */
    public class DAL
    {
        //Host of the database
        private string Server { get; set; }

        //Username to connect to the database
        private string User { get; set; }

        //Port to connect to the database
        private string Port { get; set; }

        //Password to connect to the databse
        private string Password { get; set; }

        //Name of the Database
        private string DatabaseName { get; set; }

        public DAL()
        {
            ConnectionStringStart();
        }

        private void ConnectionStringStart()
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

            PopulateConnectionString();
        }

        private void PopulateConnectionString()
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
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Critical);
                throw;
            }
        }
        public override string ToString()
        {
            return $"server={Server};user={User};database={DatabaseName};port={Port};password={Password}; convert zero datetime=True";
        }

        // Check USerName
        public bool CheckUsername(string username)
        {
            bool existing = false;
            using (MySqlConnection conn = new MySqlConnection(ToString()))
            {
                conn.Open();
                try
                {
                    string sql = $"SELECT * FROM Users WHERE Username='{username}'";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    // If data is found
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            string DbUsername = rdr["Username"].ToString();
                            if (username == DbUsername)
                            {
                                existing = true;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(e.Message, LogLevel.Error);
                    throw;
                }
                conn.Close();
            }

            return existing;
        }


        //Now Check Password
        public bool CheckUserPassword(string userName, string password)
        {
            //Compare password
            bool isValid = false;
            using (MySqlConnection connection = new MySqlConnection(ToString()))
            {
                connection.Open();
                try
                {
                    string query = $"SELECT * FROM Users WHERE Username='{userName}'";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    //If data is found
                    if (rdr.HasRows)
                    {
                        while ((rdr.Read()))
                        {
                            string DbPassword = rdr["Password"].ToString();
                            if (password == DbPassword)
                            {
                                isValid = true;
                            }
                        }
                    }
                }

                catch (Exception e) {
                    Logger.Log(e.Message, LogLevel.Error);
                    throw;
                }
                connection.Close();
            }
            return isValid;
        }


        //Now check if the user belongs to the role specified
        public string GetUserType(string userName)
        {
            string userType = null;
            using (MySqlConnection conn = new MySqlConnection(ToString()))
            {
                conn.Open();
                try
                {
                    string query = $"SELECT UserType FROM Users WHERE Username='{userName}'";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    //If data is found
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            UserRole DbUserType = (UserRole)int.Parse(rdr["UserType"].ToString());
                            userType = DbUserType.ToString();
                        }
                    }
                }

                catch (Exception e)
                {
                    Logger.Log(e.Message, LogLevel.Error);
                    throw;
                }
                conn.Close();
            }

            return userType;
        }


        //This function inserts a new user to the database
        public bool CreateUser(User user)
        {
            string query = "INSERT INTO Users (FirstName, LastName, UserName, Password, Email, UserType) " +
                "VALUES (@FirstName, @LastName, @UserName, @Password, @Email, @UserType)";

            bool userCreated = false;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ToString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", user.LastName);
                        cmd.Parameters.AddWithValue("@Username", user.Username);
                        cmd.Parameters.AddWithValue("@Password", user.Password);
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        cmd.Parameters.AddWithValue("@IsActive", 1);
                        cmd.Parameters.AddWithValue("@UserType", (int)user.UserType);

                        // Execute the insertion and check the number of rows affected
                        // An exception will be thrown if the column is repeated
                        cmd.ExecuteNonQuery();
                        userCreated = true;
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw new ArgumentException($"User \"{user.Username}\" already exists.");
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw;
            }
            return userCreated;
        }

        //Inserts a new order in the order table
        public void CreateOrder(Order order)
        {
            string sql = "INSERT INTO Orders (ClientID, Origin, Destination, JobType, Quantity, VanType, OrderCreationDate) " +
                "VALUES (@ClientID, @Origin, @Destination, @JobType, @Quantity, @VanType, @OrderCreationDate)";

            try
            {
                //Get the client from the database and raise an error if it doesn't exist
                Clients clients; 
                if((clients = FilterClientByName(order.ClientName)) == null)
                {
                    throw new KeyNotFoundException($"Client {order.ClientName} doesnot exist in the database.");
                }

                using (MySqlConnection conn = new MySqlConnection(ToString()))
                {
                    conn.Open();
                    
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        order.OrderAcceptedDate = DateTime.Now;

                        //Populate all argumetns in the insert
                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@ClientID", clients.ClientID);
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

        //Filter clients by Name
        public Clients FilterClientByName(string name)
        {
            string sql = "SELECT ClientID, ClientName FROM Clients WHERE ClientName=@ClientName";
            Clients clients = null; 

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ToString()))
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
                                clients = new Clients
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

            return clients;
        }


        //Set the order start date
        public void StartOrder(Order currentOrder)
        {
            string sql = "UPDATE Orders SET OrderDate=@OrderDate WHERE OrderID=@OrderID";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ToString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", currentOrder.OrderID);
                        cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
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

        // Inserts a new client in the client table
        public void CreateClient(Clients clients)
        {
            string sql = "INSERT INTO Clients (ClientName) VALUES (@ClientName)";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ToString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@ClientName", clients.ClientName);

                        // Execute the insertion and check the number of rows affected
                        // An exception will be thrown if the column is repeated
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw new ArgumentException($"Client {clients.ClientName} already exists.");
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw;
            }
        }

        // Create Invoice
        public void CreateInvoice(Invoice invoice)
        {
            string sql = "INSERT INTO Invoices VALUES (@OrderID, @ClientName, @Origin, " +
                "@Destination, @TotalAmount, @TotalDistance, @TotalDays, @CompletedDate)";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ToString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        invoice.CompletedDate = DateTime.Now;
                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@OrderID", invoice.OrderID);
                        cmd.Parameters.AddWithValue("@ClientName", invoice.ClientName);
                        cmd.Parameters.AddWithValue("@Origin", invoice.Origin);
                        cmd.Parameters.AddWithValue("@Destination", invoice.Destination);
                        cmd.Parameters.AddWithValue("@TotalAmount", invoice.TotalAmount);
                        cmd.Parameters.AddWithValue("@TotalDistance", invoice.TotalKM);
                        cmd.Parameters.AddWithValue("@TotalDays", invoice.Days);
                        cmd.Parameters.AddWithValue("@CompletedDate", invoice.CompletedDate);

                        // Execute the insertion and check the number of rows affected
                        // An exception will be thrown if the column is repeated
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw new ArgumentException($"Invoice for order #\"{invoice.OrderID}\" already exists.");
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw;
            }
        }


        //Check if there is an existing record in the Invoices table for the given orderID
        public bool IsExistentInvoice(long orderID)
        {
            bool isExistent = false;

            string qSQL = "SELECT * FROM Invoices WHERE OrderID=@OrderId";
            try
            {
                string connectionString = ToString();
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qSQL, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", orderID);

                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            isExistent = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw;
            }
            return isExistent;
        }


        //is responsible for updating a record in the "Routes" table of a MySQL database based on the destination. 

        public void UpdateRoute(Route newRoute)
        {
            string sql = "UPDATE Routes SET Distance=@Distance, Time=@Time WHERE Destination=@Destination";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ToString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@Distance", newRoute.Distance);
                        cmd.Parameters.AddWithValue("@Time", newRoute.Time);
                        cmd.Parameters.AddWithValue("@Destination", newRoute.Destination);

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


        //Get Routes
        public List<Route> GetRoutes()
        {
            List<Route> routeList = new List<Route>();
            string qSQL = "SELECT * FROM Routes";
            try
            {
                string connectionString = ToString();
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qSQL, conn))
                    {
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                Route route = new Route
                                {
                                    // Set default values
                                    West = City.Null,
                                    East = City.Null,
                                    Destination = (City)int.Parse(rdr["Destination"].ToString())
                                };

                                if (int.TryParse(rdr["Distance"].ToString(), out int d))
                                {
                                    route.Distance = d;
                                }

                                if (double.TryParse(rdr["Time"].ToString(), out double t))
                                {
                                    route.Time = t;
                                }

                                if (int.TryParse(rdr["West"].ToString(), out int w))
                                {
                                    route.West = (City)w;
                                }

                                if (int.TryParse(rdr["East"].ToString(), out int e))
                                {
                                    route.East = (City)e;
                                }

                                routeList.Add(route);
                            }
                        }
                    }
                }

                // Create the graph
                foreach (Route route in routeList)
                {
                    // If not found (edges), return null
                    route.WestPtr = routeList.ElementAtOrDefault((int)route.West);
                    route.EastPtr = routeList.ElementAtOrDefault((int)route.East);
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw;
            }
            return routeList;
        }


        // Inserts a new carrier in the carrier table
        public long CreateCarrier(Carrier carrier)
        {
            string sql = "INSERT INTO Carriers (CarrierName, FTLRate, LTLRate, reefCharge) VALUES (@CarrierName, @FTLRate, " +
                "@LTLRate, @reefCharge)";

            long id;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ToString()))
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


        // Inserts a new carrierCity in the carrier city table
        public void CreateCarrierCity(CarrierCity carrierCity)
        {
            string sql = "INSERT INTO CarrierCity (CarrierID, DepotCity, FTLAval, LTLAval) VALUES (@CarrierID," +
                " @DepotCity, @FTLAval, @LTLAval)";

            try
            {
                List<CarrierCity> CarrierCities = FilterCitiesByCarrier(carrierCity.Carrier.Name);
                // Check if current carrier already contains the new depot city, if so, don't duplicate
                if ((CarrierCities.FindIndex(_carrierCity => _carrierCity.DepotCity == carrierCity.DepotCity) >= 0))
                {
                    throw new ArgumentException($"Carrier city {carrierCity.DepotCity} already exists.");
                }

                using (MySqlConnection conn = new MySqlConnection(ToString()))
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

        //It updates Carrier
        public void UpdateCarrier(Carrier newCarrier)
        {
            string sql = "UPDATE Carriers SET CarrierName=@CarrierName, FTLRate=@FTLRate, LTLRate=@LTLRate, ReefCharge=@ReefCharge WHERE CarrierID=@CarrierID";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ToString()))
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


        //It update existing carrier attributes
        public void UpdateCarrierCity(CarrierCity newCarrierCity, City oldCity)
        {
            string sql = "UPDATE CarrierCity SET DepotCity=@DepotCity, FTLAval=@FTLAval, LTLAval=@LTLAval " +
                "WHERE CarrierID=@CarrierID AND DepotCity=@OldDepotCity";

            try
            {
                List<CarrierCity> CarrierCities = FilterCitiesByCarrier(newCarrierCity.Carrier.Name);
                // Check if current carrier already contains the new depot city, if so, don't duplicate
                if ((CarrierCities.FindIndex(carrierCity => carrierCity.DepotCity == newCarrierCity.DepotCity) >= 0) && oldCity != newCarrierCity.DepotCity)
                {
                    throw new ArgumentException($"Carrier city {newCarrierCity.DepotCity} already exists.");
                }

                using (MySqlConnection conn = new MySqlConnection(ToString()))
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

        //Get a carrier by its id
        public Carrier GetCarrier(int carrierId)
        {
            string qSQL = "SELECT * FROM Carriers WHERE CarrierID=@CarrierID";
            Carrier carr = null;
            try
            {
                string conString = ToString();
                using (MySqlConnection conn = new MySqlConnection(conString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qSQL, conn))
                    {
                        cmd.Parameters.AddWithValue("@CarrierID", carrierId);
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                carr = new Carrier
                                {
                                    CarrierID = int.Parse(rdr["CarrierID"].ToString()),
                                    Name = rdr["CarrierName"].ToString(),
                                    FTLRate = double.Parse(rdr["FTLRate"].ToString()),
                                    LTLRate = double.Parse(rdr["LTLRate"].ToString()),
                                    ReeferCharge = double.Parse(rdr["reefCharge"].ToString())
                                };
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
            return carr;
        }

        /// \brief Deactivate an active carrier by its id

        public void DeactivateCarrier(Carrier carrier)
        {
            string sql = "UPDATE Carriers SET IsActive=0 WHERE CarrierID=@CarrierID";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ToString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@CarrierID", carrier.CarrierID);

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


        //Used to update the invoice generated field for a specific order
        public void UpdateInvoiceGenerated(long orderID)
        {
            string sql = "UPDATE Orders SET InvoiceGenerated=1 WHERE OrderID=@OrderID";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ToString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@OrderID", orderID);

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


        //Deactivate an active carrier by its id
        public void DeleteCarrier(Carrier carrier)
        {
            string sql = "DELETE FROM Carriers WHERE CarrierName=@CarrierName";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ToString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@CarrierName", carrier.Name);

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


        //Remove a city from the carrier
        public void RemoveCarrierCity(CarrierCity carrierCity)
        {
            string sql = "DELETE FROM CarrierCity WHERE CarrierID=@CarrierID AND DepotCity=@DepotCity";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ToString()))
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


        //Filter carriers by city
        public List<CarrierCity> FilterCarriersByCity(City city, int jobType)
        {
            List<CarrierCity> carrierCities = new List<CarrierCity>();
            string qSQL;
            if (jobType == 0)
            {
                // job type is FTL
                qSQL = "SELECT * FROM Carriers INNER JOIN CarrierCity ON CarrierCity.CarrierID = Carriers.CarrierID " +
                    "WHERE DepotCity=@DepotCity AND IsActive=1 AND FTLAval>0";
            }
            else
            {
                // job type is LTL
                qSQL = "SELECT * FROM Carriers INNER JOIN CarrierCity ON CarrierCity.CarrierID = Carriers.CarrierID " +
                    "WHERE DepotCity=@DepotCity AND IsActive=1 AND LTLAval>0";
            }

            try
            {
                string conString = ToString();
                using (MySqlConnection conn = new MySqlConnection(conString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qSQL, conn))
                    {
                        cmd.Parameters.AddWithValue("@DepotCity", city.ToString());
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
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


        // Filter carriers by id
        public Carrier FilterCarriersByID(long carrierID)
        {
            Carrier carrier = new Carrier();
            string qSQL = "SELECT * FROM Carriers WHERE CarrierID=@CarrierID";

            try
            {
                string conString = ToString();
                using (MySqlConnection conn = new MySqlConnection(conString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qSQL, conn))
                    {
                        cmd.Parameters.AddWithValue("@CarrierID", carrierID);

                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                carrier.CarrierID = int.Parse(rdr["CarrierID"].ToString());
                                carrier.Name = rdr["CarrierName"].ToString();
                                carrier.FTLRate = double.Parse(rdr["FTLRate"].ToString());
                                carrier.LTLRate = double.Parse(rdr["LTLRate"].ToString());
                                carrier.ReeferCharge = double.Parse(rdr["reefCharge"].ToString());
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

            return carrier;
        }


        // Filter cities by carrier
        public List<CarrierCity> FilterCitiesByCarrier(string carrierName)
        {
            List<CarrierCity> carrierCities = new List<CarrierCity>();
            string qSQL = "SELECT * FROM CarrierCity INNER JOIN Carriers ON " +
                "CarrierCity.CarrierID = Carriers.CarrierID WHERE CarrierName=@CarrierName AND IsActive=1";

            try
            {
                string conString = ToString();
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

        // Returns a list of all users in our system
        public List<User> GetUsers()
        {
            List<User> usersList = new List<User>();
            string qSQL = "SELECT * FROM Users";
            try
            {
                string connectionString = ToString();
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qSQL, conn))
                    {
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                User user = new User
                                {
                                    FirstName = rdr["FirstName"].ToString(),
                                    LastName = rdr["LastName"].ToString(),
                                    Username = rdr["Username"].ToString(),
                                    Password = rdr["Password"].ToString(),
                                    Email = rdr["Email"].ToString(),
                                    IsActive = bool.Parse(rdr["IsActive"].ToString()),
                                    UserType = (UserRole)int.Parse(rdr["UserType"].ToString())
                                };
                                usersList.Add(user);
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
            return usersList;
        }

        // Returns a list of all clients in our system
        public List<Clients> GetClients()
        {
            List<Clients> clientsList = new List<Clients>();
            string qSQL = "SELECT * FROM Clients";
            try
            {
                string connectionString = ToString();
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qSQL, conn))
                    {
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                Clients client = new Clients
                                {
                                    ClientID = int.Parse(rdr["ClientID"].ToString()),
                                    ClientName = rdr["ClientName"].ToString(),
                                    IsActive = int.Parse(rdr["IsActive"].ToString())
                                };
                                clientsList.Add(client);
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
            return clientsList;
        }

        // Returns a list of all active orders
        public List<Order> GetActiveOrders()
        {
            List<Order> orders = new List<Order>();
            try
            {
                string conString = ToString();
                using (MySqlConnection con = new MySqlConnection(conString))
                {
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
                throw new ArgumentException($"Unable to fetch all active orders. {e.Message}");
            }

            return orders;
        }

        //Returns a list of all completed orders
        public List<Order> GetCompletedOrders()
        {
            List<Order> orders = new List<Order>();
            try
            {
                string conString = ToString();
                using (MySqlConnection con = new MySqlConnection(conString))
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

        // Returns a list of all completed orders that have an invoice generated for them
        public List<Order> GetInvoiceGeneratedOrders()
        {
            List<Order> orders = new List<Order>();
            try
            {
                string conString = ToString();
                using (MySqlConnection con = new MySqlConnection(conString))
                {
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM Orders " +
                         "INNER JOIN Clients ON Orders.ClientID = Clients.ClientID WHERE IsCompleted=1 AND InvoiceGenerated=1", con);
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
                            if (DateTime.TryParse(rdr["OrderCreationDate"].ToString(), out DateTime dt))
                            {
                                newOrder.OrderAcceptedDate = dt;
                            }

                            newOrder.Origin = (City)Enum.Parse(typeof(City), rdr["Origin"].ToString(), true);
                            newOrder.Destination = (City)Enum.Parse(typeof(City), rdr["Destination"].ToString(), true);
                            newOrder.JobType = (JobType)int.Parse(rdr["JobType"].ToString());
                            newOrder.VanType = (VanType)int.Parse(rdr["VanType"].ToString());
                            newOrder.Quantity = int.Parse(rdr["Quantity"].ToString());
                            newOrder.InvoiceGenerated = int.Parse(rdr["InvoiceGenerated"].ToString());
                            newOrder.IsCompleted = int.Parse(rdr["IsCompleted"].ToString());
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


        // Returns a list of all orders
        public List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();
            try
            {
                string conString = ToString();
                using (MySqlConnection con = new MySqlConnection(conString))
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
                throw new ArgumentException($"Unable to fetch all orders. {e.Message}");
            }

            return orders;
        }

        // Set an order to completed
        public void CompleteOrder(Order order)
        {
            string sql = "UPDATE Orders SET IsCompleted=1, OrderCompletedDate=@OrderCompletedDate WHERE OrderID=@OrderID";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ToString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        order.OrderCompletionDate = DateTime.Now;

                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@OrderID", order.OrderID);
                        cmd.Parameters.AddWithValue("@OrderCompletedDate", order.OrderCompletionDate.ToString("yyyy-MM-dd H:mm:ss"));

                        // Execute the insertion and check the number of rows affected
                        // An exception will be thrown if the column is repeated
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            throw new ArgumentException("Invalid Order");
                        }
                    }
                }
            }
            catch (ArgumentException e)
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


        // Determine whether an order has been assigned to a carrier or not
        public bool IsCarriedAssigned(Order currentOrder)
        {
            bool isCarrierAssigned = false;
            try
            {
                string conString = ToString();
                using (MySqlConnection con = new MySqlConnection(conString))
                {
                    MySqlCommand cmd = new MySqlCommand("SELECT Trips.OrderID FROM Trips" +
                         " INNER JOIN Orders ON Orders.OrderID = Trips.OrderID" +
                         " WHERE Trips.OrderID = @OrderID", con);

                    con.Open();

                    cmd.Parameters.AddWithValue("@OrderID", currentOrder.OrderID);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        isCarrierAssigned = true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw new ArgumentException($"Unable to fetch all orders. {e.Message}");
            }
            return isCarrierAssigned;
        }


        //  Determine whether an order has an invoice generated for it or not
        public bool IsInvoiceGenerated(Order order)
        {
            bool isInvoiceGenerated = false;
            try
            {
                string conString = ToString();
                using (MySqlConnection con = new MySqlConnection(conString))
                {
                    MySqlCommand cmd = new MySqlCommand("SELECT OrderID from Orders WHERE OrderID=@OrderID AND InvoiceGenerated=1", con);

                    con.Open();

                    cmd.Parameters.AddWithValue("@OrderID", order.OrderID);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        isInvoiceGenerated = true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw new ArgumentException($"Unable to fetch all orders. {e.Message}");
            }

            return isInvoiceGenerated;
        }


        // Return a list of all active customers in our system
        public List<Clients> GetActiveClients()
        {
            List<Clients> clients = new List<Clients>();
            try
            {
                string conString = ToString();
                using (MySqlConnection con = new MySqlConnection(conString))
                {
                    MySqlCommand cmd = new MySqlCommand("SELECT * From Clients WHERE IsActive=1", con);
                    con.Open();
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            Clients newClient = new Clients
                            {
                                ClientName = rdr["ClientName"].ToString(),
                                ClientID = int.Parse(rdr["ClientID"].ToString()),
                                IsActive = int.Parse(rdr["IsActive"].ToString())
                            };
                            clients.Add(newClient);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw new ArgumentException($"Unable to fetch all active clients. {e.Message}");
            }

            return clients;
        }


        // Return a list of all carriers in our system
        public List<Carrier> GetAllCarriers()
        {
            string qSQL = "SELECT * FROM Carriers WHERE IsActive=1";
            List<Carrier> carriers = new List<Carrier>();
            try
            {
                string conString = ToString();
                using (MySqlConnection conn = new MySqlConnection(conString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qSQL, conn))
                    {
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                Carrier carr = new Carrier
                                {
                                    CarrierID = int.Parse(rdr["CarrierID"].ToString()),
                                    Name = rdr["CarrierName"].ToString(),
                                    FTLRate = double.Parse(rdr["FTLRate"].ToString()),
                                    LTLRate = double.Parse(rdr["LTLRate"].ToString()),
                                    ReeferCharge = double.Parse(rdr["reefCharge"].ToString()),
                                    IsActive = bool.Parse(rdr["IsActive"].ToString())
                                };
                                carriers.Add(carr);
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
            return carriers;
        }


        // Return carrierID of the carrier
        public int GetCarrierIdByName(string carrierName)
        {
            string qSQL = "SELECT CarrierID FROM Carriers WHERE CarrierName=@CarrierName";
            int carrierID = -1;

            try
            {
                string conString = ToString();
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
                                carrierID = int.Parse(rdr["CarrierID"].ToString());
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
            return carrierID;
        }


        //  Used to get all the OSHT Rates.

        public Rate GetOSHTRates()
        {
            Rate OSHTRates = new Rate
            {
                RateValuePair = new Dictionary<RateType, double>()
            };
            string qSQL = "SELECT Rate, RateType FROM Rates WHERE Name='OSHT'";

            try
            {
                string conString = ToString();
                using (MySqlConnection conn = new MySqlConnection(conString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qSQL, conn))
                    {
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                RateType rateType = (RateType)int.Parse(rdr["RateType"].ToString());
                                double rateValue = double.Parse(rdr["Rate"].ToString());

                                OSHTRates.RateValuePair.Add(rateType, rateValue);
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

            return OSHTRates;
        }

        //  Update the OSHT Rate in the Rates table

        public void UpdateOSHTRate(double newRate, RateType rateType)
        {
            string sql = "UPDATE Rates SET Rate=@Rate WHERE Name='OSHT' AND RateType=@rateType";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ToString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@RateType", rateType);
                        cmd.Parameters.AddWithValue("@Rate", newRate);
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

        // Used to create a trip using trip object
        public void CreateTrip(Trip trip)
        {
            string sql = "INSERT INTO Trips (OrderID, CarrierID, OriginCity, DestinationCity, " +
                "JobType, VanType, TotalDistance, TotalTime) VALUE (@OrderID, @CarrierID, @OriginCity, " +
                "@DestinationCity, @JobType, @VanType, @TotalDistance, @TotalTime)";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ToString()))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // Populate all arguments in the insert
                        cmd.Parameters.AddWithValue("@OrderID", trip.OrderID);
                        cmd.Parameters.AddWithValue("@CarrierID", trip.CarrierID);
                        cmd.Parameters.AddWithValue("@OriginCity", trip.OriginCity);
                        cmd.Parameters.AddWithValue("@DestinationCity", trip.DestinationCity);
                        cmd.Parameters.AddWithValue("@JobType", trip.JobType);
                        cmd.Parameters.AddWithValue("@VanType", trip.VanType);
                        cmd.Parameters.AddWithValue("@TotalDistance", trip.TotalDistance);
                        cmd.Parameters.AddWithValue("@TotalTime", trip.TotalTime);

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


        // Returns a list with all trips attached to a specific order
        public List<Trip> FilterTripsByOrderId(long orderId)
        {
            List<Trip> trips = new List<Trip>();
            string qSQL = "SELECT * FROM Trips WHERE OrderID=@OrderID";

            try
            {
                string conString = ToString();
                using (MySqlConnection conn = new MySqlConnection(conString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qSQL, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                Trip trip = new Trip
                                {
                                    TripID = int.Parse(rdr["TripID"].ToString()),
                                    OrderID = int.Parse(rdr["OrderID"].ToString()),
                                    CarrierID = int.Parse(rdr["CarrierID"].ToString()),
                                    OriginCity = (City)Enum.Parse(typeof(City), rdr["OriginCity"].ToString(), true),
                                    DestinationCity = (City)Enum.Parse(typeof(City), rdr["DestinationCity"].ToString(), true),
                                    TotalDistance = int.Parse(rdr["TotalDistance"].ToString()),
                                    TotalTime = double.Parse(rdr["TotalTime"].ToString())
                                };
                                trips.Add(trip);
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

            return trips;
        }

        // Fetches the total time of a trip
        public double GetTotalTimeFromTrip(Order currentOrder)
        {
            double totalTime = 0;
            string qSQL = "SELECT TotalTime FROM Trips WHERE OrderID=@OrderID";

            try
            {
                string conString = ToString();
                using (MySqlConnection conn = new MySqlConnection(conString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qSQL, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", currentOrder.OrderID);
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                totalTime = double.Parse(rdr["TotalTime"].ToString());
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

            return totalTime;
        }

        // Returns a list with all orders filtered by 2 weeks or all time
        public List<Invoice> FilterInvoicesByTime(bool onlyPast2Weeks)
        {
            List<Invoice> invoices = new List<Invoice>();

            try
            {
                string qSQL;

                // If all time
                if (onlyPast2Weeks)
                {
                    // Filter all invoices from the past 2 weeks
                    qSQL = "SELECT * FROM Invoices WHERE CompletedDate between date_sub" +
                        "(@CurrentDatetime, INTERVAL 2 WEEK) and @CurrentDatetime";
                }
                else
                {
                    qSQL = "SELECT * FROM Invoices";
                }

                string conString = ToString();
                using (MySqlConnection conn = new MySqlConnection(conString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(qSQL, conn))
                    {
                        cmd.Parameters.AddWithValue("@CurrentDatetime", DateTime.Now);

                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                Invoice invoice = new Invoice()
                                {
                                    OrderID = long.Parse(rdr["OrderID"].ToString()),
                                    ClientName = rdr["ClientName"].ToString(),
                                    Origin = (City)Enum.Parse(typeof(City), rdr["Origin"].ToString(), true),
                                    Destination = (City)Enum.Parse(typeof(City), rdr["Destination"].ToString(), true),
                                    TotalAmount = decimal.Parse(rdr["TotalAmount"].ToString()),
                                    TotalKM = int.Parse(rdr["TotalDistance"].ToString()),
                                    Days = double.Parse(rdr["TotalDays"].ToString()),
                                    CompletedDate = DateTime.Parse(rdr["CompletedDate"].ToString())
                                };

                                invoices.Add(invoice);
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

            return invoices;
        }

        //  Backup up the entire database to a .sql file
        public void BackupDatabase(string backUpFilePath)
        {
            string fileName = $"TMS-DB-Backup-{DateTime.Now:MM-dd-yyyy HH-mm-ss}.sql";
            if (backUpFilePath == "")
            {
                throw new ArgumentNullException("Backup file path was not provided. Backup failed.");
            }

            string fullPath = $"{backUpFilePath}\\{fileName}";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(ToString()))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            mb.ExportToFile(fullPath);
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogLevel.Error);
                throw;
            }
        }




    }
}


