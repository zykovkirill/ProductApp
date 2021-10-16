using AKSoftware.WebApi.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProductApp.Shared.Models.UserData;
using ProductApp.Shared.Models;

namespace ProductApp.WebClient.Services
{
    //TODO: Объеденить в единый админ сервис
    public class UserDataService
    {
        private readonly string _baseUrl;

        ServiceClient client = new ServiceClient();

        public UserDataService(string url)
        {
            _baseUrl = url;
        }

        public string AccessToken
        {
            get => client.AccessToken;
            set
            {
                client.AccessToken = value;
            }
        }
        /// <summary>
        /// Получить профиль пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResponse<UserProfile>> GetUserDataAsync()
        {
            var response = await client.GetProtectedAsync<OperationResponse<UserProfile>>($"{_baseUrl}/api/userdata");
            return response.Result;
        }


        ///// <summary>
        ///// Получить профиль пользователя
        ///// </summary>
        ///// <returns></returns>
        //public async Task<OperationResponse<UserProfile>> GetUserCartAsync()
        //{
        //    var response = await client.GetProtectedAsync<OperationResponse<UserProfile>>($"{_baseUrl}/api/userdata");
        //    return response.Result;
        //}


        ///// <summary>
        ///// Получить все продукты по id
        ///// </summary>
        ///// <param name="id"> ID продукта </param>
        ///// <returns></returns>
        //public async Task<OperationResponse<ChangeRoleViewModel>> GetUsersByIdAsync(string id)
        //{
        //    var response = await client.GetProtectedAsync<OperationResponse<ChangeRoleViewModel>>($"{_baseUrl}/api/users/edit?id={id}");
        //    return response.Result;
        //}

        ///// <summary>
        ///// Редактировать пользователя  с помощью API
        ///// </summary>
        ///// <param name="model"> Обьект для добавления представляющий продукт </param>
        ///// <returns></returns>
        //public async Task<UserManagerResponse> EditUserAsync(ChangeRoleViewModel model)
        //{
        //    var response = await client.PutProtectedAsync<UserManagerResponse>($"{_baseUrl}/api/users", model);
        //    return response.Result;
        //}
    }
}
