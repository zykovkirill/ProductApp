using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductApp.Shared.Models
{
    public class EditUserViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Некорректное Имя")]
        [StringLength(25)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Некорректная фамилия")]
        [StringLength(25)]
        public string LastName { get; set; }
    }
}
