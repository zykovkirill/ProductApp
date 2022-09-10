using System.ComponentModel.DataAnnotations;

namespace ProductApp.Shared.Models
{

    public class ProductRequest
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        [StringLength(100)]
        public string Description { get; set; }

        /// <summary>
        /// Наименование файла
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public int Price { get; set; }

        ///// <summary>
        ///// Тип продукта
        ///// </summary>
        //[Required]
        //public ProductType ProductType { get; set; }


        /// <summary>
        /// ID типа продукта
        /// </summary>
        [Required]
        [StringLength(50)]
        public string ProductTypeId { get; set; }


        ///// <summary>
        ///// Пользователь отправивший запрос
        ///// </summary>
        //[Required]
        //[StringLength(100)]
        //public string EditedUser { get; set; }


    }
}

