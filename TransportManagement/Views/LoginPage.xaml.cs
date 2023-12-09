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
            string userType = CheckUserData();  //This will hold the type of the User : Admin, Buyer or Planner
        }

        private string CheckUserData()
        {
            //Our Models is responsible to PLAY with the data in MVC. So, create one class in Model called DAL (Data Access Layer)

            string userType;
            return userType;
        }
    }
}
