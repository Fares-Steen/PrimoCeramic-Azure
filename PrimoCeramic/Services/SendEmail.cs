using MailKit.Net.Smtp;
using MimeKit;
using PrimoCeramic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimoCeramic.Services
{
    public class SendEmail
    {

        private string EmailTo;
        private string Subject;
        private string HTML;
        private BodyBuilder bodyBuilder;
        private Emails Emails;

        //bodyBuilder.HtmlBody = "<b>This is some html text</b>";
        //    bodyBuilder.TextBody = "This is some plain text";
        public SendEmail(string EmailTo,string Subject,string HTML, Emails Emails)
        {
            this.EmailTo = EmailTo;
            this.Subject = Subject;
            this.HTML = HTML;
            this.Emails = Emails;
        }
        public SendEmail(string EmailTo, string Subject, BodyBuilder bodyBuilder,Emails Emails)
        {
            this.EmailTo = EmailTo;
            this.Subject = Subject;
            this.bodyBuilder = bodyBuilder;
            this.Emails = Emails;
        }
        public void SendOrder()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Emails.Email));
            message.To.Add(new MailboxAddress(EmailTo));
            message.Subject = Subject;
            bodyBuilder = new BodyBuilder();
               bodyBuilder.HtmlBody = HTML;
         
            message.Body = bodyBuilder.ToMessageBody();
            try { 
            using (var client = new SmtpClient())
            {
                client.Connect(Emails.Map, Emails.Port, false);
                client.Authenticate(Emails.Email, Emails.Password);
                client.Send(message);
                client.Disconnect(true);

            }
            }catch(Exception e) { }
        }
        public void SendRestPassword()
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Emails.Email));
            message.To.Add(new MailboxAddress(EmailTo));
            message.Subject = Subject;

            message.Body = bodyBuilder.ToMessageBody();
            try
            {
                using (var client = new SmtpClient())
            {
                client.Connect(Emails.Map, Emails.Port, false);
                client.Authenticate(Emails.Email, Emails.Password);
                client.Send(message);
                client.Disconnect(true);

            }
            }
            catch (Exception e) { }
        }
    }
}
