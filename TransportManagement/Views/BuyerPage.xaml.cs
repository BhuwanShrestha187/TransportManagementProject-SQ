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
            clientsButton.Background = Brushes.WhiteSmoke;

            ordersGrid.Visibility = Visibility.Hidden;  
            ordersButton.Background = Brushes.WhiteSmoke; 
            clientsGrid.Visibility = Visibility.Hidden;
            
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
            contracts = buyer.FetchContracts();
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

            // Get the current contracts list shown in the table
            List<Contract> currentContracts = ContractsList.ItemsSource.Cast<Contract>().ToList();

            Buyer buyer = new Buyer();

            // Process each selected contract and generate an order for it
            foreach (Contract selectedContract in ContractsList.SelectedItems)
            {
                buyer.GenerateOrder(selectedContract);
                currentContracts.Remove(selectedContract);
            }

            // Update the contracts list in the ListView
            ContractsList.ItemsSource = currentContracts;
        }

        //************************************ Order Grid Started **********************************************
        private void ordersButton_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
            ordersGrid.Visibility = Visibility.Visible; 
            ordersButton.Background = Brushes.LightSkyBlue;
            allOrdersButton.Background = Brushes.LightSkyBlue;
            ViewOrders(); 
        }

        private void ViewOrders()
        {
            List<Order> orders = new List<Order>();
            orders = buyer.GetOrders(2);
            OrdersList.ItemsSource = orders;

        }

        private void allOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            allOrdersButton.Background = Brushes.LightSkyBlue;
            pendingOrdersButton.Background = Brushes.WhiteSmoke;
            completedOrdersButton.Background= Brushes.WhiteSmoke;
            ViewOrders();
            
        }

        private void pendingOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            pendingOrdersButton.Background = Brushes.LightSkyBlue;
            allOrdersButton.Background = Brushes.WhiteSmoke;
            completedOrdersButton.Background = Brushes.WhiteSmoke;
            List<Order> orderList = new List<Order>();
            orderList = buyer.GetOrders(0);
            OrdersList.ItemsSource = orderList;
        }

        private void completedOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            pendingOrdersButton.Background = Brushes.WhiteSmoke;
            allOrdersButton.Background = Brushes.WhiteSmoke;
            allOrdersButton.Background = Brushes.LightSkyBlue;
            List<Order> orderList = new List<Order>();
            orderList = buyer.GetOrders(1);
            OrdersList.ItemsSource = orderList;
        }

        private void generateOrdersButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // ********************** Working WIth Clients *****************************
        private void Clients_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
            clientsButton.Background = Brushes.LightSkyBlue;
            clientsGrid.Visibility = Visibility.Visible;

            List<Client> clientList = buyer.GetClientsFromDatabase(0);
            ClientsList.ItemsSource = clientList;
        }

     
        
    }
}
