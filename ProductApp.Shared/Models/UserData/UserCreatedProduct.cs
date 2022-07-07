namespace ProductApp.Shared.Models.UserData
{
    public class UserCreatedProduct : BaseProduct
    {
        /// <summary>
        /// ID игрушки
        /// </summary>
        public string ToyProductId { get; set; }
        /// <summary>
        /// ID шеврона
        /// </summary>
        public string ChevronProductId { get; set; }
        /// <summary>
        /// Размер
        /// </summary>
        public float Size { get; set; }
        /// <summary>
        /// Координата X
        /// </summary>
        public float X { get; set; }
        /// <summary>
        /// Координата Y
        /// </summary>
        public float Y { get; set; }
    }
}
