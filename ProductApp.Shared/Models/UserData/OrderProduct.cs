using System;
using System.Collections.Generic;
using System.Text;

namespace ProductApp.Shared.Models.UserData
{
    public class OrderProduct : Record
    {

        public string ProductId { get; set; }
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
