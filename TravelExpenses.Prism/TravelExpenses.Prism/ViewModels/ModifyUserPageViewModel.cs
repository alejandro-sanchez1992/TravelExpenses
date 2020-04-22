using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using System.Threading.Tasks;
using TravelExpenses.Common.Enums;
using TravelExpenses.Common.Helpers;
using TravelExpenses.Common.Models;
using TravelExpenses.Prism.Helpers;
using TravelExpenses.Prism.Views;
using Xamarin.Forms;
using TravelExpenses.Common.interfaces;

namespace TravelExpenses.Prism.ViewModels
{
    public class ModifyUserPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IFilesHelper _filesHelper;
        private readonly IApiService _apiService;
        private bool _isRunning;
        private bool _isEnabled;
        private ImageSource _image;
        private EmployeeResponse _user;
        private MediaFile _file;
        private DelegateCommand _changeImageCommand;
        private DelegateCommand _saveCommand;
        private DelegateCommand _changePasswordCommand;

        public ModifyUserPageViewModel(INavigationService navigationService, IFilesHelper filesHelper, IApiService apiService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _filesHelper = filesHelper;
            _apiService = apiService;
            Title = Languages.ModifyUser;
            IsEnabled = true;
            Employee = JsonConvert.DeserializeObject<EmployeeResponse>(Settings.Employee);
            Image = Employee.User.PictureFullPath;
        }

        public DelegateCommand ChangePasswordCommand => _changePasswordCommand ?? (_changePasswordCommand = new DelegateCommand(ChangePasswordAsync));

        public DelegateCommand ChangeImageCommand => _changeImageCommand ?? (_changeImageCommand = new DelegateCommand(ChangeImageAsync));

        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(SaveAsync));

        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public EmployeeResponse Employee
        {
            get => _user;
            set => SetProperty(ref _user, value);
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

        private async void ChangePasswordAsync()
        {
            await _navigationService.NavigateAsync(nameof(ChangePasswordPage));
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

            byte[] imageArray = null;
            if (_file != null)
            {
                imageArray = _filesHelper.ReadFully(_file.GetStream());
            }

            UserRequest userRequest = new UserRequest
            {
                Address = Employee.User.Address,
                Document = Employee.User.Document,
                Email = Employee.User.Email,
                FirstName = Employee.User.FirstName,
                LastName = Employee.User.LastName,
                Password = "123456", // It doesn't matter what is sent here. It is only for the model to be valid
                PhoneNumber = Employee.User.PhoneNumber,
                PictureArray = imageArray,
                UserTypeId = Employee.User.UserType == UserType.Employee ? 1 : 2,
                CultureInfo = Languages.Culture
            };

            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            string url = App.Current.Resources["UrlAPI"].ToString();
            Response<object> response = await _apiService.PutAsync(url, "/api", "/Account", userRequest, "bearer", token.Token);

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Error", response.Message, Languages.Accept);
                return;
            }

            Settings.Employee = JsonConvert.SerializeObject(Employee);
            MainMasterDetailPageViewModel.GetInstance().ReloadUser();
            await App.Current.MainPage.DisplayAlert(Languages.Ok, Languages.UserUpdated, Languages.Accept);
        }

        private async Task<bool> ValidateDataAsync()
        {
            if (string.IsNullOrEmpty(Employee.User.Document))
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.DocumentError, Languages.Accept);
                return false;
            }

            if (string.IsNullOrEmpty(Employee.User.FirstName))
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.FirstNameError, Languages.Accept);
                return false;
            }

            if (string.IsNullOrEmpty(Employee.User.LastName))
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.LastNameError, Languages.Accept);
                return false;
            }

            if (string.IsNullOrEmpty(Employee.User.Address))
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.AddressError, Languages.Accept);
                return false;
            }

            if (string.IsNullOrEmpty(Employee.User.PhoneNumber))
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.PhoneError, Languages.Accept);
                return false;
            }

            return true;
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
                        Directory = "Sample",
                        Name = "test.jpg",
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
    }
}
