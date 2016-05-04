namespace TheWorld.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Mvc;
    using TheWorld.Models;
    using TheWorld.ViewModels;

    public class AuthController : Controller
    {
        private readonly SignInManager<WorldUser> signInManager;

        public AuthController(SignInManager<WorldUser> signInManager)
        {
            this.signInManager = signInManager;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Trips", "App");
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel vm, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // username = samhastings
                // pwd = P@ssw0rd!

                var signInResult = await this.signInManager.PasswordSignInAsync(vm.Username, vm.Password, true, false);

                if (signInResult.Succeeded)
                {
                    if (string.IsNullOrWhiteSpace(returnUrl))
                    {
                        return RedirectToAction("Trips", "App");
                    }

                    return this.Redirect(returnUrl);
                }

                this.ModelState.AddModelError("", "Username or password incorrect");
            }

            return View();
        }

        public async Task<ActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await this.signInManager.SignOutAsync();
            }

            return RedirectToAction("Index", "App");
        }
    }
}
