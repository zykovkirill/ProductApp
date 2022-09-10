using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ProductApp.WebClient.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tewr.Blazor.FileReader;

namespace ProductApp.WebClient
{
    public class Program
    {
        private const string URL = "http://192.168.1.7:1485";//RELEASE /*http://188.235.156.133:1485*/
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddScoped<AuthenticationService>(s =>
            {
                return new AuthenticationService(URL);
            });
            builder.Services.AddScoped<ProductsService>(s =>
            {
                return new ProductsService(URL);
            });

            builder.Services.AddScoped<ProductTypesService>(s =>
            {
                return new ProductTypesService(URL);
            });

            builder.Services.AddScoped<UserDataService>(s =>
            {
                return new UserDataService(URL);
            });

            builder.Services.AddScoped<UsersService>(s =>
            {
                return new UsersService(URL);
            });
            builder.Services.AddScoped<RolesService>(s =>
            {
                return new RolesService(URL);
            });
            builder.Services.AddFileReaderService(options =>
            {
                options.UseWasmSharedBuffer = true;
            });
            builder.Services.AddBlazoredSessionStorage();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, LocalAuthenticationStateProvider>();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
