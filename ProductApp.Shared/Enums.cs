using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApp.Shared.Models
{
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

    //TODO: убрать или переделать
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
    //TODO: как лучше хранить в БД инфу в string или с помощью
    /// <summary>
    /// Типы продуктов связаны с ProductType в Utils
    /// </summary>
    public enum ProductTypeEnum
    {
        /// <summary>Неизвестно что</summary>
        None = 0,
        /// <summary>Игрушка</summary>
        Toy = 1,
        /// <summary>Шеврон</summary>
        Сhevron = 2
    }

    /// <summary>Тип</summary>
    public enum ClassType
    {
        /// <summary>Неизвестно что</summary>
        None = 0,
        /// <summary>Продукт</summary>
        Product = 1,
        /// <summary>Продукт пользователя</summary>
        UserProduct = 2,
    }
}
