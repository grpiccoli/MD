using System;
using System.Threading.Tasks;
using System.Net;
using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Identity;
using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.Html;
using ConsultaMD.Models.VM;

namespace ConsultaMD.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkId=532713
    public class EmailSender : IEmailSender
    {
        private readonly IViewRenderService _viewRenderService;
        public EmailSender(
            IViewRenderService viewRenderService,
            IConfiguration configuration)
        {
            Configuration = configuration;
            _viewRenderService = viewRenderService;
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
        public async Task<Response> SendVerificationEmail(string email, Uri callbackUrl)
        {
            var logo = "wwwroot/android-chrome-256x256.png";

            var emailModel = new ValidationEmailVM
            {
                Header = new HtmlString("¡Estás a un paso de tu atención médica!<br>Confirma tu correo."),
                Url = new HtmlString(callbackUrl?.ToString()),
                Color = "19989d",
                LogoId = "logo",
                Body = new HtmlString("Al hacer click en el siguiente enlace, estás confirmando tu correo."),
                ButtonTxt = new HtmlString("Confirmar Correo")
            };

            var emailView = await _viewRenderService
                .RenderToStringAsync("Shared/_ValidationEmail", emailModel)
                .ConfigureAwait(false);

            return await SendEmailAsync(email, "Verifica tu correo", emailView, logo)
                .ConfigureAwait(false);
        }
    }
}
