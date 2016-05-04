namespace TheWorld.Models
{
    using System.Collections.Generic;

    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();

        IEnumerable<Trip> GetAllTripsWithStops();

        void AddTrip(Trip newTrip);

        bool SaveAll();

        Trip GetTripByName(string tripName, string username);

        void AddStop(string tripName, string username, Stop newStop);

        IEnumerable<Trip> GetUserTripsWithStops(string name);
    }
}