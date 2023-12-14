/*
 * Filename: PlannerPage.xaml.cs
 * Project: Transport Management System
 * Date: December 15, 2023
 * Author: Bhuwan Shrestha, Ahmed Alemleh, Smaran Adhikari
 * Description: It serves as a code-behind file for the planner role.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
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
    /// Interaction logic for PlannerPage.xaml
    /// </summary>
    public partial class PlannerPage : Window
    {
        DAL dal = new DAL(); 
        Planner planner = new Planner();
        public PlannerPage()
        {
            InitializeComponent();
            DisplayOrders();
        }

        private void LoginPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DisplayOrders()
        {
            ordersButton.Background = Brushes.LightSkyBlue;
            allOrdersRadioButton.IsChecked = true;
            ordersGrid.Visibility = Visibility.Visible;
            Refresh_Orders();
        }

        private void Refresh_Orders()
        {
            List<Order> orderList = new List<Order>();

            if (allOrdersRadioButton.IsChecked == true)
            {
                // Get all orders
                orderList = planner.FetchOrders(2);
            }
            else if (activeOrdersRadioButton.IsChecked == true)
            {
                // Get active orders
                orderList = planner.FetchOrders(0);
            }
            else if (completedOrdersRadioButton.IsChecked == true)
            {
                // completed box is checked, fetch only completed orders
                orderList = planner.FetchOrders(1);
            }

            OrdersList.ItemsSource = orderList;
            // sort by OrderID
            CollectionView viewOrder = (CollectionView)CollectionViewSource.GetDefaultView(OrdersList.ItemsSource);
            viewOrder.SortDescriptions.Add(new SortDescription("OrderID", ListSortDirection.Ascending));
        }

        private void ordersButton_Click(object sender, RoutedEventArgs e)
        {
            ordersButton.Background = Brushes.LightSkyBlue;
            allOrdersRadioButton.IsChecked = true;
            ordersGrid.Visibility = Visibility.Visible;
            Refresh_Orders();
        }

        private void invoiceButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void allOrdersRadio_Click(object sender, RoutedEventArgs e)
        {
            Refresh_Orders();
        }

        private void activeOrdersRadio_Click(object sender, RoutedEventArgs e)
        {
            Refresh_Orders();
        }

        private void completedOrdersRadio_Click(object sender, RoutedEventArgs e)
        {
            Refresh_Orders();
        }

        private void Selection_Changed(object sender, SelectionChangedEventArgs e)
        {
            Order currentOrder = (Order)OrdersList.SelectedItem;
           
         
        }

        private void selectCarrier_Click(object sender, RoutedEventArgs e)
        {
            
            
        }
    }
}
