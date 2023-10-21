using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
//using SampleMauiMvvmApp.Helpers;
using SampleMauiMvvmApp.Models;
using SampleMauiMvvmApp.Services;
using SampleMauiMvvmApp.Views;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SampleMauiMvvmApp.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        public LoginViewModel(AuthenticationService _authenticationService )
        {
            this.authenticationService = _authenticationService; 
        }

        [ObservableProperty]
        string username;

        [ObservableProperty]
        string password;

        private AuthenticationService authenticationService;

        [RelayCommand]

        async Task Login()
        {
            if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayLoginMessage("Invalid Login Attempt");
            }
            else
            {
                IsBusy = true;
                await Task.Delay(100);
                // Call API to attempt a login
                var loginModel = new LoginModel(username, password);
                var response = await authenticationService.Login(loginModel);

                //Display welcome message
                await DisplayLoginMessage(authenticationService.StatusMessage);
                //await Shell.Current.DisplayAlert("Success", "Login was successful", "OK");

                if (!string.IsNullOrEmpty(response.Token)) 
                {
                    //Store token in secure 
                    await SecureStorage.SetAsync("Token",response.Token);


                    //Build a menu on the fly...based on the role
                    var jsonToken = new JwtSecurityTokenHandler().ReadToken(response.Token) as
                        JwtSecurityToken;

                    var role = jsonToken.Claims.FirstOrDefault(q =>q.Type.Equals(ClaimTypes.Role))?.Value;

                    

                    App.UserInfo = new UserInfo()
                    {
                        Username = Username,
                        Role = role,
                    };


                    //You can use this to access details of the logged_in user//commented out 
                    //App.UserInfo = userInfo;

                    //Navigate to the app's main page
                    IsBusy = false;
                    await Shell.Current.GoToAsync($"//{nameof(MonthCustomerTabPage)}");;

                    
                }
                IsBusy = false;
                

            }
        }



        async Task DisplayLoginMessage(string message) 
        {
            await Shell.Current.DisplayAlert("Attempt Result", message, "OK");
            Password = string.Empty;
        }

    }
}
