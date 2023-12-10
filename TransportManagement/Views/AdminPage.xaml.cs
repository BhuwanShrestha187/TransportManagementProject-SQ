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
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Window
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        //When user drags the window
        private void LoginPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void ResetUI()
        {

        }
        private void generalConfigButton_Click(object sender, RoutedEventArgs e)
        {

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

        private void updateDatabaseButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void selectPathButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void updateLogPathButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
