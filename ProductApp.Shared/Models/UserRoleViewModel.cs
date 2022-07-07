using System.ComponentModel.DataAnnotations;

namespace ProductApp.Shared.Models
{
    public class UserRoleViewModel
    {

        public UserRoleViewModel(bool isOn, string name)
        {
            IsOn = isOn;
            RoleName = name;
        }

        public bool IsOn { get; set; }
        [StringLength(50)]
        public string RoleName { get; set; }
    }
}
