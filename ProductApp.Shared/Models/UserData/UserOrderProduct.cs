using ProductApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ProductApp.Shared.Models.UserData
{
    /// <summary>
    /// Продукт в заказе
    /// </summary>
    public class UserOrderProduct:Record
    {


        public string  ProductId { get; set; }
        /// <summary>
        /// Цена
        /// </summary>
        public int ProductPrice { get; set; }

        public string ProductCoverPath { get; set; }

        public string ProductName { get; set; }
        /// <summary>
        /// Колличество
        /// </summary>
        public int ProductCount { get; set; }
    }
}
