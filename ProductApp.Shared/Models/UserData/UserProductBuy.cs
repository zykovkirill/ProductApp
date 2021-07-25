using ProductApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ProductApp.Shared.Models.UserData
{
    //TODO: То же самое что и UserProductInCart, надо изменить логику добавить флаг покупки
    /// <summary> 
    /// Продукт куплен
    /// </summary>
    public class UserProductBuy: Record
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
