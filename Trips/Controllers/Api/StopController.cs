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
    using TheWorld.Services;
    using TheWorld.ViewModels;

    [Authorize]
    [Route("api/trips/{tripName}/stops")]
    public class StopController : Controller
    {
        private readonly CoordService coordService;

        private readonly ILogger<StopController> logger;

        private readonly IWorldRepository repository;

        /// <summary>
        /// The controller for managing trip stops.
        /// </summary>
        /// <param name="repository">The <see cref="IWorldRepository"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="coordService">The <see cref="CoordService"/>.</param>
        public StopController(IWorldRepository repository,
          ILogger<StopController> logger,
          CoordService coordService)
        {
            this.repository = repository;
            this.logger = logger;
            this.coordService = coordService;
        }

        [HttpGet("")]
        public JsonResult Get(string tripName)
        {
            try
            {
                var results = this.repository.GetTripByName(tripName, User.Identity.Name);

                if (results == null)
                {
                    return Json(null);
                }

                return Json(Mapper.Map<IEnumerable<StopViewModel>>(results.Stops.OrderBy(s => s.Order)));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get stops for trip {tripName}", ex);

                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return Json("Error occurred finding trip name");
            }
        }

        public async Task<JsonResult> Post(string tripName, [FromBody]StopViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newStop = Mapper.Map<Stop>(vm);

                    // Looking up Geo-coordinates
                    var coordResult = await this.coordService.Lookup(newStop.Name);

                    if (!coordResult.Success)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;

                        Json(coordResult.Message);
                    }

                    newStop.Longitude = coordResult.Longitude;
                    newStop.Latitude = coordResult.Latitude;

                    this.repository.AddStop(tripName, User.Identity.Name, newStop);

                    if (this.repository.SaveAll())
                    {
                        Response.StatusCode = (int)HttpStatusCode.Created;

                        return Json(Mapper.Map<StopViewModel>(newStop));
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError("Failed to save new stop", ex);

                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return Json("Failed to save new stop");
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json("Validation failed on new stop");
        }
    }
}
