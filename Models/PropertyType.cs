using Community_House_Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community_House_Management.Models
{
    public class PropertyType : ViewModelBase
    {
		private string _type;
		private int _count;
		public string Type
		{
			get { return _type; }
			set 
			{
				_type = value;
				OnPropertyChanged(nameof(Type));
			}
		}
        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged(nameof(Count));
            }
        }
    }
}
