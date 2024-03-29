﻿namespace ProductApp.Shared.Models
{
    /// <summary>
    /// История изменения статусов заказа
    /// </summary>
    public class OrderHistory : Record
    {

        public string IdOrder { get; set; }

        public OrderStatus Status { get; set; }
    }
}
