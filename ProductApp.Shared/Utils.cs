﻿using System;
using System.Collections.Generic;

namespace ProductApp.Shared.Models
{

    public static class Utils
    {
        /// <summary>
        /// Возвращает название продукта 
        /// </summary>
        /// <param name="type">Тип продукта</param>
        /// <returns></returns>
        public static string GetNameProductByType(int type)
        {
            string value = String.Empty;
            bool hasValue = ProductTypeDictionary.TryGetValue(type, out value);
            if (hasValue)
                return value;
            else
                //TODO: Сделать запись в логи об ошибке
                return value;
        }

        //TODO: Типы вынести в базу и инициализировать в скрипте развертке(таж же и создавать пользователя Admin)
        /// <summary>
        /// Словарь типов продуктов 
        /// </summary>
        public static readonly Dictionary<int, string> ProductTypeDictionary = new Dictionary<int, string>
        {
            //{0, "Ничего"} - не использовать в коде(AddProduct) есть место где 0 выступает как пустота 
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
