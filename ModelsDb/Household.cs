using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community_House_Management.ModelsDb
{
    class Household
    {
        public int Id { get; set; }
        public ICollection<Person> Members { get; set; }
    }
}
