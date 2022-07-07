using System.Collections.Generic;


namespace ProductApp.Shared.Models.UserData
{
    /// <summary>
    /// Заказ пользователя
    /// </summary>
    public class UserOrder : Record
    {

        public string UserId { get; set; }
        public IList<UserOrderProduct> Products { get; set; }
        /// <summary>
        /// Колличество
        /// </summary>
        public int ProductCount { get; set; }
        /// <summary>
        /// Общая цена
        /// </summary>
        public int TotalSum { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        public OrderStatus Status { get; set; }
        public UserOrder()
        {
            Products = new List<UserOrderProduct>();
        }

    }
}
