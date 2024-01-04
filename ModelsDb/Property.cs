using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community_House_Management.ModelsDb
{
    public class Property
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public ICollection<EventProperty> EventProperties { get; set; } = new List<EventProperty>();

        public int State { get; set; }
    }
}
