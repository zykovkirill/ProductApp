using System.ComponentModel.DataAnnotations;

namespace ProductApp.Shared.Models
{
    public class Product : BaseProduct
    {
        /// <summary>
        /// Описание
        /// </summary>
        [StringLength(200)]
        public string Description { get; set; }

        /// <summary>
        /// Тип продукта
        /// </summary>
        [Required]
        public int ProductType { get; set; }

    }
}
