using Experiment.Models;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Utilities
{
    public class DBHandler: INotifyPropertyChanged
    {
        //public static ObservableCollection<EventStack> _events = null;
        public static List<EventStack> _events = null;
        public static ObservableCollection<Employer> _employers = null;
        public static ObservableCollection<Formule> _formules = null;
        public static ObservableCollection<Band> _bands = null;
        public static ObservableCollection<Location> _locations = null;
        public static bool _isLoaded = false;
        private static DBHandler _instance = null;
        private static SQLiteConnection connection;

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

        private static void FillDB()
        {
            InitMusicans();
            InitEmployers();
            InitFormules();
            InitLocations();
        }

        private static void loadDB()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                var formules = conn.Table<Formule>().ToList();
                Console.WriteLine("Formules : " + formules.Count);
                var employers = conn.Table<Employer>().ToList();
                Console.WriteLine("Employers : " + employers.Count);
                var locations = conn.Table<Location>().ToList();
                Console.WriteLine("Employers : " + locations.Count);
            }
        }

        public static void InitEmployers()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {

                var EmployersTable = conn.CreateTable<Employer>();
                SQLiteCommand cmd = new SQLiteCommand(conn);
                cmd.CommandText = "SELECT Count(*) FROM Employers";
                int rowCount = (cmd.ExecuteScalar<int>());
                if (rowCount == 0)
                {
                    conn.Insert(new Employer
                    {
                        FirstName = "Marc",
                        LastName = "Barbaud",
                        PhoneNumber = "0546841224"
                    });
                    conn.Insert(new Employer
                    {
                        FirstName = "Julien",
                        LastName = "Chard",
                        PhoneNumber = "0546285314"
                    });
                    conn.Insert(new Employer
                    {
                        FirstName = "Auguste",
                        LastName = "Delanoe",
                        PhoneNumber = "0525445201"
                    });
                }
            }

            //_employers = new ObservableCollection<Employer>();
            //_employers.Add(new Employer("Marc", "Barbaud", "0546841224"));
            //_employers.Add(new Employer("Julien", "Chard", "0546285314"));
            //_employers.Add(new Employer("Auguste", "Delanoe", "0525445201"));
            //_employers.Add(new Employer("Pierre", "Cambroue", "0544865172"));
            //_employers.Add(new Employer("Ophélie", "Cantaro", "0533641244"));
            //_employers.Add(new Employer("Josh", "Groban", "0527984125"));
            //_employers.Add(new Employer("Victor", "Taupin", "0555441254"));
            //_employers.Add(new Employer("Samuel", "Balabaud", "0531221710"));
            //_employers.Add(new Employer("Thierry", "Solard", "0533322471"));
            //_employers.Add(new Employer("Vincent", "Ernest", "0565650220"));
            //_employers.Add(new Employer("Hector", "Barbin", "0641802090"));
            //_employers.Add(new Employer("Julian", "Creteau", "0587952120"));
            //_employers.Add(new Employer("Valentin", "Marmin", "0510111836"));
            //_employers.Add(new Employer("Julie", "Hazard", "0511227853"));
        }

        public static void InitMusicans()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                var musicians = conn.CreateTable<Musician>();
            }
        }

        public static void InitFormules()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                var FormulesTable = conn.CreateTable<Formule>();
                conn.CreateTable<FormulesMusicians>();
            }
        }

        public static void InitLocations()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                var EmployersTable = conn.CreateTable<Location>();
                SQLiteCommand cmd = new SQLiteCommand(conn);
                cmd.CommandText = "SELECT Count(*) FROM Locations";
                int rowCount = (cmd.ExecuteScalar<int>());
                if (rowCount == 0)
                {
                    conn.Insert(new Location
                    {
                        TownName = "Châtellereau",
                        Address = "5 Rue du Bois"
                    });
                    conn.Insert(new Location
                    {
                        TownName = "Savinien",
                        Address = "Venelle du Marechal"
                    });
                    conn.Insert(new Location
                    {
                        TownName = "St Georges du Bois",
                        Address = "Avenue de Georges"
                    });
                    conn.Insert(new Location
                    {
                        TownName = "Marans",
                        Address = "10 rue Savigneau"
                    });
                    conn.Insert(new Location
                    {
                        TownName = "La Rochelle",
                        Address = "Rue de la plèbe"
                    });
                }
            }
            //_locations = new ObservableCollection<Location>();
            //_locations.Add(new Location("Châtellereau", "5 rue du Bois"));
            //_locations.Add(new Location("Savinien", "Venelle du Marechal"));
            //_locations.Add(new Location("St Georges du Bois", "Avenue de Georges"));
            //_locations.Add(new Location("Marans", "10 rue Savigneau"));
            //_locations.Add(new Location("La Rochelle", "Rue de la plèbe"));
            //_locations.Add(new Location("Montmartre", "Rue du Tartre"));
            //_locations.Add(new Location("Bordeaux", "Avenue Marc"));
            //_locations.Add(new Location("St Vivien", "4 Rue des écoles"));
            //_locations.Add(new Location("Niort", "10 Rue de Paris"));
            //_locations.Add(new Location("Petit Breuil", "Rue de Marchand"));
            //_locations.Add(new Location("Simoussais", "Place de la Toussaint"));
            //_locations.Add(new Location("St Jean d angely", "Avenue Tartuffe"));
            //_locations.Add(new Location("Saintes", "Place du concept"));
            //_locations.Add(new Location("Chaille les marais", "Rue Saint Patrick"));
        }

        public static void DbInit()
        {
            //ClearBand();
            connection = new SQLiteConnection(LoadConnectionString());
            Band TC = new Band
            {
                Name = "Orchestre Thierry Coudret"
            };
            FillDB();
            InsertBand(TC);

            //ClearEvtStacks();
            //loadDB();
            //ClearEvtStacks();
            //Band TC = new Band("Orchestre Thierry Coudret");
            //InitEmployers();
            //InitFormules();
            //InitLocations();
            //_bands = new ObservableCollection<Band>();
            //_bands.Add(TC);
            //_isLoaded = true;
            //TC.Formules = new ObservableCollection<Formule>();
            //TC.addFormule(_formules[0]);
            //TC.addFormule(_formules[1]);
            //TC.addFormule(_formules[2]);
            //_events = new List<EventStack>();
            //EventStack no1 = new EventStack(new Day(DateTime.Parse("12/10/19")));
            //var evtno1Evt1 = new Event(TC, 10, 05, 11, 08, "anniv", _locations[0]);
            //var evtno1Evt1 = new Event(TC, new DateTime(2019, 10, 12, 10, 05, 0), new DateTime(2019, 10, 12, 11, 08, 0), "anniv", _locations[0]);
            //evtno1Evt1.CurrentFormule = _formules[0];
            //evtno1Evt1.ActualEmployer = _employers[0];
            //no1.AddEvent(evtno1Evt1);
            //EventStack no2 = new EventStack(new Day(DateTime.Parse("08/05/19")));
            //var evtno2Evt1 = new Event(TC, 12, 00, 13, 18, "break", _locations[1]);
            //var evtno2Evt1 = new Event(TC, new DateTime(2019, 05, 08, 12, 00, 0), new DateTime(2019, 05, 08, 13, 18, 0), "break", _locations[1]);
            //evtno2Evt1.CurrentFormule = _formules[2];
            //evtno2Evt1.ActualEmployer = _employers[1];
            //var evtno2Evt2 = new Event(TC, 15, 05, 20, 10, "after", _locations[2]);
            //var evtno2Evt2 = new Event(TC, new DateTime(2019, 05, 08, 15, 05, 0), new DateTime(2019, 05, 08, 20, 10, 0), "after", _locations[2]);
            //evtno2Evt2.ActualEmployer = _employers[2];
            //evtno2Evt2.CurrentFormule = _formules[0];
            //no2.AddEvent(evtno2Evt1);
            //no2.AddEvent(evtno2Evt2);

            //EventStack no3 = new EventStack(new Day(DateTime.Parse("04/01/19")));
            //var evtno3Evt1 = new Event(TC, new DateTime(2019, 04, 01, 08, 25, 0), new DateTime(2019, 04, 01, 11, 10, 0), "live", _locations[3]);
            //evtno3Evt1.ActualEmployer = _employers[3];
            //evtno3Evt1.CurrentFormule = _formules[0];
            //no3.AddEvent(evtno3Evt1);
            //EventStack no4 = new EventStack(new Day(DateTime.Parse("25/05/19")));
            //var evtno4Evt1 = new Event(TC, new DateTime(2019, 05, 25, 07, 00, 0), new DateTime(2019, 05, 25, 09, 00, 0), "concert", _locations[4]);
            //var evtno4Evt2 = new Event(TC, new DateTime(2019, 05, 25, 09, 05, 0), new DateTime(2019, 05, 25, 10, 00, 0), "pré-concert", _locations[5]);
            //evtno4Evt1.ActualEmployer = _employers[4];
            //evtno4Evt2.ActualEmployer = _employers[5];
            //evtno4Evt1.CurrentFormule = _formules[1];
            //evtno4Evt2.CurrentFormule = _formules[0];
            //no4.AddEvent(evtno4Evt1);
            //no4.AddEvent(evtno4Evt2);
            //EventStack no5 = new EventStack(new Day(DateTime.Parse("10/05/20")));
            //var evtno5Evt1 = new Event(TC, new DateTime(2020, 05, 10, 06, 00, 0), new DateTime(2020, 05, 10, 15, 10, 0), "anniv d untel", _locations[6]);
            //evtno5Evt1.ActualEmployer = _employers[6];
            //evtno5Evt1.CurrentFormule = _formules[0];
            //no5.AddEvent(evtno5Evt1);
            //EventStack no6 = new EventStack(new Day(DateTime.Parse("11/09/20")));
            //var evtno6Evt1 = new Event(TC, new DateTime(2020, 09, 11, 08, 10, 0), new DateTime(2020, 09, 11, 09, 54, 0), "Reflexion", _locations[7]);
            //var evtno6Evt2 = new Event(TC, new DateTime(2020, 09, 11, 12, 05, 0), new DateTime(2020, 09, 11, 15, 30, 0), "Encore autre chose", _locations[8]);
            //evtno6Evt1.ActualEmployer = _employers[7];
            //evtno6Evt2.ActualEmployer = _employers[8];
            //evtno6Evt1.CurrentFormule = _formules[2];
            //evtno6Evt2.CurrentFormule = _formules[0];
            //no6.AddEvent(evtno6Evt1);
            //no6.AddEvent(evtno6Evt2);
            //EventStack no7 = new EventStack(new Day(DateTime.Parse("15/08/19")));
            //var evtno7Evt1 = new Event(TC, new DateTime(2019, 08, 15, 04, 00, 0), new DateTime(2019, 08, 15, 05, 00, 0), "Reflexion", _locations[9]);
            //var evtno7Evt2 = new Event(TC, new DateTime(2019, 08, 15, 14, 05, 0), new DateTime(2019, 08, 15, 15, 20, 0), "Encore autre chose", _locations[10]);
            //var evtno7Evt3 = new Event(TC, new DateTime(2019, 08, 15, 18, 05, 0), new DateTime(2019, 08, 15, 19, 10, 0), "Encore encore autre chose", _locations[11]);
            //evtno7Evt1.ActualEmployer = _employers[9];
            //evtno7Evt2.ActualEmployer = _employers[10];
            //evtno7Evt3.ActualEmployer = _employers[11];
            //evtno7Evt1.CurrentFormule = _formules[1];
            //evtno7Evt2.CurrentFormule = _formules[2];
            //evtno7Evt3.CurrentFormule = _formules[0];
            //no7.AddEvent(evtno7Evt1);
            //no7.AddEvent(evtno7Evt2);
            //no7.AddEvent(evtno7Evt3);
            //EventStack no8 = new EventStack(new Day(DateTime.Parse("18/09/19")));
            //var evtno8Evt1 = new Event(TC, new DateTime(2019, 09, 18, 18, 24, 0), new DateTime(2019, 09, 18, 19, 10, 0), "Reflexion", _locations[12]);
            //var evtno8Evt2 = new Event(TC, new DateTime(2019, 09, 18, 15, 05, 0), new DateTime(2019, 09, 18, 16, 10, 0), "Encore autre chose", _locations[13]);
            //evtno8Evt1.ActualEmployer = _employers[12];
            //evtno8Evt2.ActualEmployer = _employers[13];
            //evtno8Evt1.CurrentFormule = _formules[1];
            //evtno8Evt2.CurrentFormule = _formules[0];
            //no8.AddEvent(evtno8Evt1);
            //no8.AddEvent(evtno8Evt2);

            //_events.Add(no1);
            //_events.Add(no2);
            //_events.Add(no3);
            //_events.Add(no4);
            //_events.Add(no5);
            //_events.Add(no6);
            //_events.Add(no7);
            //_events.Add(no8);
        }

        //public ObservableCollection<EventStack> Events
        //{
        //    get
        //    {
        //        return _events;
        //    }
        //    set
        //    {
        //        _events = value;
        //        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("getEvents"));
        //    }
        //}
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public List<EventStack> Events
        {
            get
            {
                return _events;
            }
            set
            {
                _events = value;
            }
        }
            
        public static ObservableCollection<EventStack> getEvents()
        {
            //return new ObservableCollection<EventStack>(_events);
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                conn.CreateTable<EventStack>();
                var eventstacks = conn.Table<EventStack>().ToList<EventStack>();

                return new ObservableCollection<EventStack>(eventstacks);
            }

        }

        public static void InsertBand(Band newBand)
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                conn.CreateTable<Band>();
                conn.CreateTable<Event>();
                SQLiteCommand cmd = new SQLiteCommand(conn);
                cmd.CommandText = "SELECT Count(*) FROM Bands";
                int rowCount = (cmd.ExecuteScalar<int>());
                if (rowCount == 0)
                {
                    ObservableCollection<Musician> musicians = new ObservableCollection<Musician>();
                    musicians.Add(new Musician
                    {
                        FirstName = "John",
                        LastName = "Lambda",
                        PhoneNumber = "0745848425"
                    });
                    musicians.Add(new Musician
                    {
                        FirstName = "Marc",
                        LastName = "Chaba",
                        PhoneNumber = "0620103040"
                    });
                    conn.InsertAll(musicians);
                    newBand.Musicians = musicians;
                    ObservableCollection<Formule> formules = new ObservableCollection<Formule>();
                    formules.Add(new Formule
                    {
                        Name = "( Non définie )",
                    });
                    formules.Add(new Formule
                    {
                        Name = "Thé Dansant",
                        Musicians = musicians

                    });
                    formules.Add(new Formule
                    {
                        Name = "Mariage",
                        Musicians = new ObservableCollection<Musician>
                        {
                            musicians[0]
                        }
                    });
                    formules.Add(new Formule
                    {
                        Name = "Anniversaire",
                        Musicians = new ObservableCollection<Musician>
                        {
                            musicians[1]
                        }
                    });
                    conn.InsertAll(formules);
                    newBand.Formules = formules;
                    conn.InsertWithChildren(newBand, recursive:true);
                    //conn.UpdateWithChildren(newBand);
                }
            }
        }

        public static void ClearBand()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                conn.CreateTable<Band>();
                SQLiteCommand cmd = new SQLiteCommand(conn);
                cmd.CommandText = "DELETE FROM Band";
                cmd.ExecuteNonQuery();
            }
        }


        public static void ClearEvtStacks()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                conn.CreateTable<EventStack>();
                SQLiteCommand cmd = new SQLiteCommand(conn);
                cmd.CommandText = "DELETE FROM EventStack";
                cmd.ExecuteNonQuery();
            }
        }

        public static ObservableCollection<EventStack> getEvents(int year)
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                var connection = conn.CreateTable<EventStack>();
                var events = conn.GetAllWithChildren<EventStack>(recursive: true)
                    .Where<EventStack>((x) => x.EventStackDay.Year == year)
                    .ToList<EventStack>();
                if (events.Count > 0)
                {
                    return new ObservableCollection<EventStack>(events);
                }
                return new ObservableCollection<EventStack>();
            }
        }

        public static ObservableCollection<EventStack> getEvents(int year, int month)
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                var connection = conn.CreateTable<EventStack>();
                conn.CreateTable<Event>();
                conn.CreateTable<Employer>();
                conn.CreateTable<Location>();
                conn.CreateTable<Formule>();
                var events = conn.GetAllWithChildren<EventStack>(recursive: true)
                    .Where<EventStack>((x) => x.EventStackDay.Year == year && x.EventStackDay.Month == month)
                    .ToList<EventStack>();
                if (events.Count > 0)
                {
                    return new ObservableCollection<EventStack>(events);
                }
                return new ObservableCollection<EventStack>();
            }

        }

        public static List<EventStack> getEventsInMonth(int year, int month)
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                var events = conn.GetAllWithChildren<EventStack>(recursive: true)
                    .Where<EventStack>((x) => x.EventStackDay.Year == year && x.EventStackDay.Month == month)
                    .ToList<EventStack>();
                if (events.Count > 0)
                {
                    return events;
                }
                return new List<EventStack>();
            }

        }

        public static EventStack getEventStack(int id)
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                var item = conn.GetWithChildren<EventStack>(id);
                return item;
            }

        }


        public static void AddEventStack(EventStack evt)
        {
            //SQLite.SQLiteConnection nonAsyncConn = new SQLite.SQLiteConnection(LoadConnectionString());
            //using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            SQLite.SQLiteAsyncConnection conn = new SQLite.SQLiteAsyncConnection(LoadConnectionString());
            conn.RunInTransactionAsync(nonAsyncConn =>
            {
                nonAsyncConn.InsertWithChildren(evt, recursive: true);
            }
            );
            conn.CloseAsync();
            //conn.InsertWithChildren(evt, recursive:true);
            
        }

        public static void UpdateEventStack(EventStack evt)
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                conn.InsertOrReplaceWithChildren(evt, recursive:true);
            }
        }

        public static void DeleteEventStack(EventStack evt)
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                var deleteEventQuery = "DELETE FROM Events WHERE EventStackID = ?";
                var deleteEventStackQuery = "DELETE FROM EventStacks WHERE Id = ?";
                conn.BeginTransaction();
                conn.Execute(deleteEventQuery, evt.Id);
                conn.Execute(deleteEventStackQuery, evt.Id);
                //conn.Delete<EventStack>(evt.Id);
                conn.Commit();
            }
        }

        public static void DeleteEvent(Event evt)
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                //var deleteQuery = "DELETE FROM Events WHERE Id = ?";
                //conn.Execute(deleteQuery, evt.Id);
                connection.Delete<Event>(evt.Id);
            }
        }

        public static void HandleDragEvent(EventStack previous, EventStack newOne, Event evt)
        {
            int dayInterval = 0;
            if (evt.End.Day != evt.Start.Day)
            {
                dayInterval = 1;
            }
            //SQLite.SQLiteAsyncConnection conn = new SQLite.SQLiteAsyncConnection(LoadConnectionString());
            //conn.RunInTransactionAsync(nonAsyncConn =>
            //{
            //    nonAsyncConn.Insert(newOne);
            //    int newOneId = nonAsyncConn.ExecuteScalar<int>("Select last_insert_rowid() as id From EventStacks");
            //    nonAsyncConn.Execute(
            //        "UPDATE Events SET EventStackId = ?, Start = ?, End = ? WHERE Id = ?",
            //        newOneId,
            //        new DateTime(newOne.EventStackDay.Year, newOne.EventStackDay.Month, newOne.EventStackDay.Day, evt.Start.Hour, evt.Start.Minute, 0).Ticks,
            //        new DateTime(newOne.EventStackDay.Year, newOne.EventStackDay.Month, newOne.EventStackDay.Day, evt.End.Hour, evt.End.Minute, 0).AddDays(dayInterval).Ticks,
            //        evt.Id
            //        );
            //    if (previous.Events.Count == 0)
            //    {
            //        nonAsyncConn.Execute("DELETE FROM EventStacks WHERE Id = ?", previous.Id);
            //    }
            //}
            //);
            //conn.CloseAsync();

            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                conn.BeginTransaction();
                conn.Insert(newOne);
                int newOneId = conn.ExecuteScalar<int>("Select last_insert_rowid() as id From EventStacks");
                //newOne.AddEvent(evt);
                //conn.UpdateWithChildren(newOne);
                //conn.Update(evt);
                conn.Execute(
                    "UPDATE Events SET EventStackId = ?, Start = ?, End = ? WHERE Id = ?",
                    newOneId,
                    new DateTime(newOne.EventStackDay.Year, newOne.EventStackDay.Month, newOne.EventStackDay.Day, evt.Start.Hour, evt.Start.Minute, 0).Ticks,
                    new DateTime(newOne.EventStackDay.Year, newOne.EventStackDay.Month, newOne.EventStackDay.Day, evt.End.Hour, evt.End.Minute, 0).AddDays(dayInterval).Ticks,
                    evt.Id
                    );
                if (previous.Events.Count == 0)
                {
                    conn.Execute("DELETE FROM EventStacks WHERE Id = ?", previous.Id);
                }
                conn.Commit();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


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
            //return _bands[0];
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                conn.CreateTable<Band>();
                Band defaultBand = conn.GetWithChildren<Band>(1, recursive:true);
                return defaultBand;
            }
        }

        public static ObservableCollection<Formule> getFormules()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                conn.CreateTable<Band>();
                conn.CreateTable<Formule>();
                Band actualBand = conn.GetWithChildren<Band>(1);
                Console.WriteLine(actualBand);
                return actualBand.Formules;
            }
        }

        public static ObservableCollection<Employer> getEmployers()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                conn.CreateTable<Employer>();
                var employers = conn.GetAllWithChildren<Employer>().ToList<Employer>();
                return new ObservableCollection<Employer>(employers);
            }
        }

        public static void AddEmployer(Employer employer)
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                conn.CreateTable<Employer>();
                conn.Insert(employer);
            }
        }

        public static ObservableCollection<Location> getLocations()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                conn.CreateTable<Location>();
                var locations = conn.GetAllWithChildren<Location>().ToList<Location>();
                return new ObservableCollection<Location>(locations);
            }
        }

        public static void AddLocation(Location newLocation)
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                conn.CreateTable<Location>();
                conn.Insert(newLocation);
            }
        }   
    }
}
