using System.ComponentModel.DataAnnotations;

namespace ProductApp.Shared.Models
{
    /// <summary>
    /// Тип продукта
    /// </summary>
    public class ProductType : Record
    {
        /// <summary>
        /// Наименование типа продукта
        /// </summary>
        [Required]
        [StringLength(50)]
        public string TypeName { get; set; }

        /// <summary>
        /// Путь к файлу
        /// </summary>
        //[Required]
        [StringLength(256)]
        public string CoverPath { get; set; }

    }
}
