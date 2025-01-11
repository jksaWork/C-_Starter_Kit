using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentiyFreamwork.conf;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;

namespace IdentiyFreamwork.Service
{
    public class MailService
    {
        public readonly MailSettings _mailSetting;
       public  MailService(IOptions<MailSettings> mailSetting){
            _mailSetting = mailSetting.Value;
       }

       public async Task SendEmailAsync(string ToEmail , string Subject , string Body){

            var email = new MimeMessage(); 
            email.Sender = MailboxAddress.Parse(_mailSetting.SenderEmail);
            email.From.Add(new MailboxAddress(_mailSetting.SenderName, _mailSetting.SenderEmail));
            email.To.Add(MailboxAddress.Parse(ToEmail));
            email.Subject = Subject;

            var bulider = new BodyBuilder {
                HtmlBody = Body,
            };

            email.Body =  bulider.ToMessageBody();

            using var Smtp = new SmtpClient();
            await Smtp.ConnectAsync(_mailSetting.SmtpHost, _mailSetting.SmtpPort,MailKit.Security.SecureSocketOptions.StartTls);
            await Smtp.AuthenticateAsync(_mailSetting.SenderName, _mailSetting.SmtpPass);
            await Smtp.SendAsync(email);
            await Smtp.DisconnectAsync(true);
       }
    }
}