﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
    public class HomePageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private EmployeeResponse _employee;
        private ObservableCollection<MyTripItemViewModel> _myTrips;
        private DelegateCommand _addTripsCommand;
        private bool _isRunning;
        private bool _isEnabled;
        private static HomePageViewModel _instance;

        public HomePageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _instance = this;
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "My Trips Page";
            IsEnabled = true;
            //LoadEmployee();
        }

        public DelegateCommand AddTripsCommand => _addTripsCommand ?? (_addTripsCommand = new DelegateCommand(AddTrips));


        public ObservableCollection<MyTripItemViewModel> MyTrips
        {
            get => _myTrips;
            set => SetProperty(ref _myTrips, value);
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

        public static HomePageViewModel GetInstance()
        {
            return _instance;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            _employee = JsonConvert.DeserializeObject<EmployeeResponse>(Settings.Employee);
            Title = $"Trips of: {_employee.User.FirstName}";
            LoadTripsAsync(_employee.Id);
        }

        private async void LoadTripsAsync(int id)
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

            Response<object> response = await _apiService.GetListAsync<TripResponse>(
                url,
                "api",
                $"/Trips/GetMyTrips/{id}",
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

            MainViewModel.GetInstance().TripsList = (List<TripResponse>)response.Result;
            MyTrips = new ObservableCollection<MyTripItemViewModel>(ToItemViewModel());
        }

        private IEnumerable<MyTripItemViewModel> ToItemViewModel()
        {
            return MainViewModel.GetInstance().TripsList.Select(l => new MyTripItemViewModel(_navigationService)
            {
                Id = l.Id,
                City = l.City,
                StartDate = l.StartDateLocal,
                EndDate = l.EndDateLocal,
                TripDetails = l.TripDetails
            });
        }

        private async void AddTrips()
        {
            await _navigationService.NavigateAsync("AddTripsPage");
        }
    }
}
