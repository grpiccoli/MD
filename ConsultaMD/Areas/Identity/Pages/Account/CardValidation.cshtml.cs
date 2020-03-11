using System;
using System.Threading.Tasks;
using ConsultaMD.Data;
using ConsultaMD.Models.Entities;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    public class CardValidationModel : PageModel
    {
        private readonly IFlow _flow;
        private readonly ApplicationDbContext _context;
        private readonly IRedirect _redirect;
        public CardValidationModel
            (IFlow flow,
            IRedirect redirect,
            ApplicationDbContext context)
        {
            _flow = flow;
            _redirect = redirect;
            _context = context;
        }
        public Uri ReturnUrl { get; set; }
        public async Task<IActionResult> OnGet(string token, Uri returnUrl = null)
        {
            ReturnUrl = returnUrl;
            var user = await _context.Users
                .Include(u => u.Person)
                    .ThenInclude(p => p.Customer)
                .SingleOrDefaultAsync(u => u.UserName == User.Identity.Name).ConfigureAwait(false);
            if(user.Person.Customer.Token == token)
            {
                var customer = _flow.GetRegisterStatus(token);
                if(customer.Status == Status.Activo)
                {
                    user.Person.Customer.Status = customer.Status;
                    user.Person.Customer.CreditCardType = customer.CreditCardType;
                    user.Person.Customer.Last4CardDigits = customer.Last4CardDigits;
                    _context.Customers.Update(user.Person.Customer);
                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    return await _redirect.Redirect(ReturnUrl, User.Identity.Name).ConfigureAwait(false);
                }
            }
            return await _redirect.Redirect(ReturnUrl, User.Identity.Name).ConfigureAwait(false);
        }
    }
}
