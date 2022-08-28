namespace ProductApp.Shared.Models.UserData
{
    /// <summary>
    /// Продукт в заказе
    /// </summary>
    public class UserOrderProduct : BaseProduct, ICountable
    {
        public UserOrderProduct()
        {

        }
        public UserOrderProduct(BaseProduct baseProduct, int count, string editedUser)
        {
            this.Count = count;
            this.CoverPath = baseProduct.CoverPath;
            this.Name = baseProduct.Name;
            this.Price = baseProduct.Price;
            this.ProductKind = baseProduct.ProductKind;
            this.ProductId = baseProduct.Id;
            this.EditedUser = editedUser;
        }
        /// <summary>
        /// Колличество
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Идентификатор продукта
        /// </summary>
        public string ProductId { get; set; }
    }
}
