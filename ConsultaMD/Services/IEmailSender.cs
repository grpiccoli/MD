using ConsultaMD.Models.Entities;
using SendGrid;
using System;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public interface IEmailSender
    {
        Task<Response> SendEmailAsync(string email, string subject, string message, string logo = null);
        Task<Response> SendVerificationEmail(string email, Uri callbackUrl);
    }
}
