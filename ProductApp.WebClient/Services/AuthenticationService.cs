using AKSoftware.WebApi.Client;
using ProductApp.Shared.Models;
using System.Threading.Tasks;

namespace ProductApp.WebClient.Services
{
    public class AuthenticationService
    {

        private readonly string _baseUrl;

        ServiceClient client = new ServiceClient();

        public AuthenticationService(string url)
        {
            _baseUrl = url;
        }

        public async Task<UserManagerResponse> RegisterUserAsync(RegisterRequest request)
        {
            var response = await client.PostAsync<UserManagerResponse>($"{_baseUrl}/api/Auth/Register", request);
            return response.Result;
        }


        public async Task<UserManagerResponse> LoginUserAsync(LoginRequest request)
        {
            var response = await client.PostAsync<UserManagerResponse>($"{_baseUrl}/api/Auth/Login", request);
            return response.Result;
        }
    }
}
