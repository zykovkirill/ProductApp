using System;
using System.Collections.Generic;
using System.Text;

namespace ProductApp.Shared.Models
{
    public class Rating: Record
    {
        public int ProductRating { get; set; }
        public string UserId { get; set; }
    }
}
