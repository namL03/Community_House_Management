using Community_House_Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community_House_Management.Models
{
    public class PropertyModel : ViewModelBase
    {
        public int Id { get; set; }
        private string _type;
        public string Type 
        { 
            get { return _type; } 
            set 
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }
    }
}
