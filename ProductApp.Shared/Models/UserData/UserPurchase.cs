using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductApp.Shared.Models;



namespace ProductApp.Shared.Models.UserData
{
    /// <summary>
    ///Покупка
    /// </summary>
    public class UserPurchase 
    {
        public int Id { get; set; }
        public List <UserProductBuy> UserProductBuy { get; set; }
        public DateTime? PurchaseTime { get; set; }

        public Status Satus { get; set; }


    }
}
