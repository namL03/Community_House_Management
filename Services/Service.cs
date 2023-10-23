using Community_House_Management.DataAccess;
using Community_House_Management.Models;
using Community_House_Management.ModelsDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Community_House_Management.Services
{
    class Service
    {
        public async Task CreateEventAsync(int creatorId, EventModel eventcreated)
        {
            using (var _context = new AppDbContext())
            {
                _context.Events.Add(new Event
                {
                    Name = eventcreated.Name,
                    timeEnd = eventcreated.TimeEnd,
                    timeStart = eventcreated.TimeStart,
                    PersonId = creatorId
                });
                await _context.SaveChangesAsync();
            }
        }
        public async Task AddPropertiesToEventAsync(int eventId, List<int> propertyIds)
        {
            foreach (var propertyId in  propertyIds)
            {
                using (var _context = new AppDbContext()) 
                {
                    Event eventchosen = await _context.Events
                        .Include(e => e.EventProperties)
                        .ThenInclude(ep => ep.Property)
                        .SingleOrDefaultAsync(e => e.Id == propertyId);
                    EventProperty eventProperty = new EventProperty
                    {
                        EventId = eventId,
                        PropertyId = propertyId
                    };
                    eventchosen.EventProperties.Add(eventProperty);
                    await _context.SaveChangesAsync();
                }
            }
        }
        public async Task CreatePropertyAsync(PropertyModel property)
        {
            using (var _context = new AppDbContext())
            {
                _context.Properties.Add(new Property
                {
                    Type = property.Type,
                });
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<PropertyType>> GetPropertiesTypeAsync()
        {
            using (var _context = new AppDbContext ())
            {
                var result = await _context.Properties
                .GroupBy(p => p.Type)
                .Select(g => new PropertyType
                {
                    Type = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
                return result;
            }
        }
        
        /*public async Task CreateAccountAsync(int personId, AccountModel accountCreated)
        {
            using (var _context = new AppDbContext())
            {
                _context.OfficialAccounts.Add(new OfficialAccount
                {
                    Username = accountCreated.Username,
                    Password = accountCreated.Password,
                    PersonId = personId,
                    CitizenId = accountCreated.CitizenId
                });
                await _context.SaveChangesAsync();
            }
        }*/
    }
}
