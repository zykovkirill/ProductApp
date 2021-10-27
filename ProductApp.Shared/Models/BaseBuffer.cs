using System;
using System.Collections.Generic;
using System.Text;

namespace ProductApp.Shared.Models
{
    public class BaseBuffer<T>
    {
        public string Id { get; set; }
        public T Entity { get; set; }
    }
}
