using ConsultaMD.Data;
using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    public class CardDetailsModel : PageModel
    {
        private readonly IFlow _flow;
        private readonly FlowSettings _options;
        private readonly ApplicationDbContext _context;
        public CardDetailsModel(
            IFlow flow,
            IOptions<FlowSettings> options,
            ApplicationDbContext context)
        {
            _flow = flow;
            _options = options?.Value;
            _flow.FlowSettings = _options;
            _context = context;
        }
        public Uri ReturnUrl { get; set; }
        public async Task<IActionResult> OnGet(Uri returnUrl = null)
        {
            ReturnUrl = returnUrl;

            //var user = await _context.Users
            //    .SingleOrDefaultAsync(u => u.UserName == User.Identity.Name)
            //    .ConfigureAwait(false);
            //var customer = await _flow.CustomerCreate(user.Email, user.PersonId)
            //    .ConfigureAwait(false);
            var customer = new Customer { 
                Status = Status.Activo,
                ExternalId = RUT.Unformat(User.Identity.Name).Value.rut
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            //var register = await _flow
            //    .CustomerRegister(customer.CustomerId, validation)
            //    .ConfigureAwait(false);
            //customer.Token = register.Token;
            //_context.Customers.Update(customer);
            //await _context.SaveChangesAsync().ConfigureAwait(false);
            //return Redirect($"{register.Url.ToString()}?token={register.Token}");
            return RedirectToPage("CardValidation", new { returnUrl = ReturnUrl, token = "" });
        }
    }
}
