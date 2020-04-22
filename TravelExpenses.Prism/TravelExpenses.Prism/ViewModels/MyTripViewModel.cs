using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using TravelExpenses.Common.Helpers;
using TravelExpenses.Common.interfaces;
using TravelExpenses.Common.Models;
using TravelExpenses.Prism.Helpers;
using TravelExpenses.Prism.Views;

namespace TravelExpenses.Prism.ViewModels
{
    public class MyTripViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private TripResponse _trip;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _addExpensesCommand;

        public MyTripViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Trip";
            IsEnabled = true;
        }

        public DelegateCommand AddExpensesCommand => _addExpensesCommand ?? (_addExpensesCommand = new DelegateCommand(AddExpenses));

        public TripResponse Trip
        {
            get => _trip;
            set => SetProperty(ref _trip, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("trip"))
            {
                Trip = parameters.GetValue<TripResponse>("trip");
            }

            Title = $"Details of Trip: {_trip.Id}";
            LoadTripAsync(_trip.Id);
        }

        private async void LoadTripAsync(int id)
        {
            IsRunning = true;
            IsEnabled = false;

            string url = App.Current.Resources["UrlAPI"].ToString();
            bool connection = await _apiService.CheckConnectionAsync(url);
            if (!connection)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.ConnectionError,
                    Languages.Accept);
                return;
            }

            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            Response<object> response = await _apiService.GetTripAsync(
                url,
                "api",
                "/Trips",
                id,
                "bearer",
                token.Token);

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    response.Message,
                    Languages.Accept);
                return;
            }

            TripResponse tripResult = (TripResponse)response.Result;
            Trip = tripResult;
        }

        private async void AddExpenses()
        {
            NavigationParameters parameters = new NavigationParameters
            {
                { "tripId", _trip.Id },
            };

            await _navigationService.NavigateAsync(nameof(AddExpensePage), parameters);
        }
    }
}
