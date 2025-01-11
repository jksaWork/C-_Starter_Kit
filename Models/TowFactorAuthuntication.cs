using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;
using Microsoft.VisualBasic;

namespace IdentiyFreamwork.Models
{
    public class TowFactorAuthuntication
    {
        public string? Token;

        [Required]
        public string Code;
    }
}