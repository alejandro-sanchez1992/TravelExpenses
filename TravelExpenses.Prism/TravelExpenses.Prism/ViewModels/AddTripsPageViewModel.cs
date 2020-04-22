using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using TravelExpenses.Common.Helpers;
using TravelExpenses.Common.interfaces;
using TravelExpenses.Common.Models;
using TravelExpenses.Prism.Helpers;

namespace TravelExpenses.Prism.ViewModels
{
    public class AddTripsPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private CountryResponse _country;
        private ObservableCollection<CountryResponse> _countries;
        private CityResponse _city;
        private ObservableCollection<CityResponse> _cities;
        private DateTime _startDate;
        private DateTime _endDate;
        private DelegateCommand _saveCommand;
        private bool _isRunning;
        private bool _isEnabled;

        public AddTripsPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = Languages.Register;
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
            IsEnabled = true;
            LoadContriesAsync();
        }

        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(SaveAsync));

        public ObservableCollection<CountryResponse> Countries
        {
            get => _countries;
            set
            {
                SetProperty(ref _countries, value);
            }
        }

        public CountryResponse Country
        {
            get => _country;
            set
            {
                SetProperty(ref _country, value);

                if (_country.Id != 0)
                {
                    City = null;
                    LoadCitiesAsync(_country.Id);
                }
            }

        }

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public ObservableCollection<CityResponse> Cities
        {
            get => _cities;
            set => SetProperty(ref _cities, value);
        }

        public CityResponse City
        {
            get => _city;
            set => SetProperty(ref _city, value);
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

        private async void LoadContriesAsync()
        {
            IsRunning = true;
            IsEnabled = false;

            string url = App.Current.Resources["UrlAPI"].ToString();
            bool connection = await _apiService.CheckConnectionAsync(url);
            if (!connection)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.ConnectionError, Languages.Accept);
                return;
            }

            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var response = await _apiService.GetListAsync<CountryResponse>(
                url,
                "/api",
                "/Countries/GetCountries",
                "bearer",
                token.Token);

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.LoginError,
                    Languages.Accept);
                await _navigationService.GoBackAsync();
                return;
            }

            var countries = (List<CountryResponse>)response.Result;
            Countries = new ObservableCollection<CountryResponse>(countries);
        }

        private async void LoadCitiesAsync(int cityId)
        {
            IsRunning = true;
            IsEnabled = false;

            string url = App.Current.Resources["UrlAPI"].ToString();
            bool connection = await _apiService.CheckConnectionAsync(url);
            if (!connection)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.ConnectionError, Languages.Accept);
                return;
            }

            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            var response = await _apiService.GetListAsync<CityResponse>(
                url,
                "/api",
                $"/Countries/GetCities?id={cityId}",
                "bearer",
                token.Token);

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.LoginError,
                    Languages.Accept);
                await _navigationService.GoBackAsync();
                return;
            }

            var cities = (List<CityResponse>)response.Result;
            Cities = new ObservableCollection<CityResponse>(cities);
        }

        private async void SaveAsync()
        {
            var isValid = await ValidateData();

            if (!isValid)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            var employee = JsonConvert.DeserializeObject<EmployeeResponse>(Settings.Employee);
            
            var tripRequest = new TripRequest
            {
                UserId = new Guid(employee.User.Id),
                EmployeeId = employee.Id,
                City = City.Id,
                StartDate = StartDate,
                EndDate = EndDate
            };

            Response<object> response = await _apiService.PostAsync(url, "/api", "/Trips", tripRequest, "bearer", token.Token);

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error, response.Message, Languages.Accept);
                return;
            }

            await App.Current.MainPage.DisplayAlert(
                Languages.Accept,
                response.Message,
                Languages.Accept);

            await _navigationService.GoBackToRootAsync();
        }

        private async Task<bool> ValidateData()
        {
            if (string.IsNullOrEmpty(City.Name))
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.StartDate, Languages.Accept);
                return false;
            }

            return true;
        }
    }
}