using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community_House_Management.ModelsDb
{
    public class EventProperty
    {
        public int EventId { get; set; }
        public Event Event { get; set; }
        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
}
