using Community_House_Management.DataAccess;
using Community_House_Management.Models;
using Community_House_Management.ModelsDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Community_House_Management.Services
{
    class Service
    {
        public async Task CreateEventAsync(EventModel eventcreated)
        {
            using (var _context = new AppDbContext())
            {
                _context.Events.Add(new Event
                {
                    Name = eventcreated.Name,
                    timeEnd = eventcreated.TimeEnd,
                    timeStart = eventcreated.TimeStart,
                    PersonId = eventcreated.PersonId
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

        public async Task<List<EventModel>> GetEventsAsync()
        {
            using (var _context = new AppDbContext())
            {
                var result = await _context.Events
                    .Include(e => e.Person)
                    .Include(e => e.EventProperties)
                    .ThenInclude(ep => ep.Property)
                    .Select(e => new EventModel
                    {
                        Id = e.Id,
                        PersonId = e.PersonId,
                        TimeEnd = e.timeEnd,
                        TimeStart = e.timeStart,
                        Organizer = new PersonModel
                        {
                            Name = e.Person.Name,
                            Address = e.Person.Address,
                            CitizenId = e.Person.CitizenId
                        },
                        Name = e.Name,
                        PropertyTypes = e.EventProperties
                            .GroupBy(ep => ep.Property.Type)
                            .Select(g => new PropertyType
                            {
                                Type = g.Key,
                                Count = g.Count()
                            })
                            .ToList()
                    })
                    .ToListAsync();
                return result;
            }
        }

        public async Task<PersonModel> GetPersonByCitizenIdAsync(string citizenid)
        {
            using (var _context = new AppDbContext())
            {
                var person =  await _context.Persons
                    .Include(p => p.Household)
                    .ThenInclude(h => h.Header)
                    .Where(p => p.CitizenId == citizenid)
                    .Select(p => new PersonModel
                    {
                        Name = p.Name,
                        Address = p.Address,
                        CitizenId = p.CitizenId,
                        Header = p.Household == null ? null : new PersonModel
                        {
                            Name = p.Household.Header.Name,
                            Address = p.Household.Header.Address,
                            CitizenId = p.Household.Header.CitizenId,
                            HeaderId = p.Household.HeaderId
                        },
                        State = p.state 
                    })
                    .SingleOrDefaultAsync();
                return person;
            }
        }

        public async Task<bool> CheckAccountAsync(string username, string password)
        {
            using (var _context = new AppDbContext())
            {
                var accountFound = await _context.OfficialAccounts
                    .Where(a => a.Username == username && a.Password == password)
                    .SingleOrDefaultAsync();
                return accountFound != null;
            }
        }

        public async Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword)
        {
            using (var _context = new AppDbContext())
            {
                var accountFound = await _context.OfficialAccounts
                    .Where(a => a.Username == username && a.Password == oldPassword)
                    .SingleOrDefaultAsync();
                if (accountFound != null)
                {
                    accountFound.Password = newPassword;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> DeleteEventAsync(string eventName)
        {
            using (var _context = new AppDbContext())
            {
                var eventFound = await _context.Events
                    .Where(e => e.Name == eventName)
                    .SingleOrDefaultAsync();
                if(eventFound != null)
                {
                    _context.Events.Remove(eventFound);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
        }
        
        public async Task<List<PersonModel>> GetPeopleAsync()
        {
            using (var _context = new AppDbContext())
            {
                var allPeople = await _context.Persons
                    .Include(p => p.Household)
                    .ThenInclude(h => h.Header)
                    .Select(p =>  new PersonModel
                    {
                        Name = p.Name,
                        Address = p.Address,
                        CitizenId = p.CitizenId,
                        Header = p.Household == null ? null : new PersonModel
                        {
                            Name = p.Household.Header.Name,
                            Address = p.Household.Header.Address,
                            CitizenId = p.Household.Header.CitizenId,
                            HeaderId = p.Household.HeaderId
                        },
                        State = p.state
                    })
                    .ToListAsync();
                return allPeople;
            } 
        }

        public async Task<bool> AddNewPersonAsync(string name, string address, string citizenId)
        {
            using (var _context = new AppDbContext())
            {
                var person = await _context.Persons.SingleOrDefaultAsync(p => p.CitizenId == citizenId);
                if (person != null) return false;
                _context.Persons.Add(new Person
                {
                    Name = name,
                    Address = address,
                    CitizenId = citizenId
                });
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeletePersonAsync(string citizenId)
        {
            using (var _context = new AppDbContext())
            {
                var personFound = await _context.Persons
                    .SingleOrDefaultAsync(p => p.CitizenId == citizenId);
                if (personFound == null) return false;
                if (personFound.HouseholdOwnedId != null)
                {
                    await DeleteHouseholdAsync(personFound.CitizenId);
                }
                _context.Persons.Remove(personFound);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> ChangeStateAsync(PersonModel person)
        {
            using (var _context = new AppDbContext())
            {
                var personFound = await _context.Persons
                    .SingleOrDefaultAsync(p => p.CitizenId == person.CitizenId);
                if (personFound == null) return false;
                if (personFound.HouseholdId == null) return false;
                personFound.state = person.State;
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> AddMembersAsync(string headerCitizenId, List<PersonModel>members)
        {
            using (var _context = new AppDbContext())
            {
                var header = await _context.Persons
                    .SingleOrDefaultAsync(p => p.CitizenId == headerCitizenId);
                if (header == null) return false;
                if (header.HouseholdOwnedId == null) return false;
                foreach (var member in members)
                {
                    var memberFound = await _context.Persons
                        .SingleOrDefaultAsync(p => p.CitizenId == member.CitizenId);
                    if (memberFound == null) return false;
                    if (memberFound.HouseholdId != null) return false;
                }
                foreach (var member in members)
                {
                    var memberFound = await _context.Persons
                        .SingleOrDefaultAsync(p => p.CitizenId == member.CitizenId);
                    memberFound.HouseholdId = header.HouseholdOwnedId;
                    memberFound.state = member.State;
                }
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> AddNewHouseholdAsync(string headerCitizenId)
        {
            using (var _context = new AppDbContext())
            {
                var header = await _context.Persons
                    .SingleOrDefaultAsync(p => p.CitizenId == headerCitizenId);
                if (header == null) return false;
                if (header.HouseholdId != null) return false;
                _context.Households.Add(new Household
                {
                    HeaderId = header.Id
                });
                await _context.SaveChangesAsync();
                header.HouseholdId = header.HouseholdOwnedId;
                header.state = 1;
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteHouseholdAsync(string headerCitizenId)
        {
            using (var _context = new AppDbContext())
            {
                var header = await _context.Persons
                    .SingleOrDefaultAsync(p => p.CitizenId == headerCitizenId);
                if (header == null) return false;
                var household = await _context.Households
                    .SingleOrDefaultAsync(h => h.HeaderId == header.Id);
                if(household == null) return false;
                await foreach (var member in _context.Persons
                        .Where(p => p.HouseholdId == household.Id)
                        .AsAsyncEnumerable())
                {
                    member.HouseholdId = null;
                    member.state = null;
                }
                header.HouseholdOwnedId = null;
                _context.Households.Remove(household);
                await _context.SaveChangesAsync();
                return true;
            }
        }
        public async Task<bool> ChangePersonInformationAsync(string oldCitizenId, PersonModel newPerson)
        {
            using(var _context = new AppDbContext())
            {
                var person = await _context.Persons
                    .SingleOrDefaultAsync(p => p.CitizenId ==  oldCitizenId);
                if (person == null) return false;
                if(newPerson.CitizenId != oldCitizenId)
                {
                    var personDuplicated = await _context.Persons
                        .SingleOrDefaultAsync(p => p.CitizenId == newPerson.CitizenId);
                    if(personDuplicated != null) return false;
                }
                person.Name = newPerson.Name;
                person.Address = newPerson.Address;
                person.CitizenId = newPerson.CitizenId;
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> RemoveMembersAsync(string headerCitizenId, List<string>memberCitizenIds)
        {
            using(var _context = new AppDbContext())
            {
                var header = await _context.Persons
                    .SingleOrDefaultAsync(p => p.CitizenId == headerCitizenId);
                if (header == null) return false;
                if (header.HouseholdOwnedId == null) return false;
                foreach(var memberId in memberCitizenIds)
                {
                    var memberFound = await _context.Persons
                        .SingleOrDefaultAsync(p => p.CitizenId == memberId);
                    if (memberFound == null) return false;
                    if (memberId == headerCitizenId) return false;
                    if (memberFound.HouseholdId != header.HouseholdId) return false;
                }
                foreach (var memberId in memberCitizenIds)
                {
                    var memberFound = await _context.Persons
                        .SingleOrDefaultAsync(p => p.CitizenId == memberId);
                    memberFound.HouseholdId = null;
                    memberFound.state = null;
                }
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<PersonModel>> GetHouseholdAsync(string headerCitizenId)
        {
            using (var _context = new AppDbContext())
            {
                var header = await _context.Persons
                    .SingleOrDefaultAsync(p => p.CitizenId == headerCitizenId);
                List<PersonModel> result = new List<PersonModel>();
                if (header == null) return result;
                if (header.HouseholdOwnedId == null) return result;
                result = await _context.Persons
                    .Where(p => p.HouseholdId == header.HouseholdOwnedId)
                    .Select(p => new PersonModel
                    {
                        Name = p.Name,
                        Address = p.Address,
                        CitizenId = p.CitizenId,
                        State = p.state
                    })
                    .ToListAsync();
                return result;
            }
        }
    }
}
