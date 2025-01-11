using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentiyFreamwork.data
{
    public class ClamsRepository
    {
        public static List<Claim> ClamsList = new  List<Claim>  {
            new Claim("Create", "Create"),
            new Claim("Edit", "Edit"),
            new Claim("Update", "Update"),
            new Claim("Delete", "Delete"),
        };
    }
}