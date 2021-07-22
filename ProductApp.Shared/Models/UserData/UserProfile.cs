using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductApp.Shared.Models.UserData
{
    /// <summary>
    /// Данные пользователя
    /// </summary>
    public class UserProfile
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        /// <summary>
        /// Всего покупок на сумму
        /// </summary>
        public int CommonSum { get; set; }
        ///// <summary>
        ///// Корзина
        ///// </summary>
        //public UserCart UserCart { get; set; }
        public ICollection <UserProduct> UserProducts { get; set; }
        /// <summary>
        /// Покупки
        /// </summary>
        public ICollection<UserPurchase> UserPurchases { get; set; }

        ///// <summary>
        ///// Путь к файлу изображения
        ///// </summary>
        //[Required]
        //[StringLength(256)]
        //public string CoverPath { get; set; }

        public UserProfile()
        {
            UserProducts = new List<UserProduct>();
            UserPurchases = new List<UserPurchase>();
          //  UserCart = new UserCart();
        }
    }
}
