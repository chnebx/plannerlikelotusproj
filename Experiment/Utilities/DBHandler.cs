using Experiment.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Utilities
{
    public class DBHandler: INotifyPropertyChanged
    {
        public static ObservableCollection<EventStack> _events = null;
        public static ObservableCollection<Employer> _employers = null;
        public static ObservableCollection<Formule> _formules = null;
        public static ObservableCollection<Band> _bands = null;
        public static ObservableCollection<Location> _locations = null;
        public static bool _isLoaded = false;
        private static DBHandler _instance = null;

        public static DBHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DBHandler();
                }
                return _instance;
            }
        }

        public static void InitEmployers()
        {
            _employers = new ObservableCollection<Employer>();
            _employers.Add(new Employer("Marc", "Barbaud", "0546841224"));
            _employers.Add(new Employer("Julien", "Chard", "0546285314"));
            _employers.Add(new Employer("Auguste", "Delanoe", "0525445201"));
            _employers.Add(new Employer("Pierre", "Cambroue", "0544865172"));
            _employers.Add(new Employer("Ophélie", "Cantaro", "0533641244"));
            _employers.Add(new Employer("Josh", "Groban", "0527984125"));
            _employers.Add(new Employer("Victor", "Taupin", "0555441254"));
            _employers.Add(new Employer("Samuel", "Balabaud", "0531221710"));
            _employers.Add(new Employer("Thierry", "Solard", "0533322471"));
            _employers.Add(new Employer("Vincent", "Ernest", "0565650220"));
            _employers.Add(new Employer("Hector", "Barbin", "0641802090"));
            _employers.Add(new Employer("Julian", "Creteau", "0587952120"));
            _employers.Add(new Employer("Valentin", "Marmin", "0510111836"));
            _employers.Add(new Employer("Julie", "Hazard", "0511227853"));
        }

        public static void InitFormules()
        {
            _formules = new ObservableCollection<Formule>();
            _formules.Add(new Formule("Thé Dansant"));
            _formules.Add(new Formule("Mariage"));
            _formules.Add(new Formule("Anniversaire"));
        }

        public static void InitLocations()
        {
            _locations = new ObservableCollection<Location>();
            _locations.Add(new Location("Châtellereau", "5 rue du Bois"));
            _locations.Add(new Location("Savinien", "Venelle du Marechal"));
            _locations.Add(new Location("St Georges du Bois", "Avenue de Georges"));
            _locations.Add(new Location("Marans", "10 rue Savigneau"));
            _locations.Add(new Location("La Rochelle", "Rue de la plèbe"));
            _locations.Add(new Location("Montmartre", "Rue du Tartre"));
            _locations.Add(new Location("Bordeaux", "Avenue Marc"));
            _locations.Add(new Location("St Vivien", "4 Rue des écoles"));
            _locations.Add(new Location("Niort", "10 Rue de Paris"));
            _locations.Add(new Location("Petit Breuil", "Rue de Marchand"));
            _locations.Add(new Location("Simoussais", "Place de la Toussaint"));
            _locations.Add(new Location("St Jean d angely", "Avenue Tartuffe"));
            _locations.Add(new Location("Saintes", "Place du concept"));
            _locations.Add(new Location("Chaille les marais", "Rue Saint Patrick"));
        }

        public static void DbInit()
        {
            Band TC = new Band("Orchestre Thierry Coudret");
            InitEmployers();
            InitFormules();
            InitLocations();
            _bands = new ObservableCollection<Band>();
            _bands.Add(TC);
            _isLoaded = true;
            TC.Formules = new ObservableCollection<Formule>();
            TC.addFormule(_formules[0]);
            TC.addFormule(_formules[1]);
            TC.addFormule(_formules[2]);
            _events = new ObservableCollection<EventStack>();
            EventStack no1 = new EventStack(new Day(DateTime.Parse("12/10/19")));
            //var evtno1Evt1 = new Event(TC, 10, 05, 11, 08, "anniv", _locations[0]);
            var evtno1Evt1 = new Event(TC, new DateTime(2019, 10, 12, 10, 05, 0), new DateTime(2019, 10, 12, 11, 08, 0), "anniv", _locations[0]);
            evtno1Evt1.CurrentFormule = _formules[0];
            evtno1Evt1.ActualEmployer = _employers[0];
            no1.AddEvent(evtno1Evt1);
            EventStack no2 = new EventStack(new Day(DateTime.Parse("08/05/19")));
            Console.WriteLine(no2.Current.Date.Month);
            //var evtno2Evt1 = new Event(TC, 12, 00, 13, 18, "break", _locations[1]);
            var evtno2Evt1 = new Event(TC, new DateTime(2019, 05, 08, 12, 00, 0), new DateTime(2019, 05, 08, 13, 18, 0), "break", _locations[1]);
            evtno2Evt1.CurrentFormule = _formules[2];
            evtno2Evt1.ActualEmployer = _employers[1];
            //var evtno2Evt2 = new Event(TC, 15, 05, 20, 10, "after", _locations[2]);
            var evtno2Evt2 = new Event(TC, new DateTime(2019, 05, 08, 15, 05, 0), new DateTime(2019, 05, 08, 20, 10, 0), "after", _locations[2]);
            evtno2Evt2.ActualEmployer = _employers[2];
            evtno2Evt2.CurrentFormule = _formules[0];
            no2.AddEvent(evtno2Evt1);
            no2.AddEvent(evtno2Evt2);

            EventStack no3 = new EventStack(new Day(DateTime.Parse("04/01/19")));
            var evtno3Evt1 = new Event(TC, new DateTime(2019, 04, 01, 08, 25, 0), new DateTime(2019, 04, 01, 11, 10, 0), "live", _locations[3]);
            evtno3Evt1.ActualEmployer = _employers[3];
            evtno3Evt1.CurrentFormule = _formules[0];
            no3.AddEvent(evtno3Evt1);
            EventStack no4 = new EventStack(new Day(DateTime.Parse("25/05/19")));
            var evtno4Evt1 = new Event(TC, new DateTime(2019, 05, 25, 07, 00, 0), new DateTime(2019, 05, 25, 09, 00, 0), "concert", _locations[4]);
            var evtno4Evt2 = new Event(TC, new DateTime(2019, 05, 25, 09, 05, 0), new DateTime(2019, 05, 25, 10, 00, 0), "pré-concert", _locations[5]);
            evtno4Evt1.ActualEmployer = _employers[4];
            evtno4Evt2.ActualEmployer = _employers[5];
            evtno4Evt1.CurrentFormule = _formules[1];
            evtno4Evt2.CurrentFormule = _formules[0];
            no4.AddEvent(evtno4Evt1);
            no4.AddEvent(evtno4Evt2);
            EventStack no5 = new EventStack(new Day(DateTime.Parse("10/05/20")));
            var evtno5Evt1 = new Event(TC, new DateTime(2020, 05, 10, 06, 00, 0), new DateTime(2020, 05, 10, 15, 10, 0), "anniv d untel", _locations[6]);
            evtno5Evt1.ActualEmployer = _employers[6];
            evtno5Evt1.CurrentFormule = _formules[0];
            no5.AddEvent(evtno5Evt1);
            EventStack no6 = new EventStack(new Day(DateTime.Parse("11/09/20")));
            var evtno6Evt1 = new Event(TC, new DateTime(2020, 09, 11, 08, 10, 0), new DateTime(2020, 09, 11, 09, 54, 0), "Reflexion", _locations[7]);
            var evtno6Evt2 = new Event(TC, new DateTime(2020, 09, 11, 12, 05, 0), new DateTime(2020, 09, 11, 15, 30, 0), "Encore autre chose", _locations[8]);
            evtno6Evt1.ActualEmployer = _employers[7];
            evtno6Evt2.ActualEmployer = _employers[8];
            evtno6Evt1.CurrentFormule = _formules[2];
            evtno6Evt2.CurrentFormule = _formules[0];
            no6.AddEvent(evtno6Evt1);
            no6.AddEvent(evtno6Evt2);
            EventStack no7 = new EventStack(new Day(DateTime.Parse("15/08/19")));
            var evtno7Evt1 = new Event(TC, new DateTime(2019, 08, 15, 04, 00, 0), new DateTime(2019, 08, 15, 05, 00, 0), "Reflexion", _locations[9]);
            var evtno7Evt2 = new Event(TC, new DateTime(2019, 08, 15, 14, 05, 0), new DateTime(2019, 08, 15, 15, 20, 0), "Encore autre chose", _locations[10]);
            var evtno7Evt3 = new Event(TC, new DateTime(2019, 08, 15, 18, 05, 0), new DateTime(2019, 08, 15, 19, 10, 0), "Encore encore autre chose", _locations[11]);
            evtno7Evt1.ActualEmployer = _employers[9];
            evtno7Evt2.ActualEmployer = _employers[10];
            evtno7Evt3.ActualEmployer = _employers[11];
            evtno7Evt1.CurrentFormule = _formules[1];
            evtno7Evt2.CurrentFormule = _formules[2];
            evtno7Evt3.CurrentFormule = _formules[0];
            no7.AddEvent(evtno7Evt1);
            no7.AddEvent(evtno7Evt2);
            no7.AddEvent(evtno7Evt3);
            EventStack no8 = new EventStack(new Day(DateTime.Parse("18/09/19")));
            var evtno8Evt1 = new Event(TC, new DateTime(2019, 09, 18, 18, 24, 0), new DateTime(2019, 09, 18, 19, 10, 0), "Reflexion", _locations[12]);
            var evtno8Evt2 = new Event(TC, new DateTime(2019, 09, 18, 15, 05, 0), new DateTime(2019, 09, 18, 16, 10, 0), "Encore autre chose", _locations[13]);
            evtno8Evt1.ActualEmployer = _employers[12];
            evtno8Evt2.ActualEmployer = _employers[13];
            evtno8Evt1.CurrentFormule = _formules[1];
            evtno8Evt2.CurrentFormule = _formules[0];
            no8.AddEvent(evtno8Evt1);
            no8.AddEvent(evtno8Evt2);

            _events.Add(no1);
            _events.Add(no2);
            _events.Add(no3);
            _events.Add(no4);
            _events.Add(no5);
            _events.Add(no6);
            _events.Add(no7);
            _events.Add(no8);
        }

        public ObservableCollection<EventStack> Events
        {
            get
            {
                return _events;
            }
            set
            {
                _events = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("getEvents"));
            }
        }

        public static ObservableCollection<EventStack> getEvents()
        {
            return _events;
        }

        public static ObservableCollection<EventStack> getEvents(int year)
        {

            return new ObservableCollection<EventStack>(_events.Where<EventStack>((x) => x.Current.Date.Year == year).ToList());
        }

        public static ObservableCollection<EventStack> getEvents(int year, int month)
        {
            if (_isLoaded)
            {
                return new ObservableCollection<EventStack>(_events.Where<EventStack>((x) => x.Current.Date.Year == year && x.Current.Date.Month == month).ToList());
            }
            return new ObservableCollection<EventStack>();

        }


        public static void AddEventStack(EventStack evt)
        {
            _events.Add(evt);
        }

        public static void UpdateEventStack(EventStack evt)
        {
            var item = _events.FirstOrDefault<EventStack>((x) => x.Current.Date == evt.Current.Date);
            if (item != null)
            {
                item = evt;
            }
        }

        public static void DeleteEventStack(EventStack evt)
        {
            var item = _events.FirstOrDefault<EventStack>((x) => x.Current.Date == evt.Current.Date);
            if (item != null)
            {
                _events.Remove(item);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /*
        public void AddEvent(EventStack item)
        {
            _events.Add(item);
        }
        */
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

        public static Band getDefaultBand()
        {
            return _bands[0];
        }

        public static ObservableCollection<Formule> getFormules()
        {
            return _formules;
        }

        public static ObservableCollection<Employer> getEmployers()
        {
            return _employers;
        }

        public static void AddEmployer(Employer employer)
        {
            _employers.Add(employer);
        }

        public ObservableCollection<Location> Locations
        {
            get
            {
                return _locations;
            }
            set
            {
                _locations = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Locations"));
            }
        }

        public static ObservableCollection<Location> getLocations()
        {
            return _locations;
        }

        public static void AddLocation(Location newLocation)
        {
            _locations.Add(newLocation);
        }
    }
}
