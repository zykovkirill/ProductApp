using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApp.Client.Models
{
    public class LocalUserInfo
    {
        public string Id { get; set; }
        public string FirstName {get; set;}
        public string LastName { get; set; }
        public string AccessToken { get; set; }
        public string Email { get; set; }
        //TODO: сделать перечень всех ролей !!
        public string Role { get; set; }

    }
}
