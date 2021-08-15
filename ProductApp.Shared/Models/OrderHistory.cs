using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProductApp.Shared.Models
{
    /// <summary>
    /// История изменения статусов заказа
    /// </summary>
    public class OrderHistory : Record
    {

        public string IdOrder { get; set; }

        public Status Status { get; set; }
    }
}
