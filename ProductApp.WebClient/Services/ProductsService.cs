using AKSoftware.WebApi.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProductApp.Shared.Models;
using ProductApp.Shared.Models.UserData;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace ProductApp.WebClient.Services
{
    public class ProductsService
    {
        private readonly string _baseUrl;

        ServiceClient client = new ServiceClient();

        public ProductsService(string url)
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
        /// Получить все продукты с помощью API
        /// </summary>
        /// <param name="page"> Номер страницы </param>
        /// <returns></returns>
        public async Task<CollectionPagingResponse<Product>> GetAllProductsByPageAsync(int page =1 )
        {
           // var str = page.ToString();
            var response = await client.GetProtectedAsync<CollectionPagingResponse<Product>>($"{_baseUrl}/api/products?page={page}");
            return response.Result;
        }

        /// <summary>
        /// Получить все продукты с помощью API
        /// </summary>
        /// <param name="page"> Номер страницы </param>
        /// <param name="typeFilter"> Фильтр типов продуктов</param>
        /// <returns></returns>
        public async Task<CollectionPagingResponse<Product>> GetFilterProductsByPageAsync( int page = 1, string typeFilter="")
        {
            // var str = page.ToString();
            var response = await client.GetProtectedAsync<CollectionPagingResponse<Product>>($"{_baseUrl}/api/products/filter?filter={typeFilter}&page={page}");
            return response.Result;
        }

        /// <summary>
        /// Получить все продукты по id
        /// </summary>
        /// <param name="id"> ID продукта </param>
        /// <returns></returns>
        public async Task<OperationResponse<Product>> GetProductByIdAsync(string id)
        {
            var response = await client.GetProtectedAsync<OperationResponse<Product>>($"{_baseUrl}/api/products/edit?id={id}");
            return response.Result;
        }

        /// <summary>
        /// Получить все продукты с помощью API
        /// </summary>
        /// <param name="page"> Номер страницы </param>
        /// <returns></returns>
        public async Task<CollectionPagingResponse<Product>> SearchProductsByPageAsync(string query, int page = 1)
        {
            var response = await client.GetProtectedAsync<CollectionPagingResponse<Product>>($"{_baseUrl}/api/products/search?query={query}&page={page}");
            return response.Result;
        }

        /// <summary>
        /// Отправить продукт  с помощью API
        /// </summary>
        /// <param name="model"> Обьект для добавления представляющий продукт </param>
        /// <returns></returns>
        public async Task<OperationResponse<Product>> PostProductAsync(ProductRequestClient model)
        {
            var formKeyValues = new List<FormKeyValue>()
            {
                new StringFormKeyValue("Name", model.Name),
                new StringFormKeyValue("Description", model.Description),
                new StringFormKeyValue("Price", model.Price.ToString()),
                new StringFormKeyValue("ProductType", model.ProductType.ToString())
            };
            if (model.CoverFile != null)
                formKeyValues.Add(new FileFormKeyValue("CoverFile", model.CoverFile, model.FileName));



            var response = await client.SendFormProtectedAsync<OperationResponse<Product>>($"{_baseUrl}/api/products", ActionType.POST, formKeyValues.ToArray());
            return response.Result;
        }

        /// <summary>
        /// Редактировать продукт  с помощью API
        /// </summary>
        /// <param name="model"> Обьект для добавления представляющий продукт </param>
        /// <returns></returns>
        public async Task<OperationResponse<Product>> EditProductAsync(ProductRequestClient model)
        {
            var formKeyValues = new List<FormKeyValue>()
            {
                new StringFormKeyValue("Id", model.Id), 
                new StringFormKeyValue("Name", model.Name),
                new StringFormKeyValue("Description", model.Description),
                new StringFormKeyValue("Price", model.Price.ToString()),
                new StringFormKeyValue("ProductType", model.ProductType.ToString())
            };

            if (model.CoverFile != null)
                formKeyValues.Add(new FileFormKeyValue("CoverFile", model.CoverFile, model.FileName));
            var response = await client.SendFormProtectedAsync<OperationResponse<Product>>($"{_baseUrl}/api/products", ActionType.PUT, formKeyValues.ToArray() );
            return response.Result;
        }

        /// <summary>
        /// Удалить продукт  с помощью API
        /// </summary>
        /// <param name="id"> Обьект для добавления представляющий продукт </param>
        /// <returns></returns>
        public async Task<OperationResponse<Product>> DeleteProductAsync(string id)
        {
            var response = await client.DeleteProtectedAsync<OperationResponse<Product>>($"{_baseUrl}/api/products/{id}");
            return response.Result;
        }

        /// <summary>
        /// Добавление заказа
        /// </summary>
        public async Task<OperationResponse<UserOrder>> AddOrderAsync(UserOrder userOrder)
        {
            //TODO: Переписать используя этот запрос вместо await client.PostProtectedAsync<OperationResponse<UserOrder>>($"{_baseUrl}/api/usercart", userOrder);
            using (var client1 = new HttpClient())
            {
                client1.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",client.AccessToken); 
                var response1 = await client1.PostAsJsonAsync($"{_baseUrl}/api/usercart", userOrder);
                response1.EnsureSuccessStatusCode();
                return await response1.Content.ReadAsAsync<OperationResponse<UserOrder>>();
            }

           // var response = await client.PostProtectedAsync<OperationResponse<UserOrder>>($"{_baseUrl}/api/usercart", userOrder);
          //  return response.Result;
        }

        /// <summary>
        /// Получить  корзину с продуктами 
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResponse<UserOrder>> GetUserOrderAsync()
        {
            //TODO: Создать единый контроллер UserOrder вместо usercart и userpurchases
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
            //TODO: Создать отдельный класс PurchaseService вынести его из ProductService
            //TODO: Изображение сохраняется не с тем размеров в ПРОДУКТАХ ПОЛЬЗОВАТЕЛЯ!!!!! КРИТТТТТ!!!!
            var response = await client.GetProtectedAsync<CollectionPagingResponse<UserOrder>>($"{_baseUrl}/api/userpurchases?page={page}");
            return response.Result;
        }

        public async Task<CollectionPagingResponse<UserCreatedProduct>> GetAllUserProductsByPageAsync(int page = 1)
        {
            // var str = page.ToString();
            var response = await client.GetProtectedAsync<CollectionPagingResponse<UserCreatedProduct>>($"{_baseUrl}/api/userproducts?page={page}");
            return response.Result;
        }
    }
}
