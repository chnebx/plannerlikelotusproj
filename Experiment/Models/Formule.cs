using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Models
{
    public class Formule : INotifyPropertyChanged
    {
        private string _name;
        private ObservableCollection<Musician> _musicians;
        private bool _isDefault;
        public Formule(string name)
        {
            _name = name;
            _musicians = new ObservableCollection<Musician>();

        }

        public Formule DeepCopy()
        {
            Formule copiedFormule = (Formule)this.MemberwiseClone();
            ObservableCollection<Musician> newMusicianList = new ObservableCollection<Musician>();
            for (int i = 0; i < Musicians.Count; i++)
            {
                newMusicianList.Add(Musicians[i]);
            }
            return copiedFormule;
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

        public bool IsDefault
        {
            get
            {
                return _isDefault;
            }
            set
            {
                _isDefault = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsDefault"));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
