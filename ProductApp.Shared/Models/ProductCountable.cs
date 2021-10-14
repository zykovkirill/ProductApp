using System;
using System.Collections.Generic;
using System.Text;

namespace ProductApp.Shared.Models
{
    //TODO: Удалить
    public class ProductCountable<T> : Record
    {

        public  T Product { get; set; }
        public int Count { get; set; }
    }
}
