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
    /// Interaction logic for BuyerPage.xaml
    /// </summary>
    //

    
    public partial class BuyerPage : Window
    {
        Buyer buyer = new Buyer();
        public BuyerPage()
        {
            InitializeComponent();
            DisplayMarketPlaceGrid(); 
        }

        private void LoginPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ResetUI()
        {
            marketPlaceGrid.Visibility = Visibility.Hidden;
            marketPlaceButton.Background = Brushes.WhiteSmoke;
            
        }


        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DisplayMarketPlaceGrid()
        {
            marketPlaceGrid.Visibility = Visibility.Visible;
            marketPlaceButton.Background = Brushes.LightSkyBlue;
            List<Contract> contracts = new List<Contract>();
            contracts = buyer.GetContractsFromMarketPlaceDatabase();
            ContractsList.ItemsSource = contracts;

        }
        //***************************************** Working with Marketplace Grid ********************************************

        private void marketPlaceButton_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
            DisplayMarketPlaceGrid();     
        }


        private void acceptClientButton_Click(object sender, RoutedEventArgs e)
        {

            if (ContractsList.SelectedItem != null)
            {
                // Access the selected item and perform your action
                Contract selectedContract = (Contract)ContractsList.SelectedItem;

                // Example: Display a message with the selected client's name
                MessageBox.Show($"Accepted client: {selectedContract.ClientName}");

                // You can perform further actions based on the selected item
                // For example, update the database, navigate to another page, etc.
            }
            else
            {
                // No item selected, display a message or perform appropriate action
                MessageBox.Show("Please select a client before accepting.");
            }
        }
    }
}
