namespace TheWorld.Views.Shared.Components
{
    using System.Collections.Generic;

    using Microsoft.AspNet.Mvc;

    using TheWorld.Models.Shared;

    public class NavbarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var currentController = ViewContext.RouteData.Values["controller"].ToString();

            var menus = new List<MenuItem>();

            menus.Add(new MenuItem
            {
                Text = "Home",
                Url = Url.Action("Index", "App"),
                Active = currentController == "Home"
            });

            var model = new NavbarModel
            {
                DisplayNavigation = User.Identity.IsAuthenticated,
                Username = User.Identity.Name,
                MenuItems = menus
            };

            return View(model);
        }
    }
}
