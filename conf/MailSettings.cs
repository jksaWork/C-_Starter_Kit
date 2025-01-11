using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentiyFreamwork.conf
{
    public class MailSettings
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
    }
}