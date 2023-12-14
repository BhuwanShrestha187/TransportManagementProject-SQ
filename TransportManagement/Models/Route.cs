/*
 * Filename: DAL.cs
 * Project: Transport Management System
 * Date: December 15, 2023
 * Author: Bhuwan Shrestha, Ahmed Alemleh, Smaran Adhikari
 * Description: It serves as a model for the role for generating Routes..
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportManagement
{
    public class Route : INotifyPropertyChanged
    {
        private string destination;
        public string Destination
        {
            get { return destination; }
            set { destination = value; OnPropertyChanged(nameof(Destination)); }
        }

        private int distance;
        public int Distance
        {
            get { return distance; }
            set { distance = value; OnPropertyChanged(nameof(Distance)); }
        }

        private decimal time;
        public decimal Time
        {
            get { return time; }
            set { time = value; OnPropertyChanged(nameof(Time)); }
        }

        private string west;
        public string West
        {
            get { return west; }
            set { west = value; OnPropertyChanged(nameof(West)); }
        }

        private string east;
        public string East
        {
            get { return east; }
            set { east = value; OnPropertyChanged(nameof(East)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

   
}
