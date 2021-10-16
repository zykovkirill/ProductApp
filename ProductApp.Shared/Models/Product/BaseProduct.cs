using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductApp.Shared.Models
{
    /// <summary>
    /// Базовый класс для продукта
    /// </summary>
    public class BaseProduct: Record
    {
        /// <summary>
        /// Наименование
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        //TODO: сделать set privat и колличество
        /// <summary>
        /// Цена
        /// </summary>
        [Required]
        public int Price { get; set; }

        /// <summary>
        /// Путь к файлу
        /// </summary>
        [Required]
        [StringLength(256)]
        public string CoverPath { get; set; }
        /// <summary>
        /// Вид продукта
        /// </summary>
        [Required]
        [StringLength(256)]
        public int ProductKind { get; set; }

    }
}
