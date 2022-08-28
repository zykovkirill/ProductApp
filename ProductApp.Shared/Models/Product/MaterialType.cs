using System.ComponentModel.DataAnnotations;

namespace ProductApp.Shared.Models
{
    /// <summary>
    /// Материал из которого сделан продукт
    /// </summary>
    //TODO: класс не где не используется 
    public class MaterialType : Record
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
