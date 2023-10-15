using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community_House_Management.ModelsDb
{
    class OfficialAccount
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int OfficialId { get; set; }
        public Official Official { get; set; }
    }
}
