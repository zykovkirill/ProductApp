namespace ProductApp.Shared.Models
{
    /// <summary>Статус заказа</summary>
    public enum OrderStatus
    {
        /// <summary>Неизвестно что</summary>
        None = 0,
        /// <summary>Продукты у покупателя</summary>
        Ok = 1,
        /// <summary>Ошибка</summary>
        Error = 2,
        /// <summary>Продукты в процессе доставки</summary>
        Delivery = 5,
        /// <summary>Продукты в корзине</summary>
        Cart = 3,
        /// <summary>Продукты оплачены</summary>
        Buy = 4,
        /// <summary>Продукты возвращены</summary>
        Return = 6,
        /// <summary>Отмена</summary>
        Cancel = 7,
    }

    ///// <summary>
    ///// Типы продуктов связаны с ProductType в Utils
    ///// </summary>
    //public enum ProductTypeEnum
    //{
    //    /// <summary>Неизвестно что</summary>
    //    None = 0,
    //    /// <summary>Игрушка</summary>
    //    Toy = 1,
    //    /// <summary>Шеврон</summary>
    //    Сhevron = 2
    //}

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
