using Microsoft.AspNetCore.Http;

namespace ProductApp.Shared.Models
{

    public class UserProductRequest
    {
        /// <summary>
        /// ID 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// ID игрушки
        /// </summary>
        public string ToyProductId { get; set; }
        /// <summary>
        /// ID шеврона
        /// </summary>
        public string ChevronProductId { get; set; }
        /// <summary>
        /// Размер
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// Координата X
        /// </summary>
        public string X { get; set; }
        /// <summary>
        /// Координата Y
        /// </summary>
        public string Y { get; set; }
        /// <summary>
        /// Наименование файла
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Файл 
        /// </summary>
        public IFormFile CoverFile { get; set; }


    }
}

