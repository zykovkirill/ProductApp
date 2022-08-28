using System.Collections.Generic;

namespace ProductApp.Shared.Models.UserData
{
    public class UserCreatedProduct : BaseProduct
    {
        /// <summary>
        /// ID основного продукта
        /// </summary>
        public string BaseProductId { get; set; }

        ///// <summary>
        ///// ID игрушки
        ///// </summary>
        //public string ToyProductId { get; set; }

        ///// <summary>
        ///// ID шеврона
        ///// </summary>
        //public string ChevronProductId { get; set; }

        ///// <summary>
        ///// ID бисера
        ///// </summary>
        //public string BeadsId { get; set; }

        ///// <summary>
        ///// Размер
        ///// </summary>
        //public float Size { get; set; }

        ///// <summary>
        ///// Координата X
        ///// </summary>
        //public float X { get; set; }

        ///// <summary>
        ///// Координата Y
        ///// </summary>
        //public float Y { get; set; }


        public List<IncludedProduct> IncludedProducts { get; set; } = new List<IncludedProduct>();

    }
    //TODO: мб использовать структуру ?
    public class IncludedProduct: Record
    {

        /// <summary>
        /// ID Продукта
        /// </summary>
        public string ProductID { get; set; }

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
