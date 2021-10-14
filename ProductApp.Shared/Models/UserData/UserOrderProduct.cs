﻿using ProductApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ProductApp.Shared.Models.UserData
{
    /// <summary>
    /// Продукт в заказе
    /// </summary>
    public class UserOrderProduct:BaseProduct, ICountable
    {
        public UserOrderProduct()
        {
            
        }
        public UserOrderProduct(BaseProduct baseProduct, int count)
        {
            this.Count = count;
            this.CoverPath = baseProduct.CoverPath;
            this.Name = baseProduct.Name;
            this.Price = baseProduct.Price;
            this.ProductKind = baseProduct.ProductKind;
            this.ProductId = baseProduct.Id;
        }
        /// <summary>
        /// Колличество
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Идентификатор продукта
        /// </summary>
        public string ProductId { get; set; }
    }
}
