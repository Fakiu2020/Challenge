using System;

namespace NetChallenge.Domain
{
    public class Booking
    {
        public Booking()
        {
            Office= new Office();   
        }
        public Guid Id { get; set; }
        public Guid OfficeId { get; set; }
        public Office Office { get; set; }
        public string LocationName { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }
    }
}