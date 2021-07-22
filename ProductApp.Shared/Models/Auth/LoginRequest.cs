using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ProductApp.Shared.Models
{
    public class LoginRequest
    {

        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }



    }
}
