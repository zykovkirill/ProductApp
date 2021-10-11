using System;
using System.Collections.Generic;
using System.Text;

namespace ProductApp.Shared.Models
{
    public class ProductCountable<T>
    {
        public  T Product { get; set; }
        public int Count { get; set; }
    }
}
