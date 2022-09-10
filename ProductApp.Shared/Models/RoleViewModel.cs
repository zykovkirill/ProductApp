using System.ComponentModel.DataAnnotations;

namespace ProductApp.Shared.Models
{
    //TODO: возможно стоит наследовать от Record и записывать в бд кошда была создана роль
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
