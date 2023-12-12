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
    public partial class BuyerPage : Window
    {
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
        }
        //***************************************** Working with Marketplace Grid ********************************************

        private void marketPlaceButton_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
            DisplayMarketPlaceGrid(); 

        }

    }
}
