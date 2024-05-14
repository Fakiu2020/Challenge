using System;
using System.Collections.Generic;

namespace NetChallenge.Domain
{
    public class Location
    {
        public Location()
        {
            Offices= new List<Office>();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Neighborhood { get; set; }
        public ICollection<Office> Offices { get;  set; }
    }
}