using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public interface IEmailSender
    {
        Task<Response> SendEmailAsync(string email, string subject, string message, string logo = null);
    }
}
