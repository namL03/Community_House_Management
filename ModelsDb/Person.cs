using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community_House_Management.ModelsDb
{
    class Person
    {
        public int Id { get; set; }
        public string CitizenId { get; set; }
        public string? Address { get; set; }
        public string Name { get; set; }
        public ICollection<Event>? Events { get; set; }
        public int? HouseholdId { get; set; }
        public Household? Household { get; set; }

        public int? HouseholdOwnedId { get; set; }
        public Household? HouseholdOwned { get; set; }

        public int? state {  get; set; }

        

    }
}
