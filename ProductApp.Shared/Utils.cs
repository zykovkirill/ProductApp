using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApp.Shared.Models
{

    public static class Utils
    {
        /// <summary>
        /// Возвращает название продукта 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetNameProductType(int type)
        {
            string value = String.Empty;
            bool hasValue = ProductTypeDictionary.TryGetValue(type, out value);
            if (hasValue)
                return value;
            else
                //TODO: Сделать запись в логи об ошибке
                return value;


        }

        /// <summary>
        /// Словарь типов продуктов 
        /// </summary>
        public static readonly Dictionary<int, string> ProductTypeDictionary = new Dictionary<int, string>
        {
            {1,"Игрушки"},
            {2,"Шевроны" }
        };


        /// <summary>
        /// Класс типов продуктов(вспомогательный) 
        /// </summary>
        public class ProductType
        {

            public string ProductTypeInt { get; set; }

            public string ProductTypeName { get; set; }

        };

    }

}
