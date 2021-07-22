using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApp.Server.Models
{
    //TODO: Объеденить с Shared
    /// <summary>Статус</summary>
    public enum Status
    {
        /// <summary>Неизвестно что</summary>
        None = 0,
        /// <summary>Доставлено</summary>
        Ok = 1,
        /// <summary>Ошибка</summary>
        Error = 2,
        /// <summary>Доставка</summary>
        Delivery = 3
    }

    /// <summary>Роли</summary>
    public enum Roles
    {
        /// <summary>Неизвестно что</summary>
        None = 0,
        /// <summary>Администратор</summary>
        Admin = 1,
        /// <summary>Модератор</summary>
        Moderator = 2,
        /// <summary>Пользователь</summary>
        User = 3
    }

    /// <summary>Тип</summary>
    public enum Type
    {
        /// <summary>Неизвестно что</summary>
        None = 0,
        /// <summary>Продукт</summary>
        Product = 1,
        /// <summary>Продукт пользователя</summary>
        UserProduct = 2,
    }
}
