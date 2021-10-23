using ProductApp.Shared.Models.UserData;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductApp.Shared.Models
{
    public class ProductInfo : Record
    {
        /// <summary>
        /// Коментарии 
        /// </summary>
        public  List<Comment> Comments { get; set; }
        public int CountLike { get; set; }
        public int CountDislike { get; set; }
        public string ProductId { get; set; }
        public ProductInfo()
        {
            Comments = new List<Comment>();
        }

    }
}
