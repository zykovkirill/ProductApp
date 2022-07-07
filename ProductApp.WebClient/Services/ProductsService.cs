using AKSoftware.WebApi.Client;
using ProductApp.Shared.Models;
using ProductApp.Shared.Models.UserData;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<CollectionPagingResponse<Product>> GetAllProductsByPageAsync(int page = 1)
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
        public async Task<CollectionPagingResponse<Product>> GetFilterProductsByPageAsync(int page = 1, string typeFilter = "")
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
        /// Получить информацию о продукте 
        /// </summary>
        /// <param name="id"> ID продукта </param>
        /// <returns></returns>
        public async Task<OperationResponse<ProductInfo>> GetProductInfoByIdAsync(string id)
        {
            var response = await client.GetProtectedAsync<OperationResponse<ProductInfo>>($"{_baseUrl}/api/productinfo/info?id={id}");
            return response.Result;
        }

        /// <summary>
        /// Добавить комментарий 
        /// </summary>
        public async Task<OperationResponse<Comment>> AddCommentAsync(BaseBuffer<Comment> model)
        {
            var response = await client.PostProtectedAsync<OperationResponse<Comment>>($"{_baseUrl}/api/productinfo/comment", model);
            return response.Result;
        }

        /// <summary>
        /// Добавить рейтинг
        /// </summary>
        public async Task<OperationResponse<Rating>> AddRatingAsync(BaseBuffer<Rating> model)
        {
            var response = await client.PostProtectedAsync<OperationResponse<Rating>>($"{_baseUrl}/api/productinfo/rating", model);
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
            var response = await client.SendFormProtectedAsync<OperationResponse<Product>>($"{_baseUrl}/api/products", ActionType.PUT, formKeyValues.ToArray());
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

        public async Task<CollectionPagingResponse<UserCreatedProduct>> GetAllUserProductsByPageAsync(int page = 1)
        {
            // var str = page.ToString();
            var response = await client.GetProtectedAsync<CollectionPagingResponse<UserCreatedProduct>>($"{_baseUrl}/api/userproducts?page={page}");
            return response.Result;
        }
    }
}
