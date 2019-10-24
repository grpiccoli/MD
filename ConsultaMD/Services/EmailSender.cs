using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.Extensions.Configuration;
using Attachment = SendGrid.Helpers.Mail.Attachment;
using System.IO;

namespace ConsultaMD.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkId=532713
    public class EmailSender : IEmailSender
    {
        public EmailSender(
            IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public Task<Response> SendEmailAsync(string email, string subject, string message, string logo = null)
        {
            return Execute("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", subject, message, email, logo);
        }

        public static Task<Response> Execute(string apiKey, string subject, string message, string email, string logo = null)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("no-responder@consultamd.cl", "ConsultaMD"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            var bytes = File.ReadAllBytes(logo);
            var file = Convert.ToBase64String(bytes);
            msg.AddAttachment("logo.png", file, "image/png", "inline", "logo");
            msg.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(msg);
        }
    }
}
