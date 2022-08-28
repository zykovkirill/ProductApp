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

        /// <summary>
        /// Дата изменения
        /// </summary>
        //  [JsonIgnore]
        public DateTime ModifiedDate { get; set; }

        //TODO: Логику для вставки id пользователя 
        /// <summary>
        /// Последний пользователь редактировавший запись
        /// </summary>
        //  [JsonIgnore]
        [Required]

        public string EditedUser { get; set; }
        //  [JsonIgnore]
        public bool IsDeleted { get; set; }
    }
}
