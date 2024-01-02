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
using Microsoft.Extensions.Logging;
using System.DirectoryServices.ActiveDirectory;

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
        public async Task<List<PropertyTypeModel>> GetPropertiesTypeAsync()
        {
            using (var _context = new AppDbContext ())
            {
                var result = await _context.Properties
                .GroupBy(p => p.Type)
                .Select(g => new PropertyTypeModel
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
                            .Select(g => new PropertyTypeModel
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
                var person = await _context.Persons
                    .Include(p => p.Household)
                    .ThenInclude(h => h.Header)
                    .Include(p => p.Events)
                    .Where(p => p.CitizenId == citizenid)
                    .Select(p => new PersonModel
                    {
                        Id = p.Id,
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
                        State = p.state,
                        Events = p.Events.Select(e => new EventModel {
                            Id = e.Id,
                            PersonId = e.PersonId,
                            TimeEnd = e.timeEnd,
                            TimeStart = e.timeStart,
                            Name = e.Name
                        }).ToList()
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

        public async Task ChangePasswordAsync(string username, string oldPassword, string newPassword)
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
                }
            }
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            using (var _context = new AppDbContext())
            {
                var eventFound = await _context.Events
                    .Where(e => e.Id == eventId)
                    .SingleOrDefaultAsync();
                if (eventFound != null)
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
                        Id = p.Id,
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
                var personFound = await _context.Persons.Include(p => p.HouseholdOwned)
                    .SingleOrDefaultAsync(p => p.CitizenId == citizenId);
                if (personFound == null) return false;
                if (personFound.HouseholdOwned != null)
                {
                    await DeleteHouseholdAsync(personFound.CitizenId);
                }
                _context.Persons.Remove(personFound);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> ChangeStateAsync(PersonModel person)
        {   using (var _context = new AppDbContext())
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
                var header = await _context.Persons.Include(p => p.HouseholdOwned)
                    .SingleOrDefaultAsync(p => p.CitizenId == headerCitizenId);
                if (header == null) return false;
                if (header.HouseholdOwned == null) return false;
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
                    memberFound.HouseholdId = header.HouseholdOwned.Id;
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
                    .Include(p => p.HouseholdOwned)
                    .SingleOrDefaultAsync(p => p.CitizenId == headerCitizenId);
                if (header == null) return false;
                if (header.HouseholdId != null) return false;
                _context.Households.Add(new Household
                {
                    HeaderId = header.Id
                });
                await _context.SaveChangesAsync();
                header = await _context.Persons
                    .Include(p => p.HouseholdOwned)
                    .SingleOrDefaultAsync(p => p.CitizenId == headerCitizenId);
                header.HouseholdId = header.HouseholdOwned.Id;
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
                    .Include(p => p.HouseholdOwned)
                    .SingleOrDefaultAsync(p => p.CitizenId == headerCitizenId);
                if (header == null) return false;
                if (header.HouseholdOwned == null) return false;
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

        public async Task<HouseholdModel> GetHouseholdAsync(string headerCitizenId)
        {
            using (var _context = new AppDbContext())
            {
                var header = await _context.Persons
                    .Include(p => p.HouseholdOwned)
                    .SingleOrDefaultAsync(p => p.CitizenId == headerCitizenId);
             
                if (header == null) return null;
                if (header.HouseholdOwned == null) return null;
                List<PersonModel> _members = new List<PersonModel>();
                PersonModel _header = new PersonModel
                {
                    Id = header.Id,
                    Name = header.Name,
                    CitizenId = header.CitizenId,
                    State = header.state,
                    Address = header.Address,
                };
                _members = await _context.Persons
                    .Where(p => p.HouseholdId == header.HouseholdOwned.Id)
                    .Select(p => new PersonModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Address = p.Address,
                        CitizenId = p.CitizenId,
                        State = p.state
                    })
                    .ToListAsync();
                return new HouseholdModel
                {
                    Id = header.HouseholdOwned.Id,
                    Members = _members,
                    Header = _header,
                };
            }
        }

        public async Task<List<HouseholdModel>> GetAllHouseholdsAsync()
        {
            using(var _context = new AppDbContext())
            {
                var households = await _context.Households
                    .Include(h => h.Members)
                    .Include(h => h.Header)
                    .Select(h => new HouseholdModel
                    {
                        Id= h.Id,
                        Header = new PersonModel
                        {
                            Id = h.Header.Id,
                            Name = h.Header.Name,
                            Address = h.Header.Address,
                            CitizenId= h.Header.CitizenId,
                            State = h.Header.state
                        },
                        Members = h.Members.Select(m => new PersonModel
                        { 
                            Id = m.Id,
                            Name = m.Name,
                            Address = m.Address,
                            CitizenId = m.CitizenId,
                            State = m.state
                        }).ToList()
                    })
                    .ToListAsync();
                return households;
            }
        }
        public async Task<bool> AssignPropertyToEventAsync(int eventId, PropertyTypeModel propertyType)
        {
            using (var _context = new AppDbContext())
            {
                var EventFound = await _context.Events
                    .SingleOrDefaultAsync(e => e.Id == eventId);
                if (EventFound == null) return false;
                var freeProperties = await _context.Properties
                    .Where(p => p.Type == propertyType.Type)
                    .Include(p => p.EventProperties)
                    .ThenInclude(ep => ep.Event)
                    .Where(p => !p.EventProperties.Any(ep => ep.Event.timeEnd >= EventFound.timeStart && ep.Event.timeStart <= EventFound.timeEnd))
                    .ToListAsync();
                if (freeProperties.Count < propertyType.Count) return false;
                foreach (Property property in freeProperties.Take(propertyType.Count))
                {
                    property.EventProperties.Add(new EventProperty
                    {
                        EventId = eventId,
                        PropertyId = property.Id
                    });
                }
                await _context.SaveChangesAsync();
                return true;
            }
        }
        public async Task<List<PropertyTypeModel>> GetAvailablePropertiesForEvent(int eventId)
        {
            using (var _context = new AppDbContext())
            {
                var EventFound = await _context.Events
                    .SingleOrDefaultAsync(e => e.Id == eventId);
                if (EventFound == null) return null;
                var freeProperties = await _context.Properties
                    .Include(p => p.EventProperties)
                    .ThenInclude(ep => ep.Event)
                    .Where(p => !p.EventProperties.Any(ep => ep.Event.timeEnd >= EventFound.timeStart && ep.Event.timeStart <= EventFound.timeEnd))
                    .GroupBy(p => p.Type)
                    .Select(p => new PropertyTypeModel
                    {
                        Type = p.Key,
                        Count = p.Count()
                    })
                    .ToListAsync();
                return freeProperties;
            }
        }
        public async Task<bool> RemovePropertyFromEventAsync(int eventId, PropertyTypeModel propertyType)
        {
            using (var _context = new AppDbContext())
            {
                var eventFound = await _context.Events
                .Include(e => e.EventProperties)
                .ThenInclude(ep => ep.Property)
                .SingleOrDefaultAsync(e => e.Id == eventId);

                if (eventFound == null)
                {
                    return false;
                }
                var eventPropertiesToRemove = eventFound.EventProperties
                    .Where(ep => ep.Property.Type == propertyType.Type)
                    .ToList();

                if (eventPropertiesToRemove.Count < propertyType.Count)
                {
                    return false;
                }
                foreach(var eventPropertyToRemove in eventPropertiesToRemove.Take(propertyType.Count))
                {
                    eventFound.EventProperties.Remove(eventPropertyToRemove);
                }
                await _context.SaveChangesAsync();
                return true;
            }
        }
        public async Task<EventModel> GetEventByIdAsync(int eventId)
        {
            using (var _context = new AppDbContext ())
            {
                var result = await _context.Events
                    .Include(e => e.Person)
                    .Include(e => e.EventProperties)
                    .ThenInclude(ep => ep.Property)
                    .Where(e => e.Id == eventId)
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
                            .Select(g => new PropertyTypeModel
                            {
                                Type = g.Key,
                                Count = g.Count()
                            })
                            .ToList()
                    })
                    .SingleOrDefaultAsync();
                return result;
            }
        }
    }
    
}
