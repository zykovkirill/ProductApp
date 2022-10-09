using AKSoftware.WebApi.Client;
using ProductApp.Shared.Models;
using System.Threading.Tasks;

namespace ProductApp.WebClient.Services
{
    public class ProductTypesService
    {
        private readonly string _baseUrl;

        ServiceClient client = new ServiceClient();

        public ProductTypesService(string url)
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
        /// Получить все типы продуктов с помощью API
        /// </summary>
        /// <param name="page"> Номер страницы </param>
        /// <returns></returns>
        public async Task<CollectionResponse<ProductType>> GetProductTypesAsync()
        {
            // var str = page.ToString();
            var response = await client.GetProtectedAsync<CollectionResponse<ProductType>>($"{_baseUrl}/api/productTypes");
            return response.Result;
        }

         /// <summary>
        /// Добавить тип продукта
        /// </summary>
        public async Task<OperationResponse<ProductType>> AddProductTypeAsync(ProductType model)
        {

            var response = await client.PostProtectedAsync<OperationResponse<ProductType>>($"{_baseUrl}/api/productTypes", model);
            return response.Result;

        }
    }
}
