using AKSoftware.WebApi.Client;
using ProductApp.Shared.Models;
using ProductApp.Shared.Models.UserData;
using System.Threading.Tasks;

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


        /// <summary>
        /// Добавление заказа
        /// </summary>
        public async Task<OperationResponse<UserOrder>> AddOrderAsync(UserOrder userOrder)
        {
            //TODO: Переписать используя этот запрос вместо await client.PostProtectedAsync<OperationResponse<UserOrder>>($"{_baseUrl}/api/usercart", userOrder);
            //using (var client1 = new HttpClient())
            //{
            //    client1.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", client.AccessToken);
            //    var response1 = await client1.PostAsJsonAsync($"{_baseUrl}/api/usercart", userOrder);
            //    response1.EnsureSuccessStatusCode();
            //    return await response1.Content.ReadAsAsync<OperationResponse<UserOrder>>();
            //}

            var response = await client.PostProtectedAsync<OperationResponse<UserOrder>>($"{_baseUrl}/api/usercart", userOrder);
            return response.Result;
        }

        /// <summary>
        /// Получить  корзину с продуктами 
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResponse<UserOrder>> GetUserOrderAsync()
        {
            var response = await client.GetProtectedAsync<OperationResponse<UserOrder>>($"{_baseUrl}/api/usercart/GetProductFromCart");
            return response.Result;
        }

        //TODO : Объеденить запросы GetUserOrderAsync и GetPurchasesAsync добавить в параметр тип статус основная проблемма в CollectionPagingResponse
        /// <summary>
        /// Получить  заказы и их статус
        /// </summary>
        /// <returns></returns>
        public async Task<CollectionPagingResponse<UserOrder>> GetPurchasesAsync(int page)
        {
            //TODO: Изображение сохраняется не с тем размеров в ПРОДУКТАХ ПОЛЬЗОВАТЕЛЯ!!!!! КРИТТТТТ!!!!
            var response = await client.GetProtectedAsync<CollectionPagingResponse<UserOrder>>($"{_baseUrl}/api/UserPurchases?page={page}");
            return response.Result;
        }
    }
}
