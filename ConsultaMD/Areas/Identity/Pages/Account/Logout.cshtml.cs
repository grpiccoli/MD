using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultaMD.Models;
using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace ConsultaMD.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly IStringLocalizer<LogoutModel> _localizer;

        public LogoutModel(SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<LogoutModel> localizer,
            ILogger<LogoutModel> logger)
        {
            _localizer = localizer;
            _signInManager = signInManager;
            _logger = logger;
        }

        public static void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(Uri returnUrl = null)
        {
            await _signInManager.SignOutAsync().ConfigureAwait(false);
            _logger.LogInformation(_localizer["User logged out."]);
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl.ToString());
            }
            else
            {
                return Page();
            }
        }
    }
}