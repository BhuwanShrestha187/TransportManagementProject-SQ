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

namespace TransportManagement
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Window
    {
       private readonly Admin admin = new Admin();
        public AdminPage()
        {
            InitializeComponent();
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
            generalConfigGrid.Visibility = Visibility.Visible;
        }

        private void logFileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void manageDataButton_Click(object sender, RoutedEventArgs e)
        {

        }

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
                Logger.Log($"Changed Log directroy from --{oldPath}-- to --{newPath}--", LogLevel.Information);
            }

            else
            {
                System.Windows.MessageBox.Show("Log Path was not changed.", "Logpath Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Logger.Log("Failed to change the logpath", LogLevel.Error);
            }
        }

       
    }
}
