using ProductApp.Shared.Models;
using System.Collections.Generic;

public class ChangeRoleViewModel
{
    public string UserId { get; set; }
    public string UserEmail { get; set; }

    public IList<UserRoleViewModel> Roles { get; set; }
    public ChangeRoleViewModel()
    {
        Roles = new List<UserRoleViewModel>();
    }
}