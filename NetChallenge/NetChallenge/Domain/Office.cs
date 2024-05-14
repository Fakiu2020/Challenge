using System;
using System.Collections.Generic;

namespace NetChallenge.Domain
{
    public class Office
    {
        public Office()
        {
            Resources = new List<string>();
            Location = new Location();
        }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public List<string> Resources { get; set; }
        public Guid LocationId { get; set; }
        public Location Location { get; set; }



    }
}