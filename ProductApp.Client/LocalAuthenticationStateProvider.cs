using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using ProductApp.Client.Models;
using ProductApp.Shared.Models;

namespace ProductApp.Client
{
    public class LocalAuthenticationStateProvider : AuthenticationStateProvider
    {

        private readonly ILocalStorageService _storageService;

        public LocalAuthenticationStateProvider(ILocalStorageService storageService)
        {
            _storageService = storageService;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
          if (await _storageService.ContainKeyAsync("User"))
            {
                var userInfo = await _storageService.GetItemAsync<LocalUserInfo>("User");

                var listRoleClaims = new List<Claim>();
                foreach (var claimRole in userInfo.Roles)
                    listRoleClaims.Add(new Claim(ClaimTypes.Role, claimRole));

                var claims = new List<Claim>
                {
                    new Claim("Email", userInfo.Email),
                    new Claim("FirstName", userInfo.FirstName),
                    new Claim("LastName", userInfo.LastName),
                    new Claim("AccessToken", userInfo.AccessToken),
                    new Claim(ClaimTypes.NameIdentifier, userInfo.Id),
                };

                claims.AddRange(listRoleClaims);

                var identity = new ClaimsIdentity(claims.ToArray(), "BearerToken");
                var user = new ClaimsPrincipal(identity);
                var state =  new AuthenticationState(user);
                NotifyAuthenticationStateChanged(Task.FromResult(state));
                return state;
            }
            return new AuthenticationState(new ClaimsPrincipal());
        }

        public async Task LogoutAsync()
        {
            await _storageService.RemoveItemAsync("User");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
        }
    }
}
