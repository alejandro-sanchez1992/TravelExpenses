using System;
using System.Collections.Generic;
using TravelExpenses.Common.Models;

namespace TravelExpenses.Prism.ViewModels
{
    public class MainViewModel
    {
        public List<TripResponse> TripsList
        {
            get;
            set;
        }

        public HomePageViewModel Trips
        {
            get;
            set;
        }

        public MyTripViewModel Trip
        {
            get;
            set;
        }

        public MainViewModel()
        {
            instance = this;
        }

        private static MainViewModel instance;

        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                return new MainViewModel();
            }

            return instance;
        }
    }
}
