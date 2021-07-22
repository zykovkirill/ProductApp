using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
