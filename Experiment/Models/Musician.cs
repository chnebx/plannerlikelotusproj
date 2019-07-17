using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Models
{
    public class Musician : INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;
        private int _phoneNumber;

        public Musician()
        {

        }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("FirstName"));
            }
        }

        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("LastName"));
            }
        }

        public int PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                _phoneNumber = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("PhoneNumber"));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
