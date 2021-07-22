using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace ProductApp.Shared.Models
{

    //TODO: сделать через OperationResponse
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

        /// <summary>
        /// Тип продукта
        /// </summary>
        [Required]
        public int ProductType { get; set; }


    }
}

