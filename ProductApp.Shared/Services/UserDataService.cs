using AKSoftware.WebApi.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProductApp.Shared.Models.UserData;
using ProductApp.Shared.Models;

namespace ProductApp.Shared.Services
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


        /// <summary>
        /// Получить профиль пользователя
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResponse<UserProfile>> GetUserCartAsync()
        {
            var response = await client.GetProtectedAsync<OperationResponse<UserProfile>>($"{_baseUrl}/api/userdata");
            return response.Result;
        }


        /// <summary>
        /// Получить все продукты по id
        /// </summary>
        /// <param name="id"> ID продукта </param>
        /// <returns></returns>
        public async Task<OperationResponse<ChangeRoleViewModel>> GetUsersByIdAsync(string id)
        {
            var response = await client.GetProtectedAsync<OperationResponse<ChangeRoleViewModel>>($"{_baseUrl}/api/users/edit?id={id}");
            return response.Result;
        }

        /// <summary>
        /// Получить все продукты с помощью API
        /// </summary>
        /// <param name="page"> Номер страницы </param>
        /// <returns></returns>
        public async Task<CollectionPagingResponse<EditUserViewModel>> SearchUsersByPageAsync(string query, int page = 1)
        {
            var response = await client.GetProtectedAsync<CollectionPagingResponse<EditUserViewModel>>($"{_baseUrl}/api/users/search?query={query}&page={page}");
            return response.Result;
        }

        /// <summary>
        /// Отправить пользователя  с помощью API
        /// </summary>
        /// <param name="model"> Обьект для добавления представляющий пользователя </param>
        /// <returns></returns>
        public async Task<UserManagerResponse> PostUserAsync(CreateUserViewModel model)
        {
            var formKeyValues = new List<FormKeyValue>()
            {    
                new StringFormKeyValue("Email", model.Email),            
                new StringFormKeyValue("Password", model.Password),
                new StringFormKeyValue("ConfirmPassword", model.Password)

            };

            var response = await client.SendFormProtectedAsync<UserManagerResponse>($"{_baseUrl}/api/users", ActionType.POST, formKeyValues.ToArray());
            return response.Result;
        }


        /// <summary>
        /// Редактировать пользователя  с помощью API
        /// </summary>
        /// <param name="model"> Обьект для добавления представляющий продукт </param>
        /// <returns></returns>
        public async Task<UserManagerResponse> EditUserAsync(ChangeRoleViewModel model)
        {
            var response = await client.PutProtectedAsync<UserManagerResponse>($"{_baseUrl}/api/users", model);
            return response.Result;
        }

        /// <summary>
        /// Удалить пользователя  с помощью API
        /// </summary>
        /// <param name="id"> Обьект для добавления представляющий продукт </param>
        /// <returns></returns>
        public async Task<OperationResponse<EditUserViewModel>> DeleteUserAsync(string id)
        {
            var response = await client.DeleteProtectedAsync<OperationResponse<EditUserViewModel>>($"{_baseUrl}/api/users/{id}");
            return response.Result;
        }
    }
}
