namespace TheWorld.Controllers.Web
{
    using Microsoft.AspNet.Mvc;
    using TheWorld.Models;
    using TheWorld.Services;
    using TheWorld.ViewModels;
    using Microsoft.AspNet.Authorization;

    public class AppController : Controller
    {
        private readonly IMailService mailService;

        private readonly IWorldRepository repository;

        public AppController(IMailService service, IWorldRepository repository)
        {
            this.mailService = service;
            this.repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Trips()
        {
            var trips = this.repository.GetAllTrips();

            return View(trips);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = Startup.Configuration["AppSettings:SiteEmailAddress"];

                if (string.IsNullOrWhiteSpace(email))
                {
                    ModelState.AddModelError(string.Empty, "Could not send email, configuration problem.");
                }

                if (this.mailService.SendMail(email, email, $"Contact Page from {model.Name} ({model.Email})", model.Message))
                {
                    ModelState.Clear();

                    ViewBag.Message = "Your mail has been sent. Thanks for getting in touch.";
                }
            }

            return View();
        }
    }
}
