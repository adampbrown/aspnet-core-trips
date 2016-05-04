using System;
using System.Collections.Generic;
namespace TheWorld.Models.Shared
{
    public class NavbarModel
    {
        public bool DisplayNavigation { get; set; }

        public string Username { get; set; }

        public IEnumerable<MenuItem> MenuItems { get; set; }
    }
}
