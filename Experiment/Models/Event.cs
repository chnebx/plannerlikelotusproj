using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Experiment.Models
{
    [Table("Events")]
    public class Event : INotifyPropertyChanged
    {
        private int _hour;
        private int _minutes;
        private int _endHour;
        private int _endMinutes;
        private int _lengthHour;
        private int _lengthMinutes;
        private Band _band;
        private int length;
        private ObservableCollection<SubEvent> events;
        private string comment;
        private int employerID;
        private int locationID;
        private int formuleID;
        private int bandID;
        private int _row;
        private bool _isSelected;
        private int _rowSpan;
        private int _columnSpan;
        private int _column;
        private string _name;
        private Location _location;
        private Employer _employer;
        private Formule _formule;
        private string _showHour;
        private string _showMinutes;
        private string _showEndHour;
        private string _showEndMinutes;
        private string _showLengthHour;
        private string _showLengthMinutes;
        private string _shortName;
        private bool _over2Days;
        private bool _isFilterResult = false;
        private DateTime _eventStart;
        private DateTime _eventEnd;
        private double _duration;
        private SolidColorBrush colorRect;
        private static Random randomColor = new Random();

        public event PropertyChangedEventHandler PropertyChanged;

        [Ignore]
        public bool IsValid { get; set; }

        //public Event(Band band, DateTime start, DateTime end, string name, Location locationName = null)
        public Event()
        {
            //_band = band;
            //_eventStart = start;
            //_eventEnd = end;
            //TimeSpan diff = _eventEnd - _eventStart;
            //_lengthHour = (int)diff.TotalMinutes / 60;
            //_lengthMinutes = (int)diff.TotalMinutes % 60;
            //_name = name;
            //_rowSpan = 6;
            //_row = 0;
            //_shortName = ShortNameMaker(_name);
            colorRect = new SolidColorBrush(Color.FromRgb(
                        (byte)randomColor.Next(0, 255),
                        (byte)randomColor.Next(0, 255),
                        (byte)randomColor.Next(0, 255)));
            //employerID = 1;
            //if (locationName != null)
            //{
            //    _location = locationName;
            //}
            //_formule = new Formule("");
            //_over2Days = false;
            //comment = "";
            //updateDuration();
        }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [ForeignKey(typeof(Band))]
        public int BandID { get; set; }

        [ForeignKey(typeof(EventStack))]
        public int EventStackID { get; set; }

        [ManyToOne]
        public EventStack parentStack { get; set; }

        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Comment"));
            }
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
                _shortName = ShortNameMaker(_name);
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShortName"));
            }
        }

        public bool Over2Days
        {
            get
            {
                return _over2Days;
            }
            set
            {
                _over2Days = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Over2Days"));
            }
        }

        [ForeignKey(typeof(Employer))]
        public int EmployerID
        {
            get { return employerID; }
            set
            {
                employerID = value;
                //if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("EmployerID"));
            }
        }

        [ForeignKey(typeof(Location))]
        public int LocationID
        {
            get
            {
                return locationID;
            }
            set
            {
                locationID = value;
                //if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("LocationID"));
            }
        }

        [ForeignKey(typeof(Formule))]
        public int FormuleID
        {
            get
            {
                return formuleID;
            }
            set
            {
                formuleID = value;
                //if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("FormuleID"));
            }
        }

        [OneToOne]
        public Band Band
        {
            get
            {
                return _band;
            }
            set
            {
                _band = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Band"));
            }
        }

        [OneToOne]
        public Employer ActualEmployer
        {
            get
            {
                return _employer;
            }
            set
            {
                _employer = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ActualEmployer"));
            }
        }

        [OneToOne]
        public Location LocationName
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("LocationName"));
            }
        }

        [OneToOne]
        public Formule CurrentFormule
        {
            get
            {
                return _formule;
            }
            set
            {
                _formule = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CurrentFormule"));
            }
        }

        [Ignore]
        public DateTime Start
        {
            get
            {
                return _eventStart;
            }
            set
            {
                _eventStart = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Start"));
            }
        }

        [Ignore]
        public DateTime End
        {
            get
            {
                return _eventEnd;
            }
            set
            {
                _eventEnd = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("End"));
            }
        }


        [Ignore]
        public int LengthHour
        {
            get
            {
                return _lengthHour;
            }
            set
            {
                _lengthHour = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("LengthHour"));
            }
        }

        [Ignore]
        public int LengthMinutes
        {
            get
            {
                return _lengthMinutes;
            }
            set
            {
                _lengthMinutes = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("LengthMinutes"));
            }
        }

        [Ignore]
        public bool IsFilterResult
        {
            get
            {
                return _isFilterResult;
            }
            set
            {
                _isFilterResult = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsFilterResult"));
            }
        }

        [Ignore]
        public double Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Duration"));
            }
        }

        [Ignore]
        public string ShowHour
        {
            get
            {
                return String.Format("{0:00}", Start.Hour);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    int hourValue = Int32.Parse(value);
                    if (hourValue < parentStack.LowerLimitHour.Hour)
                    {
                        hourValue = 24 + hourValue;
                    }
                    //if (hourValue < 0)
                    //{
                    //    hourValue = 0;
                    //}
                    hourValue = hourValue % 24;
                    DateTime newStart = new DateTime(Start.Year, Start.Month, Start.Day, hourValue, Start.Minute, Start.Second);
                    TimeSpan inc = new TimeSpan(LengthHour, LengthMinutes, 0);
                    if (newStart >= parentStack.LowerLimitHour && newStart.Add(inc) <= parentStack.UpperLimitHour)
                    {
                        _showHour = value;
                        Start = newStart;
                        End = Start.Add(inc);
                        updateDuration();
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowHour"));
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndHour"));
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndMinutes"));
                    }
                }
            }
        }


        [Ignore]
        public string ShowMinutes
        {
            get
            {
                return String.Format("{0:00}", Start.Minute);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    int minutesValue = Int32.Parse(value);
                    if (minutesValue < 0)
                    {
                        minutesValue = 60 + minutesValue;
                    }
                    minutesValue = minutesValue % 60;
                    DateTime newStart = new DateTime(Start.Year, Start.Month, Start.Day, Start.Hour, minutesValue, Start.Second);
                    TimeSpan inc = new TimeSpan(LengthHour, LengthMinutes, 0);
                    if (newStart >= parentStack.LowerLimitHour && newStart.Add(inc) <= parentStack.UpperLimitHour)
                    {
                        _showMinutes = value;
                        Start = newStart;
                        End = Start.Add(inc);
                        updateDuration();
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowMinutes"));
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndHour"));
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndMinutes"));
                    }
                }
            }
        }

        [Ignore]
        public string ShowEndHour
        {
            get
            {
                return String.Format("{0:00}", End.Hour);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    int endHourValue = Int32.Parse(value);
                    if (endHourValue < 0)
                    {
                        endHourValue = 24 + endHourValue;
                    }
                    endHourValue = endHourValue % 24;
                    int currentEndDay = End.Day;
                    DateTime newEnd = new DateTime(End.Year, End.Month, End.Day, endHourValue, End.Minute, End.Second);
                    if (newEnd < Start)
                    {
                        if (newEnd.Day == Start.Day)
                        {
                            newEnd = newEnd.AddDays(1);
                        }
                    } else
                    {
                        if (newEnd.Day > Start.Day)
                        {
                            newEnd.AddDays(-1);
                        }
                    }
                    if (newEnd <= parentStack.UpperLimitHour)
                    {
                        End = newEnd;
                        _showEndHour = value;
                        updateDuration();
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndHour"));
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthHour"));
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthMinutes"));
                    }
                }
            }
        }

        [Ignore]
        public string ShowEndMinutes
        {
            get
            {
                return String.Format("{0:00}", End.Minute);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    int endMinutesValue = Int32.Parse(value);
                    if (endMinutesValue < 0)
                    {
                        endMinutesValue = 60 + endMinutesValue;
                    }
                    endMinutesValue = endMinutesValue % 60;
                    DateTime newEnd = new DateTime(End.Year, End.Month, End.Day, End.Hour, endMinutesValue, End.Second);
                    if (newEnd <= parentStack.UpperLimitHour)
                    {
                        _showEndMinutes = value;
                        End = newEnd;
                        updateDuration();
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthHour"));
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthMinutes"));
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndMinutes"));
                    }
                } 
            }
        }

        [Ignore]
        public string ShowLengthHour
        {
            get
            {
                return String.Format("{0:00}", LengthHour);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    int lengthHourValue = Int32.Parse(value);
                    if (lengthHourValue < 0)
                    {
                        lengthHourValue = 0;
                    }
                    lengthHourValue = lengthHourValue % 24;
                    TimeSpan inc = new TimeSpan(lengthHourValue, LengthMinutes, 0);
                    if (Start.Add(inc) <= parentStack.UpperLimitHour)
                    {
                        _showLengthHour = value;
                        //LengthHour = lengthHourValue;
                        End = Start.Add(inc);
                        updateDuration();
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthHour"));
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndHour"));
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndMinutes"));
                    }
                } 
            }
        }

        [Ignore]
        public string ShowLengthMinutes
        {
            get
            {
                return String.Format("{0:00}", LengthMinutes);
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    int lengthMinutesValue = Int32.Parse(value);
                    if (lengthMinutesValue < 0)
                    {
                        lengthMinutesValue = 60 + LengthMinutes;
                    }
                    lengthMinutesValue = lengthMinutesValue % 60;
                    TimeSpan inc = new TimeSpan(LengthHour, lengthMinutesValue, 0);
                    if (Start.Add(inc) <= parentStack.UpperLimitHour)
                    {
                        _showLengthMinutes = value;
                        //LengthMinutes = lengthMinutesValue;
                        End = Start.Add(inc);
                        updateDuration();
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthMinutes"));
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndHour"));
                        if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndMinutes"));
                    }
                }
            }
        }

        [Ignore]
        public string DisplayHour
        {
            get
            {
                return _showHour + " h " + _showMinutes;
            }
        }

        [Ignore]
        public string DisplayLastHour
        {
            get
            {
                return _showEndHour + "h" + _showEndMinutes;
            }
        }

        [Ignore]
        public SolidColorBrush ColorFill
        {
            get { return colorRect; }
            set
            {
                colorRect = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ColorFill"));
            }
        }

        [Ignore]
        public string ShortName
        {
            get
            {
                return _shortName;
            }
            set
            {
                _shortName = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShortName"));
            }
        }

        [Ignore]
        public int Row
        {
            get { return _row; }
            set
            {
                _row = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Row"));
            }
        }

        [Ignore]
        public int Column
        {
            get { return _column; }
            set
            {
                _column = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Column"));
            }
        }

        [Ignore]
        public int ColumnSpan
        {
            get { return _columnSpan; }
            set
            {
                _columnSpan = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ColumnSpan"));
            }
        }

        [Ignore]
        public int RowSpan
        {
            get { return _rowSpan; }
            set
            {
                _rowSpan = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("RowSpan"));
            }
        }

        public void updateDates(int year, int month, int day)
        {
            this.Start = new DateTime(year, month, day, Start.Hour, Start.Minute, Start.Second);
            this.End = Start.AddMinutes(_lengthHour * 60 + _lengthMinutes);
        }

        public Event DeepCopy()
        {
            Event copiedEvent = new Event();
            copiedEvent.Band = _band;
            copiedEvent.Start = Start;
            copiedEvent.End = End;
            copiedEvent.Name = Name;
            copiedEvent.LocationName = _location;
            copiedEvent.CurrentFormule = _formule;
            copiedEvent.ActualEmployer = _employer;
            copiedEvent.Comment = Comment;
            copiedEvent.LengthHour = LengthHour;
            copiedEvent.LengthMinutes = LengthMinutes;
            copiedEvent.ColorFill = ColorFill;
            return copiedEvent;
        }

        public bool Clashes(Event evt)
        {
            return evt.Start>= this.Start && evt.Start <= this.End ||
            evt.End >= this.Start && evt.End <= this.End ||
            evt.Start >= this.Start && evt.End <= this.End ||
            evt.Start <= this.Start && evt.End >= this.End;
        }

        public string ShortNameMaker(string name)
        {
            if (name.Length > 7)
            {
                return name.Substring(0, 6) + "...";
            }
            else
            {
                return name;
            }
        }

        public void updateDuration()
        {
            if (End == null || Start == null)
            {
                Duration = double.NaN;
                return;
            }
            //DateTime compare = new DateTime(End.Year, End.Month, End.Day, End.Hour, End.Minute, 0);
            double length = (End - Start).TotalMinutes;

            if (length < 20 || (End.Day != Start.Day && End.Hour > 12))
            {
                TimeSpan add20 = new TimeSpan(0, 20, 0);
                DateTime diffTime = Start.Add(add20);
                Duration = 20;
                LengthHour = (int)(diffTime - Start).TotalMinutes / 60;
                LengthMinutes = (int)(diffTime - Start).TotalMinutes % 60;
                End = diffTime;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndMinutes"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthMinutes"));
                return;
            }

            if (End >= Start)
            {
                Duration = (End - Start).TotalMinutes;
                LengthHour = (int)(End - Start).TotalMinutes / 60;
                LengthMinutes = (int)(End - Start).TotalMinutes % 60;
            }
            else
            {
                Duration = (End.AddDays(1) - Start).TotalMinutes;
                LengthHour = (int)(End.AddDays(1) - Start).TotalMinutes / 60;
                LengthMinutes = (int)(End.AddDays(1) - Start).TotalMinutes % 60;
            }
        }

    }
}
