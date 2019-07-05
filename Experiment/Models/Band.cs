using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Models
{
    public class Band : INotifyPropertyChanged
    {
        private string _name;
        private ObservableCollection<Musician> _musicians;
        private ObservableCollection<Formule> _formules;

        public Band(string name)
        {
            _name = name;
            _musicians = new ObservableCollection<Musician>();
        }

        public Band DeepCopy()
        {
            Band copiedBand = (Band)this.MemberwiseClone();
            ObservableCollection<Musician> newMusiciansList = new ObservableCollection<Musician>();
            for (int i = 0; i < _musicians.Count; i++)
            {
                newMusiciansList.Add(_musicians[i]);
            }
            ObservableCollection<Formule> newFormulesList = new ObservableCollection<Formule>();
            for (int i = 0; i < _formules.Count; i++)
            {
                newFormulesList.Add(_formules[i]);
            }
            return copiedBand;
        }

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
