using System.Collections.Generic;
using NetChallenge.Abstractions;
using NetChallenge.Domain;

namespace NetChallenge.Infrastructure
{
    public class OfficeRepository : IOfficeRepository
    {
        private readonly List<Office> _offices;

        public OfficeRepository()
        {
            _offices = new List<Office>();
        }
        public IEnumerable<Office> AsEnumerable()
        {
            return _offices;

        }

        public void Add(Office item)
        {
            _offices.Add(item);
        }
    }
}