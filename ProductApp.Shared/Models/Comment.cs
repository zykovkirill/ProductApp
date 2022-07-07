using System.ComponentModel.DataAnnotations;

namespace ProductApp.Shared.Models
{
    public class Comment : Record
    {
        public string UserName { get; set; }
        [StringLength(256)]
        public string UserComment { get; set; }
    }
}
