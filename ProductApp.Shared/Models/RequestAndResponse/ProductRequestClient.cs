using System.IO;

namespace ProductApp.Shared.Models
{
    public class ProductRequestClient : ProductRequest
    {
        public Stream CoverFile { get; set; }

    }
}

