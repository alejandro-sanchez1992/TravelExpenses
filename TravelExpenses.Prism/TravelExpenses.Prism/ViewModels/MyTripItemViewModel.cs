using System;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using TravelExpenses.Common.Helpers;
using TravelExpenses.Common.Models;

namespace TravelExpenses.Prism.ViewModels
{
    public class MyTripItemViewModel : TripResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectTripCommand;

        public MyTripItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectTripCommand => _selectTripCommand ?? (_selectTripCommand = new DelegateCommand(SelectTrip));

        private async void SelectTrip()
        {
            var parameters = new NavigationParameters
            {
                { "trip", this }
            };

            await _navigationService.NavigateAsync("MyTripPage", parameters);
        }
    }
}
