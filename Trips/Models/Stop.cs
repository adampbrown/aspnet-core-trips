namespace TheWorld.Models
{
    using System;

    /// <summary>
    /// The stop.
    /// </summary>
    public class Stop
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the stop.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the arrival date.
        /// </summary>
        public DateTime Arrival { get; set; }

        /// <summary>
        /// Gets or sets the order of the stop on the trip.
        /// </summary>
        public int Order { get; set; }
    }
}