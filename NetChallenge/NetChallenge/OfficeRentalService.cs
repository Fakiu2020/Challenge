using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using NetChallenge.Abstractions;
using NetChallenge.Domain;
using NetChallenge.Dto.Input;
using NetChallenge.Dto.Output;
using NetChallenge.Exceptions;
using NetChallenge.Infrastructure;

namespace NetChallenge
{
    public class OfficeRentalService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IOfficeRepository _officeRepository;
        private readonly IBookingRepository _bookingRepository;

        public OfficeRentalService(ILocationRepository locationRepository, IOfficeRepository officeRepository, IBookingRepository bookingRepository)
        {
            _locationRepository = locationRepository;
            _officeRepository = officeRepository;
            _bookingRepository = bookingRepository;
        }

        public void AddLocation(AddLocationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    throw new ArgumentNullException("Name is required");
                if (string.IsNullOrWhiteSpace(request.Neighborhood))
                    throw new ArgumentNullException("Neighborhood is required");
                if (_locationRepository.AsEnumerable().Any(l => l.Name == request.Name))
                    throw new DuplicateEntityException($"A location with name '{request.Name}' already exists.");

                var newLocation = new Location() { Name = request.Name, Neighborhood = request.Neighborhood };
                _locationRepository.Add(newLocation);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine($"Type: {e.GetType()}");                  // Get the actual Type of the exception
                Console.WriteLine($"Message: {e.Message}");                // Retrieve the message string that the exception was created with
                Console.WriteLine($"Source: {e.Source}");                 // Source is the application or object where the exception occurred
                Console.WriteLine($"TargetSite: {e.TargetSite}");        // TargetSite is the name of the method that threw the exception
                Console.WriteLine($"Stack: {e.StackTrace}");            // The StackTrace contains the stack calls leading up to the exception
                throw e;
            }
            catch (DuplicateEntityException e)
            {
                Console.WriteLine($"Message: {e.Message}");
                throw e;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddOffice(AddOfficeRequest request)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(request.Name))
                    throw new ArgumentNullException("Office Name", "Office Name is required");

                var location = _locationRepository.AsEnumerable().FirstOrDefault(l => l.Name.ToUpper() == request.LocationName.ToUpper());
                if (location == null)
                    throw new ArgumentNullException("Location", $"Location '{request.LocationName}' not found.");

                if (request.MaxCapacity <= 0)
                    throw new ArgumentException("Capacity", "Max capacity must be greater than zero.");

                if (location.Offices.Any(office => office.Name == request.Name))    // check if office already exist in other            
                    throw new DuplicateEntityException($"An office with name '{request.Name}' already exists in location '{request.LocationName}'.");

                var newOffice = new Domain.Office
                {
                    Name = request.Name,
                    Location = location,
                    Capacity = request.MaxCapacity,
                    Resources = request.AvailableResources.ToList()
                };
                location.Offices.Add(newOffice);
                _officeRepository.Add(newOffice);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine($"Message: {e.Message}");                // Retrieve the message string that the exception was created with                
                throw e;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            catch (DuplicateEntityException e)
            {
                Console.WriteLine($"Type: {e.Message}");
                throw e;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void BookOffice(BookOfficeRequest request)
        {

            try
            {
                var office = _officeRepository.AsEnumerable().FirstOrDefault(o => o.Name == request.OfficeName && o.Location.Name == request.LocationName);
                if (office == null)
                    throw new ArgumentException($"Office '{request.OfficeName}' not found.");

                if (request.Duration <= TimeSpan.Zero)
                    throw new ArgumentException("Booking duration must be greater than zero.");

                if (string.IsNullOrWhiteSpace(request.UserName))
                    throw new ArgumentNullException("UserName is Required.");

                if (IsBookingOverlapping(office.Name, request.DateTime, request.Duration))
                    throw new ArgumentException($"Office '{request.OfficeName}' is already booked at the specified time.");

                var booking = new Booking
                {
                    Office = office,
                    UserName = request.UserName,
                    Date = request.DateTime,
                    Duration = request.Duration
                };

                _bookingRepository.Add(booking);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine($"Message: {e.Message}");
                throw e;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Message: {ex.Message}");
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public IEnumerable<BookingDto> GetBookings(string locationName, string officeName)
        {
            try
            {
                var office = _officeRepository.AsEnumerable().FirstOrDefault(o => o.Name == officeName && o.Location.Name == locationName);
                if (office == null)
                    throw new ArgumentException($"Office '{officeName}' not found in location '{locationName}'.");


                var bookings = _bookingRepository.AsEnumerable()
                                        .Where(b => b.Office.Name == office.Name)
                                        .ToList();
                if (bookings.Count == 0)
                    throw new ArgumentException($"No bookings found for office '{officeName}' in location '{locationName}'.");

                return bookings.Select(b => new BookingDto
                {
                    LocationName = locationName,
                    OfficeName = officeName,
                    DateTime = b.Date,
                    Duration = b.Duration,
                    UserName = b.UserName
                });
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"Message: {e.Message}");
                throw e;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public IEnumerable<LocationDto> GetLocations()
        {
            return _locationRepository.AsEnumerable()
             .Select(location => new LocationDto
             {
                 Name = location.Name,
                 Neighborhood = location.Neighborhood
             });
        }

        public IEnumerable<OfficeDto> GetOffices(string locationName)
        {
            try
            {
                var location = _locationRepository.AsEnumerable().Where(x => x.Name.ToUpper() == locationName.ToUpper()).FirstOrDefault();
                if (location == null) throw new ArgumentException("Location", $"{locationName}' not found.");
                if (location.Offices.Count() == 0) return new List<OfficeDto>();

                return location.Offices.Select(office => new OfficeDto
                {
                    LocationName = locationName,
                    Name = office.Name,
                    MaxCapacity = office.Capacity,
                    AvailableResources = office.Resources.ToArray(),
                });
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"Message: {e.Message}");                // Retrieve the message string that the exception was created with                
                throw e;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public IEnumerable<OfficeDto> GetOfficeSuggestions(SuggestionsRequest request)
        {

            var availableOffices = _officeRepository.AsEnumerable()
                .Where(office => office.Capacity >= request.CapacityNeeded &&
                    request.ResourcesNeeded.All(resource => office.Resources.Contains(resource)));

            var sortedOffices = availableOffices
                .OrderByDescending(office => office.Location.Neighborhood == request.PreferedNeigborHood)
                .ThenBy(office => office.Capacity)
                .ThenBy(office => office.Resources.Count());


            return sortedOffices.Select(office => new OfficeDto
            {
                LocationName = office.Location.Name,
                Name = office.Name,
                MaxCapacity = office.Capacity,
                AvailableResources = office.Resources.ToArray(),
            });

        }

        private bool IsBookingOverlapping(string officeName, DateTime bookingDateTime, TimeSpan bookingDuration)
        {


            var endDateTime = bookingDateTime.Add(bookingDuration);
            var overlappingBookings = _bookingRepository.AsEnumerable()
               .Where(b =>
                   b.Office.Name.ToUpper() == officeName.ToUpper() &&
                    (
                    (bookingDateTime >= b.Date && bookingDateTime < b.Date.Add(b.Duration)) ||                 // The new booking starts within an existing booking.
                    (endDateTime > b.Date && endDateTime <= b.Date.Add(b.Duration)) ||                        //  The new booking ends within an existing booking.
                    (bookingDateTime <= b.Date && endDateTime >= b.Date.Add(b.Duration)))                    //   The new booking completely encloses an existing booking.
               );
            var overlappingBookings1 = _bookingRepository.AsEnumerable()
                    .Where(b =>
                        b.Office.Name.ToUpper() == officeName.ToUpper() &&
                        b.Date < bookingDateTime.Add(bookingDuration) &&
                        b.Date.Add(b.Duration) > bookingDateTime
                    );
            return overlappingBookings.Any();
        }
    }
}