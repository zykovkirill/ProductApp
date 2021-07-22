using System;
using System.Collections.Generic;
using System.Text;

namespace ProductApp.Shared.Models
{
    public class UserManagerResponse:BaseAPIResponse
    {
        public string [] Errors { get; set; }
        public Dictionary<string, string> UserInfo { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
