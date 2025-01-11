using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentiyFreamwork.Models
{
    public class EditClamsViewModel
    {
        public string UserId {get; set;}

        public List<UserClaims> UserClaims {get; set;} = new List<UserClaims>();
    }

    public class UserClaims
    {
        public string ClaimType {get; set;}

        public bool IsSelected {get; set;}
    }
}