using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentiyFreamwork.Models
{
    public class CreateUserViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}