using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProductApp.Shared.Models
{
    public class Record
    {
        public Record()
        {
            Id = Guid.NewGuid().ToString();
            CreatedDate = DateTime.UtcNow;
            ModifiedDate = DateTime.UtcNow; 
        }

        [Key]
        public string Id { get; set; }

       // [JsonIgnore]
        public DateTime CreatedDate { get; set; }
      //  [JsonIgnore]
        public DateTime ModifiedDate { get; set; }

      //  [JsonIgnore]
        public bool IsDeleted { get; set; }
    }
}
