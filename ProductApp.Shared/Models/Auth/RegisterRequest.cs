using System.ComponentModel.DataAnnotations;

namespace ProductApp.Shared.Models
{
    public class RegisterRequest
    {
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Некорректное Имя")]
        [StringLength(25)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Некорректная фамилия")]
        [StringLength(25)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Некорректный пароль")]
        [StringLength(50)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Некорректнное подтверждение пароля")]
        [StringLength(50)]
        public string ConfirmPassword { get; set; }

    }
}
