namespace TheWorld.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Data.Entity;
    using Microsoft.Extensions.Logging;

    public class WorldRepository : IWorldRepository
    {
        private WorldContext context;

        private ILogger<WorldRepository> logger;

        public WorldRepository(WorldContext context, ILogger<WorldRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public void AddStop(string tripName, string username, Stop newStop)
        {
            var theTrip = this.GetTripByName(tripName, username);

            newStop.Order = theTrip.Stops.Any() ? theTrip.Stops.Max(s => s.Order) + 1 : 1;

            theTrip.Stops.Add(newStop);

            this.context.Stops.Add(newStop);
        }

        public void AddTrip(Trip newTrip)
        {
            this.context.Add(newTrip);
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            try
            {
                return this.context.Trips.OrderBy(t => t.Name).ToList();
            }
            catch (Exception ex)
            {
                this.logger.LogError("Could not get trips from database", ex);
                return null;
            }
        }

        public IEnumerable<Trip> GetAllTripsWithStops()
        {
            try
            {
                return this.context.Trips
                .Include(t => t.Stops)
                .OrderBy(t => t.Name)
                .ToList();
            }
            catch (Exception ex)
            {
                this.logger.LogError("Could not get trips with stops from database", ex);
                return null;
            }
        }

        public Trip GetTripByName(string tripName, string username)
        {
            return
                this.context.Trips
                    .Include(t => t.Stops)
                    .FirstOrDefault(t => t.Name == tripName && t.UserName == username);
        }

        public IEnumerable<Trip> GetUserTripsWithStops(string name)
        {
            try
            {
                return this.context.Trips
                .Include(t => t.Stops)
                .OrderBy(t => t.Name)
                .Where(t => t.UserName == name)
                .ToList();
            }
            catch (Exception ex)
            {
                this.logger.LogError("Could not get trips with stops from database", ex);
                return null;
            }
        }

        public bool SaveAll()
        {
            return this.context.SaveChanges() > 0;
        }
    }
}
