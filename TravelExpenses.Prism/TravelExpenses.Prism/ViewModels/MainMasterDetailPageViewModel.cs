using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    public class MainMasterDetailPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private UserResponse _user;
        private DelegateCommand _modifyUserCommand;
        private static MainMasterDetailPageViewModel _instance;

        public MainMasterDetailPageViewModel(INavigationService navigationService, IApiService apiService)
            : base(navigationService)
        {
            _instance = this;
            _navigationService = navigationService;
            this._apiService = apiService;
            LoadUser();
            LoadMenus();
        }

        public DelegateCommand ModifyUserCommand => _modifyUserCommand ?? (_modifyUserCommand = new DelegateCommand(ModifyUserAsync));

        public UserResponse User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public static MainMasterDetailPageViewModel GetInstance()
        {
            return _instance;
        }

        public async void ReloadUser()
        {
            string url = App.Current.Resources["UrlAPI"].ToString();
            bool connection = await _apiService.CheckConnectionAsync(url);
            if (!connection)
            {
                return;
            }

            UserResponse user = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            EmailRequest emailRequest = new EmailRequest
            {
                CultureInfo = Languages.Culture,
                Email = user.Email
            };

            Response response = await _apiService.GetUserByEmail(url, "api", "/Account/GetUserByEmail", "bearer", token.Token, emailRequest);
            UserResponse userResponse = (UserResponse)response.Result;
            Settings.User = JsonConvert.SerializeObject(userResponse);
            LoadUser();
        }

        private async void ModifyUserAsync()
        {
            await _navigationService.NavigateAsync($"/MainMasterDetailPageViewModel/NavigationPage/{nameof(ModifyUserPage)}");
        }

        private void LoadUser()
        {
            if (Settings.IsLogin)
            {
                User = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            }
        }

        public ObservableCollection<MenuItemViewModel> Menus { get; set; }

        private void LoadMenus()
        {
            var menus = new List<Menu>
            {
                new Menu
                {
                    Icon = "ic_trips",
                    PageName = "PetsPage",
                    Title = "My Trips"
                },

                new Menu
                {
                    Icon = "ic_add_trips",
                    PageName = "AgendaPage",
                    Title = "Add Trip"
                },

                new Menu
                {
                    Icon = "ic_edit_user",
                    PageName = "ProfilePage",
                    Title = "Modify Profile"
                },

                new Menu
                {
                    Icon = "ic_exit",
                    PageName = "LoginPage",
                    Title = "Logout"
                }
            };

            Menus = new ObservableCollection<MenuItemViewModel>(
                menus.Select(m => new MenuItemViewModel(_navigationService)
                {
                    Icon = m.Icon,
                    PageName = m.PageName,
                    Title = m.Title
                }).ToList());
        }
    }
}
