using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductApp.Shared.Models
{
    public class RoleViewModel
    {

        public RoleViewModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
        public RoleViewModel()
        {

        }

        public string Id { get; set; }
        [StringLength(50)]
        [Required]
        public string Name { get; set; }
    }
}
