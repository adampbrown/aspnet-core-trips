namespace TheWorld.Models.Shared
{
    using System.Collections.Generic;

    public class MenuItem
    {
        public string Text { get; set; }

        public string Url { get; set; }

        public bool Active { get; set; }

        public IEnumerable<MenuItem> SubItems { get; set; }
    }
}
