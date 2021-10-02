using System.ComponentModel.DataAnnotations;

namespace ProductApp.Shared.Models
{
    public class Product : Record
    {
        /// <summary>
        /// Наименование
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        [StringLength(200)]
        public string Description { get; set; }

        //TODO: сделать set privat и колличество
        /// <summary>
        /// Цена
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// Путь к файлу
        /// </summary>
        [Required]
        [StringLength(256)]
        public string CoverPath { get; set; }

        /// <summary>
        /// Тип продукта
        /// </summary>
        [Required]
        public int ProductType { get; set; }

    }
}
