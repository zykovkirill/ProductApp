using AKSoftware.WebApi.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProductApp.Shared.Models;
using ProductApp.Shared.Models.UserData;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ProductApp.Shared.Services
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
                //TODO: правильно ли преобразовывать в int в string
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
                     //TODO: правильно ли преобразовывать в int в string
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
        /// Добавить в корзину продукт
        /// </summary>
        /// <param name="id"> Обьект для добавления представляющий продукт </param>
        /// <param name="count"> кол-во </param>
        /// <param name="classType"> тип ClassType => см. в Enum</param>
        /// <returns></returns>
        public async Task<OperationResponse<Product>> AddProductToCartAsync(int count, string id, int classType)
        {
                var response = await client.GetProtectedAsync<OperationResponse<Product>>($"{_baseUrl}/api/usercart/addprod?count={count}&id={id}&classType={classType}");
                return response.Result;

        }
        /// <summary>
        /// Добавить в корзину продукт пользователя
        /// </summary>
        /// <param name="id"> Обьект для добавления представляющий продукт </param>
        /// <param name="count"> кол-во </param>
        /// <param name="classType"> тип ClassType => см. в Enum</param>
        /// <returns></returns>
        public async Task<OperationResponse<UserProduct>> AddUserProductToCartAsync(int count, string id, int classType)
        {          
                var response = await client.GetProtectedAsync<OperationResponse<UserProduct>>($"{_baseUrl}/api/usercart/addprod?count={count}&id={id}&classType={classType}");
                return response.Result;

        }


        /// <summary>
        /// Покупка всего что в корзине
        /// </summary>
        ///// <param name="id"> Обьект для добавления представляющий продукт </param>
        ///// <param name="count"> кол-во </param>
        ///// <param name="classType"> тип ClassType => см. в Enum</param>
        /// <returns></returns>
        public async Task<OperationResponse<UserProductInCart>> BuyProductAsync(UserProductInCart products)
        {
            //var response = await client.GetProtectedAsync<OperationResponse<UserProduct>>($"{_baseUrl}/api/usercart/addprod?count={count}&id={id}&classType={classType}");
            //  return response.Result;
            return null;
        }



        /// <summary>
        /// Получить  корзину с продуктами 
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResponse<UserCart>> GetProductFromCartAsync()
        {
            var response = await client.GetProtectedAsync<OperationResponse<UserCart>>($"{_baseUrl}/api/usercart/GetProductFromCart");
            return response.Result;
        }
        /// <summary>
        /// Удалить продукт  с помощью API
        /// </summary>
        /// <param name="id"> Обьект для добавления представляющий продукт </param>
        /// <returns></returns>
        public async Task<OperationResponse<UserProductInCart>> DeleteProductFromCartAsync(string id)
        {
            var response = await client.DeleteProtectedAsync<OperationResponse<UserProductInCart>>($"{_baseUrl}/api/usercart/{id}");
            return response.Result;
        }
        ///// <summary>
        ///// Сохранить
        ///// </summary>
        ///// <returns></returns>
        //public async Task<OperationResponse<UserProduct>> SaveUserProductAsync(UserProduct model)
        //{

        //    //var response = await client.PostProtectedAsync<OperationResponse<UserProduct>>($"{_baseUrl}/api/userproducts", model);
        //    //return response.Result;
        //    return null;
        //}

        public async Task<CollectionPagingResponse<UserProduct>> GetAllUserProductsByPageAsync(int page = 1)
        {
            // var str = page.ToString();
            var response = await client.GetProtectedAsync<CollectionPagingResponse<UserProduct>>($"{_baseUrl}/api/userproducts?page={page}");
            return response.Result;
        }
        public async Task<FileStreamResult> GetStreamImage(string imgID)
        {
            
            var response = await client.GetProtectedAsync<object>($"{_baseUrl}/api/userproducts/img?imgID={imgID}");
            Stream test = await response.HttpResponse.Content.ReadAsStreamAsync();
            
            //   Stream test1 = await response.HttpResponse.Content.ReadAsStreamAsync();
            return null;
        //    return response.Result;
        }

    }
}
