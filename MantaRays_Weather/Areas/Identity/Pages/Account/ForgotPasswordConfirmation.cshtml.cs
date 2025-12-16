using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MantaRays_Weather.Areas.Identity.Pages.Account
{
    // Apply [AllowAnonymous] since this is a public-facing Identity page
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModel
    {
        public void OnGet()
        {
        }
    }
}