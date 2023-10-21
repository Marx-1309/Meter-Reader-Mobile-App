using AutoMapper;
using Newtonsoft.Json;
using SampleMauiMvvmApp.API_URL_s;
using SampleMauiMvvmApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace SampleMauiMvvmApp.Services
{
    public partial class AuthenticationService : BaseService
    {
        //public static string HOST = ListOfUrl.TnWifi;


        HttpClient _httpClient;
        public static string BaseAddress = DeviceInfo.Platform == DevicePlatform.Android ? Constants.HOST : Constants.HOST;
        private readonly IMapper _mapper;
        public AuthenticationService(DbContext dbContext) : base(dbContext)
        {
            _httpClient = new() { BaseAddress = new Uri(BaseAddress) };

        }

        public async Task<AuthResponseModel> Login(LoginModel loginModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/login", loginModel);
                response.EnsureSuccessStatusCode();
                //Save Data of the loggedIn User
                LoginHistory loggedInUser = new()
                {
                    Username = loginModel.Username,
                    loginDate = DateTime.Now.ToLongDateString(),
                };
                await dbContext.Database.InsertAsync(loggedInUser);

                StatusMessage = "Login Successful";

                return JsonConvert.DeserializeObject<AuthResponseModel>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                StatusMessage = "Failed to login successfully.";
                return new AuthResponseModel();
            }
        }

        public async Task SetAuthToken()
        {
            var token = await SecureStorage.GetAsync("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}