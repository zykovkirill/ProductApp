using Microsoft.AspNetCore.Http;

namespace ProductApp.Shared.Models
{
    public class ProductRequestServer : ProductRequest
    {

        public IFormFile CoverFile { get; set; }


    }
}

