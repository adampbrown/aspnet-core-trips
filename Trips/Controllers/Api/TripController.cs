namespace TheWorld.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Mvc;
    using Microsoft.Extensions.Logging;
    using TheWorld.Models;
    using TheWorld.ViewModels;

    [Authorize]
    [Route("api/trips")]
    public class TripController : Controller
    {
        private readonly ILogger<TripController> logger;

        private readonly IWorldRepository repository;

        public TripController(IWorldRepository repository, ILogger<TripController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        [HttpGet("")]
        public JsonResult Get()
        {
            var trips = this.repository.GetUserTripsWithStops(User.Identity.Name);
            var results = Mapper.Map<IEnumerable<TripViewModel>>(trips);

            return Json(results);
        }

        [HttpPost("")]
        public JsonResult Post([FromBody]TripViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newTrip = Mapper.Map<Trip>(vm);

                    newTrip.UserName = User.Identity.Name;

                    this.logger.LogInformation("Attempting to save a new trip");
                    this.repository.AddTrip(newTrip);

                    if (this.repository.SaveAll())
                    {
                        Response.StatusCode = (int)HttpStatusCode.Created;

                        return Json(Mapper.Map<TripViewModel>(newTrip));
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Failed to save new trip", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return Json(new { Message = ex.Message });
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json(new { Message = "Failed", ModelState = ModelState });
        }
    }
}
