using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ProductApp.Shared.Models
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Некорректные даные")]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Некорректные даные")]
        [StringLength(25)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Некорректные даные")]
        [StringLength(25)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Некорректные даные")]
        [StringLength(50)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Некорректные даные")]
        [StringLength(50)]
        public string ConfirmPassword { get; set; }

    }
}
