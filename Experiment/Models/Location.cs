using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Models
{
    public class Location : INotifyPropertyChanged
    {
        private string _townName;
        private string _address;
        private string _placeType;
        private string _placeName;
        public static ObservableCollection<string> PlaceTypes = new ObservableCollection<string> { "Salle des fêtes", "Salles de concert", "Exterieur", "Autre" };

        public Location(string townName, string addressName)
        {
            _townName = townName;
            _address = addressName;
            _placeType = PlaceTypes.First();
        }

        public void AddPlaceType(string placeTypeName)
        {
            PlaceTypes.Add(placeTypeName);
        }

        public void RemovePlaceType(string placeTypeName)
        {
            if (PlaceTypes.Contains(placeTypeName))
            {
                PlaceTypes.Remove(placeTypeName);
            }
        }

        public string PlaceType
        {
            get
            {
                return _placeType;
            }
            set
            {
                if (PlaceTypes.Contains(value))
                {
                    _placeType = value;
                    if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("PlaceType"));
                }
            }
        }

        public string PlaceName
        {
            get
            {
                return _placeName; 
            }
            set
            {
                _placeName = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("PlaceName"));
            }
        }

        public string TownName
        {
            get
            {
                return _townName;
            }
            set
            {
                _townName = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("TownName"));
            }
        }

        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Address"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
