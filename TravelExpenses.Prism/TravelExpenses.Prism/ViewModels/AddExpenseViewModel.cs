using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using TravelExpenses.Common.Helpers;
using TravelExpenses.Common.interfaces;
using TravelExpenses.Common.Models;
using TravelExpenses.Prism.Helpers;
using Xamarin.Forms;

namespace TravelExpenses.Prism.ViewModels
{
    public class AddExpenseViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private readonly IFilesHelper _filesHelper;
        private ExpenseTypeResponse _expenseType;
        private DateTime _expenseDate;
        private ImageSource _image;
        private MediaFile _file;
        private int _tripId;
        private TripDetailRequest _tripDetail;
        private ObservableCollection<ExpenseTypeResponse> _expesesTypes;
        private DelegateCommand _saveCommand;
        private DelegateCommand _changeImageCommand;
        private bool _isRunning;
        private bool _isEnabled;

        public AddExpenseViewModel(
            INavigationService navigationService,
            IApiService apiService,
            IFilesHelper filesHelper) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            _filesHelper = filesHelper;
            Title = "Add New Expenses";
            ExpenseDate = DateTime.Today;
            Image = App.Current.Resources["UrlNoImage2"].ToString();
            TripDetail = new TripDetailRequest();
            IsEnabled = true;
        }

        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(SaveAsync));
        public DelegateCommand ChangeImageCommand => _changeImageCommand ?? (_changeImageCommand = new DelegateCommand(ChangeImageAsync));

        public ObservableCollection<ExpenseTypeResponse> ExpenseTypes
        {
            get => _expesesTypes;
            set => SetProperty(ref _expesesTypes, value);
        }

        public ExpenseTypeResponse ExpenseType
        {
            get => _expenseType;
            set => SetProperty(ref _expenseType, value);
        }

        public TripDetailRequest TripDetail
        {
            get => _tripDetail;
            set => SetProperty(ref _tripDetail, value);
        }

        public DateTime ExpenseDate
        {
            get => _expenseDate;
            set => SetProperty(ref _expenseDate, value);
        }

        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
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

            _tripId = parameters.GetValue<int>("tripId");
            LoadExpenseTypesAsync();
        }

        private async void LoadExpenseTypesAsync()
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

            var response = await _apiService.GetListAsync<ExpenseTypeResponse>(
                url,
                "/api",
                "/ExpenseType",
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

            var expenseTypes = (List<ExpenseTypeResponse>)response.Result;
            ExpenseTypes = new ObservableCollection<ExpenseTypeResponse>(expenseTypes);
        }

        private async void SaveAsync()
        {
            bool isValid = await ValidateDataAsync();
            if (!isValid)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;
            string url = App.Current.Resources["UrlAPI"].ToString();
            bool connection = await _apiService.CheckConnectionAsync(url);
            var token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            if (!connection)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.ConnectionError, Languages.Accept);
                return;
            }

            byte[] imageArray = null;
            if (_file != null)
            {
                imageArray = _filesHelper.ReadFully(_file.GetStream());
            }

            TripDetail.PictureArray = imageArray;
            TripDetail.Date = ExpenseDate;
            TripDetail.ExpenseId = ExpenseType.Id;
            TripDetail.TripId = _tripId;
            Response<object> response = await _apiService.PostAsync(url, "/api", "/Trips/PostExpenseEntity", TripDetail, "bearer", token.Token);
            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }

            await App.Current.MainPage.DisplayAlert(Languages.Ok, response.Message, Languages.Accept);
            await _navigationService.GoBackAsync();
        }

        private async void ChangeImageAsync()
        {
            await CrossMedia.Current.Initialize();

            string source = await Application.Current.MainPage.DisplayActionSheet(
                Languages.PictureSource,
                Languages.Cancel,
                null,
                Languages.FromGallery,
                Languages.FromCamera);

            if (source == Languages.Cancel)
            {
                _file = null;
                return;
            }

            if (source == Languages.FromCamera)
            {
                _file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Expense",
                        Name = "expense.jpg",
                        SaveToAlbum = true,
                        CompressionQuality = 75,
                        CustomPhotoSize = 50,
                        PhotoSize = PhotoSize.Small,
                        DefaultCamera = CameraDevice.Front
                    }
                );
            }
            else
            {
                _file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (_file != null)
            {
                Image = ImageSource.FromStream(() =>
                {
                    System.IO.Stream stream = _file.GetStream();
                    return stream;
                });
            }
        }

        private async Task<bool> ValidateDataAsync()
        {
            if (string.IsNullOrEmpty(TripDetail.Description))
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.DocumentError, Languages.Accept);
                return false;
            }

            if (TripDetail.Amount < 1)
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.FirstNameError, Languages.Accept);
                return false;
            }

            if (_file == null)
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.FirstNameError, Languages.Accept);
                return false;
            }

            return true;
        }
    }
}
