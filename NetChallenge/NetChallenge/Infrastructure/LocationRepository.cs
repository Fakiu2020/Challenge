using System.Collections.Generic;
using NetChallenge.Abstractions;
using NetChallenge.Domain;

namespace NetChallenge.Infrastructure
{
    public class LocationRepository : ILocationRepository
    {
        private readonly List<Location> _locations;
        public LocationRepository()
        {
            _locations = new List<Location>();
        }
        public IEnumerable<Location> AsEnumerable()
        {
            return _locations;
        }

        public void Add(Location item)
        {
            _locations.Add(item);
        }

    }
}