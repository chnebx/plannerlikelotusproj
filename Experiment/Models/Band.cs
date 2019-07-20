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
    [Table("Bands")]
    public class Band : INotifyPropertyChanged
    {
        private string _name;
        private ObservableCollection<Musician> _musicians;
        private ObservableCollection<Formule> _formules;
        private ObservableCollection<Event> _events;

        public Band()
        {
            _musicians = new ObservableCollection<Musician>();
            _formules = new ObservableCollection<Formule>();
            _events = new ObservableCollection<Event>();
        }

        [PrimaryKey, AutoIncrement, Unique, NotNull]
        public int Id { get; set; }

        [Unique, NotNull]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead)]
        public ObservableCollection<Musician> Musicians
        {
            get
            {
                return _musicians;
            }
            set
            {
                _musicians = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Musicians"));
            }
        }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead)]
        public ObservableCollection<Formule> Formules
        {
            get
            {
                return _formules;
            }
            set
            {
                _formules = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Formules"));
            }
        }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead)]
        public ObservableCollection<Event> Events
        {
            get
            {
                return _events;
            }
            set
            {
                _events = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Events"));
            }
        }

        public void addMusician(Musician musician)
        {
            _musicians.Add(musician);
        }

        public void removeMusician(Musician musician)
        {
            int index = _musicians.IndexOf(musician);
            if (index != -1){
                _musicians.RemoveAt(index);
            }
        }

        public void addFormule(Formule formule)
        {
            _formules.Add(formule);
        }

        public void removeFormule(Formule formule)
        {
            int index = _formules.IndexOf(formule);
            if (index != -1)
            {
                _formules.RemoveAt(index);
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}
