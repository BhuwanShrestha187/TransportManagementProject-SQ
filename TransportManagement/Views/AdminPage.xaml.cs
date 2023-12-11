using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Globalization; 
namespace TransportManagement
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Window
    {
       private readonly Admin admin = new Admin();
        DAL dal = new DAL();
        public AdminPage()
        {
            InitializeComponent();
            dal.LoadConnectionString();
            ResetUI();
            StartUpGrid();
        }

        private void StartUpGrid()
        {
            generalConfigGrid.Visibility = Visibility.Visible;
            generalConfigButton.Background = Brushes.LightSkyBlue;
            GetDatabaseInfo(); 
            string logPath = Logger.GetCurrentLogDirectory();
            logPathTextBox.Text = logPath;
        }

        /*
         * Get the database info to display the information about the database in the textboxes
         */
        private void GetDatabaseInfo()
        {
            string server = ConfigurationManager.AppSettings["Server"];
            string port = ConfigurationManager.AppSettings["Port"];
            string user = ConfigurationManager.AppSettings["User"]; 
            string password = ConfigurationManager.AppSettings["Password"];
            string databaseName = ConfigurationManager.AppSettings["DatabaseName"];

            //Not make them visible in the textboxes
            serverTextBox.Text = server;
            portTextBox.Text = port;
            userTextBox.Text = user;
            passwordTextBox.Text = password;
            databaseTextBox.Text = databaseName;

        }

        //When user drags the window
        private void LoginPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /*
         ********* Reset the UI *************
        */
        private void ResetUI()
        {
            generalConfigGrid.Visibility = Visibility.Hidden; 
            logFileGrid.Visibility = Visibility.Hidden;
            manageDataGrid.Visibility = Visibility.Hidden;
         

            //Clear Background also
            logFileButton.Background = Brushes.WhiteSmoke;
            generalConfigButton.Background = Brushes.WhiteSmoke;
            manageDataButton.Background = Brushes.WhiteSmoke;
            
        }

        /*
         * ***************** Closes the application ********************
         */
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void generalConfigButton_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
            StartUpGrid();
        }

        private void logFileButton_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
            logFileGrid.Visibility = Visibility.Visible;
            logFileButton.Background = Brushes.LightSkyBlue;
            try
            {
                string logFilePath = Logger.GetCurrentLogDirectory();
                StringBuilder logContent = new StringBuilder();

                using(StreamReader sr = new StreamReader(logFilePath))
                {
                    string line; 
                    while((line = sr.ReadLine()) != null)
                    {
                        logContent.AppendLine(line);
                        
                    }
                }

                logFileContentsTextBlock.Text = logContent.ToString();  
            }

            catch(Exception ex)
            {
                System.Windows.MessageBox.Show("Error accessing the log file: " + Logger.GetCurrentLogDirectory()); 
                return;
            }
        }

        /*
         * ------------------- WOrking with the manageData ButtonCLick from here -----------------------
         */
        private void manageDataButton_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
            ResetManageButtonUI();
            manageDataButton.Background = Brushes.LightSkyBlue;
            manageDataGrid.Visibility = Visibility.Visible;
            updateRateGrid.Visibility = Visibility.Visible;
            rateButton.Background = Brushes.LightSkyBlue;
            DisplayRates();
        }

        private void  rateButton_Click(object sender, RoutedEventArgs e)
        {
            ResetManageButtonUI();
            updateRateGrid.Visibility = Visibility.Visible;
            rateButton.Background = Brushes.LightSkyBlue;
            DisplayRates(); 
        }

        private void ResetManageButtonUI()
        {
            //For the related UI as well
            updateRateGrid.Visibility = Visibility.Hidden;
            updateCarrierGrid.Visibility = Visibility.Hidden;

            //For Button background
            rateButton.Background = Brushes.WhiteSmoke;
            carrierButton.Background = Brushes.WhiteSmoke;

        }
        private void DisplayRates()
        {
            try
            {
                double ftlRate = admin.GetFTLRate();
                double ltlRate = admin.GetLTLRate();
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Cant fetch the rates!!" + ex.Message);
                throw;
            }

            ftlRateTextBox.Text = admin.GetFTLRate().ToString("C2", CultureInfo.CurrentCulture);
            ltlRateTextBox.Text = admin.GetLTLRate().ToString("C2", CultureInfo.CurrentCulture);
        }


        private void ftlUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(ftlRateTextBox.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out double newRate))
            {
                // Use the parsed value to update the database
                bool result = admin.UpdateFTLRate(newRate); 
                if(result == true)
                {
                    System.Windows.MessageBox.Show("Rates updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                else
                {
                    System.Windows.MessageBox.Show("Rates can't ne updated!!", "Success", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Invalid rate format. Please enter a valid currency value.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ltlUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(ltlRateTextBox.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out double newRate))
            {
                // Use the parsed value to update the database
                bool result = admin.UpdateLTLRate(newRate);
                if (result == true)
                {
                    System.Windows.MessageBox.Show("Rates updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                else
                {
                    System.Windows.MessageBox.Show("Rates can't ne updated!!", "Success", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Invalid rate format. Please enter a valid currency value.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //***** From  here update Carrier ******

        private void carrierButton_Click(object sender, RoutedEventArgs e)
        {
            ResetManageButtonUI();
            carrierButton.Background = Brushes.LightSkyBlue;
            CarrierDatabaseList.SelectedItem = null;
            updateCarrierGrid.Visibility = Visibility.Visible;
            CarriersFieldsHandler(sender, e);
            PopulateCarrierList(sender, e);
        }
        private void PopulateCarrierList(object sender, RoutedEventArgs e)
        {
            List<Carrier> carriersList = admin.FetchCarriers();
            CarrierDatabaseList.ItemsSource = carriersList;
        }

        private void PopulateCarrierCitiesList(object sender, RoutedEventArgs e)
        {
            Carrier selectedCarrier = (Carrier)CarrierDatabaseList.SelectedItem;

            List<CarrierCity> carriersList = admin.GetCitiesByCarrier(selectedCarrier.Name);
            CityDatabase.ItemsSource = carriersList;
        }

        private void CarriersFieldsHandler(object sender, RoutedEventArgs e)
        {
            clearButton_Click(sender, e);

            // If no option is selected
            if (CarrierDatabaseList.SelectedItems.Count == 0 && CityDatabase.SelectedItems.Count == 0)
            {
                updateButton.Visibility = Visibility.Hidden;
                deleteButton.Visibility = Visibility.Hidden;

                CityDatabase.ItemsSource = new List<CarrierCity>();
            }
            else
            {
                updateButton.Visibility = Visibility.Visible;
                deleteButton.Visibility = Visibility.Visible;

                Carrier selectedCarrier = (Carrier)CarrierDatabaseList.SelectedItem;

                if (selectedCarrier != null)
                {
                    try
                    {
                        string caller = (sender as System.Windows.Controls.ListView).Name;
                        if (caller == "CarrierDatabaseList")
                        {
                            List<CarrierCity> carriersList = admin.GetCitiesByCarrier(selectedCarrier.Name);
                            CityDatabase.ItemsSource = carriersList;
                        }
                    }
                    catch (Exception) { }

                    CarrierName.Text = selectedCarrier.Name;
                    FTLRate.Text = selectedCarrier.FTLRate.ToString();
                    LTLRate.Text = selectedCarrier.LTLRate.ToString();
                    Reefer.Text = selectedCarrier.ReeferCharge.ToString();

                    // Show details about the city if carrier and city is selected
                    if (CarrierDatabaseList.SelectedItems.Count == 1 && CityDatabase.SelectedItems.Count == 1)
                    {
                        Departure.Visibility = Visibility.Visible;
                        FTLAval.Visibility = Visibility.Visible;
                        LTLAval.Visibility = Visibility.Visible;

                        CarrierCity selectedCity = (CarrierCity)CityDatabase.SelectedItem;
                        Departure.Text = selectedCity.DepotCity.ToString();
                        FTLAval.Text = selectedCity.FTLAval.ToString();
                        LTLAval.Text = selectedCity.LTLAval.ToString();
                    }
                }
                else
                {
                    CityDatabase.ItemsSource = new List<CarrierCity>();
                }
            }
        }


        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            CarrierName.Text = "";
            Departure.Text = "";
            FTLAval.Text = "";
            LTLAval.Text = "";
            FTLRate.Text = "";
            LTLRate.Text = "";
            Reefer.Text = "";
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            //Load the selected carrier  at first
            Carrier carrier = (Carrier)CarrierDatabaseList.SelectedItem;
            
            //If only the carrier is selected
            if(CarrierDatabaseList.SelectedItems.Count == 1 && CityDatabase.SelectedItems.Count == 0)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show($"Are you sure you want to delete {carrier.Name} carrier ?", "Sure?", MessageBoxButton.YesNo); 
                if(result == MessageBoxResult.Yes)
                {
                    bool carrierDeleted = admin.DeleteCarrier(carrier); 
                    if(carrierDeleted == true)
                    {
                        PopulateCarrierList(sender, e);
                        CityDatabase.ItemsSource = new List<CarrierCity>();
                        System.Windows.MessageBox.Show($"{carrier.Name} was deleted successfully from the system", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }


            else if (CarrierDatabaseList.SelectedItems.Count == 0 && CarrierDatabaseList.SelectedItems.Count == 1)
            {
                CarrierCity carrierCity = (CarrierCity)CityDatabase.SelectedItem;
                carrier.CarrierID = admin.GetCarrierIDByName(carrier.Name);
                MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to delete the city: {carrierCity} ?", "Sure", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.Yes)
                {
                    
                }

            }
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {

        }



        private void routeButton_Click(object sender, RoutedEventArgs e)
        {

        }

     




        // *********************************************** Manage Button finished *************************************************************
        private void backupButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /*
         *  Triggers when the updateDatabase button is clicked
         */
        private void updateDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            string keyServer = "Server";
            string keyPort = "Port";
            string keyUser = "User";
            string keyPassword = "Password";
            string keyDatabase = "DatabaseName";

            //New values of server
            string newServer; 
            string newPort;
            string newUser;
            string newPassword;
            string newDatabase;

            try
            {
                //Get the information from the textboxes
                newServer = serverTextBox.Text;
                newPort = portTextBox.Text; 
                newUser = userTextBox.Text;
                newPassword = passwordTextBox.Text;
                newDatabase = databaseTextBox.Text;

                string connectionString = $"server={newServer};user={newUser};database={newDatabase};port={newPort};password={newPassword}; convert zero datetime=True";
                try
                {
                    MySqlConnection conn = new MySqlConnection(connectionString);
                    conn.Open(); 
                }

                catch(Exception)
                {
                    System.Windows.MessageBox.Show("Connection to the database was not successful!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    throw new ArgumentException(); 
                }

                //Now call the update method to update the config file
                admin.UpdateConnectionString(keyServer, newServer); 
                admin.UpdateConnectionString(keyPort, newPort);
                admin.UpdateConnectionString(keyUser, newUser);
                admin.UpdateConnectionString(keyPassword, newPassword);
                admin.UpdateConnectionString(keyDatabase, newDatabase);

                System.Windows.MessageBox.Show("Database updated Successfully!", "Database Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            catch(ArgumentException)
            {
                return; 
            }

            catch(Exception ex)
            {
                System.Windows.MessageBox.Show("Fill all the fields properly", "Error", MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
        }

        /*
         * Triggers when user clicks on the Select Path button to select the log path.
         */
        private void selectPathButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            string path = null;

            //Open the directory selection dialog box
            dialog.Description = "Select the Directory where you want to put the log files";
            dialog.ShowDialog(); 

            if(dialog.SelectedPath != null)
            {
                path = dialog.SelectedPath;
            }

            logPathTextBox.Text = path;

        }

        private void updateLogPathButton_Click(object sender, RoutedEventArgs e)
        {
            string oldPath = Logger.GetCurrentLogDirectory();
            string newPath = logPathTextBox.Text;

            int result = admin.UpdateLogPath(newPath); 
            if(result == 0)
            {
                System.Windows.MessageBox.Show("Log Path changed successfully.", "Logpath Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                Logger.Log($"Changed Log directroy from \"{oldPath}\" to \"{newPath}\"", LogLevel.Information);
            }

            else
            {
                System.Windows.MessageBox.Show("Log Path was not changed.", "Logpath Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log("Failed to change the logpath", LogLevel.Error);
            }
        }

        
    }
}
