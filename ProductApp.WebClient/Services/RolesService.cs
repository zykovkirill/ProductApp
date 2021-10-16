using AKSoftware.WebApi.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProductApp.Shared.Models;

namespace ProductApp.WebClient.Services
{
    public class RolesService
    {
        private readonly string _baseUrl;

        ServiceClient client = new ServiceClient();

        public RolesService(string url)
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
        /// Получить все роли с помощью API
        /// </summary>
        /// <returns></returns>
        public async Task<CollectionResponse<RoleViewModel>> GetAllRolesByPageAsync()
        {
            var response = await client.GetProtectedAsync<CollectionResponse<RoleViewModel>>($"{_baseUrl}/api/roles");
            return response.Result;
        }

        /// <summary>
        /// Получить все роли с помощью API
        /// </summary>
        /// <param name="page"> Номер страницы </param>
        /// <returns></returns>
        public async Task<CollectionResponse<RoleViewModel>> SearchRolesByPageAsync(string query, int page = 1)
        {
            var response = await client.GetProtectedAsync<CollectionResponse<RoleViewModel>>($"{_baseUrl}/api/roles/search?query={query}&page={page}");
            return response.Result;
        }

        /// <summary>
        /// Отправить роль  с помощью API
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResponse<RoleViewModel>> PostRoleAsync(RoleViewModel model)
        {

            var response = await client.PostProtectedAsync<OperationResponse<RoleViewModel>>($"{_baseUrl}/api/roles", model);
            return response.Result;


        }

        /// <summary>
        /// Удалить роль  с помощью API
        /// </summary>
        /// <param name="id"> Обьект для добавления представляющий роль </param>
        /// <returns></returns>
        public async Task<OperationResponse<RoleViewModel>> DeleteRoleAsync(string id)
        {
            var response = await client.DeleteProtectedAsync<OperationResponse<RoleViewModel>>($"{_baseUrl}/api/roles/{id}");
            return response.Result;
        }
    }
}
