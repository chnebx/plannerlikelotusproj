using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Models
{
    [Table("Musicians")]
    public class Musician : INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;
        private string _phoneNumber;
        private ObservableCollection<Formule> _CurrentFormules;

        public Musician()
        {

        }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [ForeignKey(typeof(Band))]
        public int BandID { get; set; }

        [ManyToOne]
        public Band CurrentBand { get; set; }

        [ManyToMany(typeof(FormulesMusicians))]
        public ObservableCollection<Formule> CurrentFormules
        {
            get
            {
                return _CurrentFormules;
            }
            set
            {
                _CurrentFormules = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CurrentFormules"));
            }
        }

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

        public string PhoneNumber
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
