using ProductApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ProductApp.Shared.Models.UserData
{
    /// <summary>
    /// Корзина
    /// </summary>
    //TODO: Может переименовать в UserOrder?
    public class UserCart : Record
    {

       // public int Id { get; set; }
        public string UserId { get; set; }
        public IList<UserProductInCart>  Products { get; set; }
        /// <summary>
        /// Колличество
        /// </summary>
        public int ProductCount { get; set; }
        /// <summary>
        /// Общая цена
        /// </summary>
        public int TotalSum { get; set; }

        //public int UserDataId { get; set; }      // внешний ключ
        //public UserProfile UserData { get; set; }    // навигационное свойство

        public UserCart()
        {
            Products = new List<UserProductInCart>();
        }
    
    }
}
