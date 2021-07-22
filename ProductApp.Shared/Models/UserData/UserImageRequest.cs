using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ProductApp.Shared.Models
{
    public class UserImageRequest
    {

        public string Name { get; set; }


        public Stream Data { get; set; }

    }
}
