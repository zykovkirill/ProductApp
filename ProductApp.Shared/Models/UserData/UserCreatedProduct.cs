using ProductApp.Shared;
using ProductApp.Shared.Models.UserData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApp.Shared.Models.UserData
{
    public class UserCreatedProduct : Record
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }
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

        /// <summary>
        /// Путь к файлу
        /// </summary>
        [Required]
        [StringLength(256)]
        public string CoverPath { get; set; }

        // public int UserDataId { get; set; }      // внешний ключ
        // public UserProfile UserData { get; set; }    // навигационное свойство

    }
    //public class UserProductRequest 
    //{
    //    /// <summary>
    //    /// ID игрушки
    //    /// </summary>
    //    public string ToyProductId { get; set; }
    //    /// <summary>
    //    /// ID шеврона
    //    /// </summary>
    //    public string ChevronProductId { get; set; }
    //    /// <summary>
    //    /// Размер
    //    /// </summary>
    //    public int Size { get; set; }
    //    /// <summary>
    //    /// Координата X
    //    /// </summary>
    //    public int X { get; set; }
    //    /// <summary>
    //    /// Координата Y
    //    /// </summary>
    //    public int Y { get; set; }

    //    // public int UserDataId { get; set; }      // внешний ключ
    //    // public UserProfile UserData { get; set; }    // навигационное свойство

    //}
}
