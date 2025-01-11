using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentiyFreamwork.Models
{
    public class EditUserViewModel
    {
         public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> AllRoles { get; set; } = new List<string>(); // To display all available roles

    }
}