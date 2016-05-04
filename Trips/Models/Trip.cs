namespace TheWorld.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The Trip.
    /// </summary>
    public class Trip
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the trip.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the date the trip was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the username of the user taking the trip.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the collection of Stops on the trip.
        /// </summary>
        public ICollection<Stop> Stops { get; set; }
    }
}
