using System;
using System.Collections.Generic;
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


namespace TransportManagement
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        public LoginPage()
        {
            InitializeComponent();
            Logger.Log("Application Started", LogLevel.Information);
        }

        private void LoginPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /*
         * When Login Button is clicked, it should show the relevant admin buyer or planner pages.
         * For this we have to check the existing data in the database to go to the relevant window.
         * For this I will make one CheckUserData() which will check in the database that the user is there or not. 
         *      1. Create a function called checkUserData() which will return a string containing the "Admin", "Buyer"
         *      or "Planner". If it doesnot return then exit the Current Window. 
         */
        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            string loginResult = CheckUserData();  //This will hold the type of the User : Admin, Buyer or Planner
            if (loginResult != null)
            {
                // loginResult is not null, user type is valid
                // go to the page of selected user type
                if (loginResult.Contains("Buyer"))
                {
                    BuyerPage buyer = new BuyerPage();
                    buyer.Show();
                }
                else if (loginResult.Contains("Planner"))
                {
                    PlannerPage planner = new PlannerPage();
                    planner.Show();
                }
                else if (loginResult.Contains("Admin"))
                {
                    AdminPage admin = new AdminPage();
                    admin.Show();
                }
                App.Current.MainWindow.Hide();
            }
        }

        /*
         * Checks the username or password is correct or not.
         */
        private string CheckUserData()
        {
            //Our Models is responsible to PLAY with the data in MVC. So, create one class in Model called DAL (Data Access Layer)
            DAL credentialCheck = new DAL();
            if(usernameTextBox.Text == "" || passwordTextBox.Text == "")
            {
                MessageBox.Show("Please provide the username and password!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null; 
            }
            else
            {
                if(credentialCheck.CheckUserName(usernameTextBox.Text) == false)
                {
                    //USername is not in the database
                    MessageBox.Show("Username doesnot exist in the system!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                if(credentialCheck.CheckUserPassword(usernameTextBox.Text,  passwordTextBox.Text) == false)
                {
                    //Password is incorrect
                    MessageBox.Show("Incorrect Password!! Please Try again!!");
                    return null; 
                }
            }

            string userType = credentialCheck.GetUserType(usernameTextBox.Text);
            return userType;     
        }






    }
}
