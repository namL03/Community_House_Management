using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community_House_Management.ModelsDb
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<EventProperty>? EventProperties { get; set; }
        public DateTime timeStart { get; set; }
        public DateTime timeEnd { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }
    }
}
