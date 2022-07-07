using System.Collections.Generic;

namespace ProductApp.Shared.Models
{
    /// <summary>
    /// Информация о продукте 
    /// </summary>
    public class ProductInfo : Record
    {
        /// <summary>
        /// Коментарии 
        /// </summary>
        public List<Comment> Comments { get; set; }
        /// <summary>
        /// Рейтинг
        /// </summary>
        public List<Rating> Ratings { get; set; }
        public string ProductId { get; set; }
        public ProductInfo()
        {
            Comments = new List<Comment>();
            Ratings = new List<Rating>();
        }

    }
}
