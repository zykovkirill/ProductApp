using System;

namespace ProductApp.Shared.Models
{
    public class UserManagerResponse : BaseAPIResponse
    {
        public string[] Errors { get; set; }
        public LocalUserInfo UserInfo { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
