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
    public class DBHandler : INotifyPropertyChanged
    {
        //public static ObservableCollection<EventStack> _events = null;
        public static ObservableCollection<EventStack> _events = null;
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
            InitEverything();
        }

        public static void InitEverything()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                conn.CreateTable<Band>();
                conn.CreateTable<Event>();
                conn.CreateTable<EventStack>();
                conn.CreateTable<Musician>();
                conn.CreateTable<Employer>();
                conn.CreateTable<Location>();
                conn.CreateTable<Formule>();

                SQLiteCommand cmd1 = new SQLiteCommand(conn);
                cmd1.CommandText = "SELECT Count(*) FROM Bands";
                int bandRowCount = (cmd1.ExecuteScalar<int>());
                SQLiteCommand cmd2 = new SQLiteCommand(conn);
                cmd2.CommandText = "SELECT Count(*) FROM Employers";
                int employersRowCount = (cmd2.ExecuteScalar<int>());
                SQLiteCommand cmd3 = new SQLiteCommand(conn);
                cmd3.CommandText = "SELECT Count(*) FROM Locations";
                int locationsRowCount = (cmd3.ExecuteScalar<int>());
                SQLiteCommand cmd4 = new SQLiteCommand(conn);
                cmd4.CommandText = "SELECT Count(*) FROM EventStacks";
                int evtStacksRowCount = (cmd1.ExecuteScalar<int>());
                conn.BeginTransaction();
                if (bandRowCount == 0)
                {
                    Band newBand = new Band { Name = "Orchestre Thierry Coudret " };
                    _bands.Add(newBand);
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
                    _formules = formules;
                    conn.InsertWithChildren(newBand, recursive: true);
                }

                if (employersRowCount == 0)
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
                    conn.Insert(new Employer
                    {
                        FirstName = "Pierre",
                        LastName = "Cambroue",
                        PhoneNumber = "0544865172"
                    });
                    conn.Insert(new Employer
                    {
                        FirstName = "Ophélie",
                        LastName = "Cantaro",
                        PhoneNumber = "0533641244"
                    });
                    conn.Insert(new Employer
                    {
                        FirstName = "Josh",
                        LastName = "Groban",
                        PhoneNumber = "0527984125"
                    });
                    conn.Insert(new Employer
                    {
                        FirstName = "Victor",
                        LastName = "Taupin",
                        PhoneNumber = "0555441254"
                    });
                    conn.Insert(new Employer
                    {
                        FirstName = "Samuel",
                        LastName = "Balabaud",
                        PhoneNumber = "0531221710"
                    });
                    conn.Insert(new Employer
                    {
                        FirstName = "Thierry",
                        LastName = "Solard",
                        PhoneNumber = "0533322471"
                    });
                    conn.Insert(new Employer
                    {
                        FirstName = "Vincent",
                        LastName = "Ernest",
                        PhoneNumber = "0565650220"
                    });
                    conn.Insert(new Employer
                    {
                        FirstName = "Hector",
                        LastName = "Barbin",
                        PhoneNumber = "0641802090"
                    });
                    conn.Insert(new Employer
                    {
                        FirstName = "Julian",
                        LastName = "Creteau",
                        PhoneNumber = "0587952120"
                    });
                    conn.Insert(new Employer
                    {
                        FirstName = "Valentin",
                        LastName = "Marmin",
                        PhoneNumber = "0510111836"
                    });
                    conn.Insert(new Employer
                    {
                        FirstName = "Julie",
                        LastName = "Hazard",
                        PhoneNumber = "0511227853"
                    });
                }

                if (locationsRowCount == 0)
                {
                    //locations
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
                    conn.Insert(new Location
                    {
                        TownName = "Montmartre",
                        Address = "Rue du Tartre"
                    });
                    conn.Insert(new Location
                    {
                        TownName = "Bordeaux",
                        Address = "Avenue Marc"
                    });
                    conn.Insert(new Location
                    {
                        TownName = "St Vivien",
                        Address = "4 Rue des écoles"
                    });
                    conn.Insert(new Location
                    {
                        TownName = "Niort",
                        Address = "10 Rue de Paris"
                    });
                    conn.Insert(new Location
                    {
                        TownName = "Petit Breuil",
                        Address = "Rue de Marchand"
                    });
                    conn.Insert(new Location
                    {
                        TownName = "Simoussais",
                        Address = "1Place de la Toussaint"
                    });
                    conn.Insert(new Location
                    {
                        TownName = "St Jean d angely",
                        Address = "Avenue Tartuffe"
                    });
                    conn.Insert(new Location
                    {
                        TownName = "Saintes",
                        Address = "Place du Concept"
                    });
                    conn.Insert(new Location
                    {
                        TownName = "Chaille les marais",
                        Address = "Rue Saint Patrick"
                    });
                }
                conn.Commit();
            }
        }

     

        public static void DbInit()
        {
            _bands = new ObservableCollection<Band>();
            _events = new ObservableCollection<EventStack>();
            _formules = new ObservableCollection<Formule>();
            _employers = new ObservableCollection<Employer>();
            FillDB();
        }

        public static ObservableCollection<EventStack> Events
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

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }


        public static ObservableCollection<EventStack> getEvents()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                conn.CreateTable<EventStack>();
                var eventstacks = conn.GetAllWithChildren<EventStack>(recursive: true).ToList<EventStack>();
                return new ObservableCollection<EventStack>(eventstacks);
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
            var beginYear = new DateTime(year, 1, 1);
            var endYear = beginYear.AddYears(1);
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                var events = conn.GetAllWithChildren<EventStack>(x => x.EventStackDay >= beginYear && x.EventStackDay < endYear, recursive:true);
                if (events.Count > 0)
                {
                    return new ObservableCollection<EventStack>(events);
                }
                return new ObservableCollection<EventStack>();
            }
        }

        public static ObservableCollection<EventStack> getEvents(int year, int month)
        {
            var beginMonth = new DateTime(year, month, 1);
            var endMonth = beginMonth.AddMonths(1);
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                var events = conn.GetAllWithChildren<EventStack>((x) => x.EventStackDay >= beginMonth && x.EventStackDay < endMonth, recursive: true);
                if (events.Count > 0)
                {
                    return new ObservableCollection<EventStack>(events);
                }
                return new ObservableCollection<EventStack>();
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
            SQLite.SQLiteAsyncConnection conn = new SQLite.SQLiteAsyncConnection(LoadConnectionString());
            conn.RunInTransactionAsync(nonAsyncConn =>
            {
                nonAsyncConn.InsertWithChildren(evt, recursive: true);
            }
            );
            conn.CloseAsync();

        }

        public static void UpdateEventStack(EventStack evt)
        {

            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                conn.InsertOrReplaceWithChildren(evt, recursive: true);
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
                conn.Commit();
            }
        }

        public static void DeleteEvent(Event evt)
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            {
                conn.CreateTable<Event>();
                conn.Delete<Event>(evt.Id);
            }
        }

        public static void HandleDragEventStack(EventStack FromStack, EventStack ToStack, bool copy = false)
        {
            
            SQLite.SQLiteAsyncConnection conn = new SQLite.SQLiteAsyncConnection(LoadConnectionString());
            conn.RunInTransactionAsync(nonAsyncConn =>
            {
                if (ToStack == null)
                {
                    nonAsyncConn.Execute(
                    "UPDATE EventStacks SET EventStackDay = ?, LowerLimitHour = ?, UpperLimitHour = ? WHERE Id = ?",
                    FromStack.EventStackDay.Ticks,
                    new DateTime(FromStack.EventStackDay.Year, FromStack.EventStackDay.Month, FromStack.EventStackDay.Day, 0, 0, 0),
                    new DateTime(FromStack.EventStackDay.Year, FromStack.EventStackDay.Month, FromStack.EventStackDay.Day, 0, 0, 0).AddDays(1).AddHours(12).Ticks,
                    FromStack.Id
                    );
                    for (int i = 0; i < FromStack.Events.Count; i++)
                    {
                        nonAsyncConn.Execute("UPDATE Events SET Start = ?, End = ? WHERE Id = ?",
                            FromStack.Events[i].Start.Ticks,
                            FromStack.Events[i].End.Ticks,
                            FromStack.Events[i].Id
                            );
                    } 
                } else
                {
                    if (copy)
                    {
                        for (int i = 0; i < ToStack.Events.Count; i++)
                        {
                            if (ToStack.Events[i].EventStackId != ToStack.Id)
                            {
                                Event temp = ToStack.Events[i];
                                temp.EventStackId = ToStack.Id;
                                temp.Id = 0;
                                nonAsyncConn.Insert(temp);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ToStack.Events.Count; i++)
                        {
                            if (ToStack.Events[i].EventStackId != ToStack.Id)
                            {
                                nonAsyncConn.Execute("UPDATE Events SET EventStackId = ?, Start = ?, End = ? WHERE EventStackId = ?",
                                    ToStack.Id,
                                    ToStack.Events[i].Start.Ticks,
                                    ToStack.Events[i].End.Ticks,
                                    FromStack.Id
                                    );
                            }
                        }

                        nonAsyncConn.Execute("DELETE FROM EventStacks WHERE Id = ?",
                           FromStack.Id
                           );
                    } 
                }
            }
            );
            conn.CloseAsync();
        }

        public static void HandleDragEvent(EventStack previous, EventStack newOne, Event evt, bool copy = false)
        {
            int dayInterval = 0;
            if (evt.End.Day != evt.Start.Day)
            {
                dayInterval = 1;
            }
            SQLite.SQLiteAsyncConnection conn = new SQLite.SQLiteAsyncConnection(LoadConnectionString());
            conn.RunInTransactionAsync(nonAsyncConn =>
            {
                int newOneId = newOne.Id;
                if (newOneId < 1)
                {
                    nonAsyncConn.Insert(newOne);
                    newOneId = nonAsyncConn.ExecuteScalar<int>("Select last_insert_rowid() as id From EventStacks");
                } 
                if (!copy)
                {
                    nonAsyncConn.Execute(
                    "UPDATE Events SET EventStackId = ?, Start = ?, End = ? WHERE Id = ?",
                    newOneId,
                    new DateTime(newOne.EventStackDay.Year, newOne.EventStackDay.Month, newOne.EventStackDay.Day, evt.Start.Hour, evt.Start.Minute, 0).Ticks,
                    new DateTime(newOne.EventStackDay.Year, newOne.EventStackDay.Month, newOne.EventStackDay.Day, evt.End.Hour, evt.End.Minute, 0).AddDays(dayInterval).Ticks,
                    evt.Id
                    );
                } else
                {
                    evt.EventStackId = newOneId;
                    nonAsyncConn.InsertWithChildren(evt);
                }
                
                if (previous.Events.Count == 0)
                {
                    nonAsyncConn.Execute("DELETE FROM EventStacks WHERE Id = ?", previous.Id);
                }
            }
            );
            conn.CloseAsync();

            //using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(LoadConnectionString()))
            //{
            //    conn.BeginTransaction();
            //    conn.Insert(newOne);
            //    int newOneId = conn.ExecuteScalar<int>("Select last_insert_rowid() as id From EventStacks");
            //    //newOne.AddEvent(evt);
            //    //conn.UpdateWithChildren(newOne);
            //    //conn.Update(evt);
            //    conn.Execute(
            //        "UPDATE Events SET EventStackId = ?, Start = ?, End = ? WHERE Id = ?",
            //        newOneId,
            //        new DateTime(newOne.EventStackDay.Year, newOne.EventStackDay.Month, newOne.EventStackDay.Day, evt.Start.Hour, evt.Start.Minute, 0).Ticks,
            //        new DateTime(newOne.EventStackDay.Year, newOne.EventStackDay.Month, newOne.EventStackDay.Day, evt.End.Hour, evt.End.Minute, 0).AddDays(dayInterval).Ticks,
            //        evt.Id
            //        );
            //    if (previous.Events.Count == 0)
            //    {
            //        conn.Execute("DELETE FROM EventStacks WHERE Id = ?", previous.Id);
            //    }
            //    conn.Commit();
            //}
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
