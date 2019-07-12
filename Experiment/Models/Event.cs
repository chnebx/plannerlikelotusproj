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

        public Event(Band band, uint hour, uint minutes, uint endHour, uint endMinutes, string name, Location locationName = null)
        {
            if (hour > 23 || minutes > 59 || endHour > 23 || endMinutes > 59)
            {
                throw new ArgumentException("Invalid time specified");
            }
            _hour = (int)hour;
            _minutes = (int)minutes;
            _endHour = (int)endHour;
            _endMinutes = (int)endMinutes;
            _band = band;
            int totalAmount = (_endHour * 60 + _endMinutes) - (_hour * 60 + _minutes);
            _lengthHour = totalAmount / 60;
            _lengthMinutes = totalAmount - ((totalAmount / 60) * 60);
            _name = name;
            _rowSpan = 6;
            _row = 0;
            //_employer = actualEmployer;
            /*
            if (actualEmployer == null)
            {
                _employer = new Employer("", "", "");
            }
            */
            colorRect = new SolidColorBrush(Color.FromRgb(
                        (byte)randomColor.Next(0, 255),
                        (byte)randomColor.Next(0, 255),
                        (byte)randomColor.Next(0, 255)));
            employerID = 1;
            if (locationName != null)
            {
                _location = locationName;
            }

            _showHour = hour.ToString("00");
            _showMinutes = minutes.ToString("00");
            _showEndHour = endHour.ToString("00");
            _showEndMinutes = endMinutes.ToString("00");
            _formule = new Formule("");
            _over2Days = false;
            comment = "";
        }

        public Event(Band band, DateTime start, DateTime end, string name, Location locationName = null)
        {
            /*
            _hour = (int)hour;
            _minutes = (int)minutes;
            _endHour = (int)endHour;
            _endMinutes = (int)endMinutes;
            */
            _band = band;
            _eventStart = start;
            _eventEnd = end;
            TimeSpan diff = _eventEnd - _eventStart;
            _lengthHour = (int)diff.TotalMinutes / 60;
            _lengthMinutes = (int)diff.TotalMinutes % 60;
            /*
            int totalAmount = (_endHour * 60 + _endMinutes) - (_hour * 60 + _minutes);
            _lengthHour = totalAmount / 60;
            _lengthMinutes = totalAmount - ((totalAmount / 60) * 60);
            */
            _name = name;
            _rowSpan = 6;
            _row = 0;
            _shortName = ShortNameMaker(_name);
            //_employer = actualEmployer;
            /*
            if (actualEmployer == null)
            {
                _employer = new Employer("", "", "");
            }
            */
            colorRect = new SolidColorBrush(Color.FromRgb(
                        (byte)randomColor.Next(0, 255),
                        (byte)randomColor.Next(0, 255),
                        (byte)randomColor.Next(0, 255)));
            employerID = 1;
            if (locationName != null)
            {
                _location = locationName;
            }
            /*
            _showHour = hour.ToString("00");
            _showMinutes = minutes.ToString("00");
            _showEndHour = endHour.ToString("00");
            _showEndMinutes = endMinutes.ToString("00");
            */
            _formule = new Formule("");
            _over2Days = false;
            comment = "";
            updateDuration();
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

        public string ShortNameMaker(string name)
        {
            if (name.Length > 7)
            {
                return name.Substring(0, 6) + "...";
            } else
            {
                return name;
            }
        }

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

        public void updateDates(int year, int month, int day)
        {
            this.Start = new DateTime(year, month, day, Start.Hour, Start.Minute, Start.Second);
            this.End = Start.AddMinutes(_lengthHour * 60 + _lengthMinutes);
        }

        public EventStack parentStack { get; set; }

        public int Hour
        {
            get
            {
                return _hour;
            }
            set
            {
                _hour = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Hour"));
            }
        }

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

        public int Minutes
        {
            get
            {
                return _minutes;
            }
            set
            {
                _minutes = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Minutes"));
            }
        }

        public string ShowHour
        {
            get
            {
                return String.Format("{0:00}", Start.Hour);
            }
            set
            {
                _showHour = value;
                int hourValue = Int32.Parse(value);
                if (hourValue < 0)
                {
                    hourValue = 24 + hourValue;
                }
                hourValue = hourValue % 24;
                DateTime newStart = new DateTime(Start.Year, Start.Month, Start.Day, hourValue, Start.Minute, Start.Second);
                TimeSpan inc = new TimeSpan(LengthHour, LengthMinutes, 0);
                Start = newStart;
                End = Start.Add(inc);
                updateDuration();
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndMinutes"));
            }
        }



        public string ShowMinutes
        {
            get
            {
                return String.Format("{0:00}", Start.Minute);
            }
            set
            {
                _showMinutes = value;
                int minutesValue = Int32.Parse(value);
                if (minutesValue < 0)
                {
                    minutesValue = 60 + minutesValue;
                }
                minutesValue = minutesValue % 60;
                Start = new DateTime(Start.Year, Start.Month, Start.Day, Start.Hour, minutesValue, Start.Second);
                //updateFinalHour();
                TimeSpan inc = new TimeSpan(LengthHour, LengthMinutes, 0);
                End = Start.Add(inc);
                updateDuration();
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowMinutes"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndMinutes"));
            }
        }

        public string ShowEndHour
        {
            get
            {
                return String.Format("{0:00}", End.Hour);
            }
            set
            {
                _showEndHour = value;
                int endHourValue = Int32.Parse(value);
                if (endHourValue < 0)
                {
                    endHourValue = 24 + endHourValue;
                }
                endHourValue = endHourValue % 24;
                int currentEndDay = End.Day;

                //if (currentEndDay == Start.Day && endHourValue < Start.Hour)
                //{
                //    currentEndDay = Start.AddDays(1).Day;
                //    Console.WriteLine(currentEndDay);
                //}

                if ((endHourValue < Start.Hour || (endHourValue == Start.Hour && End.Minute <= Start.Minute)))
                {
                    if (endHourValue >= parentStack.UpperLimitHour.Hour) // >= 12
                    {
                        if (End.Minute > 0)
                        {
                            endHourValue = parentStack.UpperLimitHour.AddHours(-1).Hour;
                        }
                        else
                        {
                            endHourValue = parentStack.UpperLimitHour.Hour;
                        }
                    }

                    if (currentEndDay == Start.Day)
                    {

                        currentEndDay = Start.AddDays(1).Day;
                        End = Start.AddDays(1);
                    }
                } else {
                    if (currentEndDay > Start.Day)
                    {
                        currentEndDay = Start.Day;
                    }
                }
                End = new DateTime(End.Year, End.Month, currentEndDay, endHourValue, End.Minute, End.Second);
                //if ((End -Start).TotalMinutes < 20)
                //{
                //    Console.WriteLine("hello");
                //}
                //updateLength();
                updateDuration();
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthMinutes"));

            }
        }

        public string ShowEndMinutes
        {
            get
            {
                return String.Format("{0:00}", End.Minute);
            }
            set
            {
                _showEndMinutes = value;
                int endMinutesValue = Int32.Parse(value);
                if (endMinutesValue < 0)
                {
                    endMinutesValue = 60 + endMinutesValue;
                }
                endMinutesValue = endMinutesValue % 60;
                if (End.Day > Start.Day)
                {
                    if (End.Hour == 12 && endMinutesValue > 0)
                    {
                        endMinutesValue = 0;
                    }
                }
                End = new DateTime(End.Year, End.Month, End.Day, End.Hour, endMinutesValue, End.Second);
                updateDuration();
                //updateLength();
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthMinutes"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndMinutes"));

            }
        }


        public string ShowLengthHour
        {
            get
            {
                return String.Format("{0:00}", LengthHour);
            }
            set
            {
                _showLengthHour = value;
                LengthHour = Int32.Parse(value);
                if (LengthHour < 0)
                {
                    LengthHour = 24 + LengthHour;
                }

                LengthHour = LengthHour % 24;
                TimeSpan inc = new TimeSpan(LengthHour, LengthMinutes, 0);
                End = Start.Add(inc);
                updateDuration();
                //updateFinalHour();
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndMinutes"));
            }
        }

        public string ShowLengthMinutes
        {
            get
            {
                return String.Format("{0:00}", LengthMinutes);
            }
            set
            {
                _showLengthMinutes = value;
                LengthMinutes = Int32.Parse(value);
                if (LengthMinutes < 0)
                {
                    LengthMinutes = 60 + LengthMinutes;
                }
                LengthMinutes = LengthMinutes % 60;
                TimeSpan inc = new TimeSpan(LengthHour, LengthMinutes, 0);
                End = Start.Add(inc);
                updateDuration();
                //updateFinalHour();
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthMinutes"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndMinutes"));
            }
        }

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

        public Event DeepCopy()
        {
            //ActualEmployer.ShallowCopy(),
            /*
            Event copiedEvent = new Event(
                _band,
                (uint)Hour,
                (uint)Minutes,
                (uint)EndHour,
                (uint)EndMinutes,
                Name,
                _location
                );
            //copiedEvent.CurrentFormule = CurrentFormule.DeepCopy();
            copiedEvent.CurrentFormule = _formule;
            copiedEvent.ActualEmployer = _employer;
            copiedEvent.Comment = Comment;
            copiedEvent.LengthHour = LengthHour;
            copiedEvent.LengthMinutes = LengthMinutes;
            copiedEvent.ColorFill = ColorFill;
            return copiedEvent;
            */
            Event copiedEvent = new Event(
                _band,
                Start,
                End,
                Name,
                _location
                );
            //copiedEvent.CurrentFormule = CurrentFormule.DeepCopy();
            copiedEvent.CurrentFormule = _formule;
            copiedEvent.ActualEmployer = _employer;
            copiedEvent.Comment = Comment;
            copiedEvent.LengthHour = LengthHour;
            copiedEvent.LengthMinutes = LengthMinutes;
            copiedEvent.ColorFill = ColorFill;
            return copiedEvent;
        }

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

        public int EndHour
        {
            get
            {
                return _endHour;
            }
            set
            {
                _endHour = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("EndHour"));
            }
        }

        public int EndMinutes
        {
            get
            {
                return _endMinutes;
            }
            set
            {
                _endMinutes = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("EndMinutes"));
            }
        }

        //private void updateFinalHour()
        //{
        //    End = Start.AddMinutes(_lengthHour * 60 + _lengthMinutes);
        //    updateDuration();
        //}

        //public void updateLength()
        //{
        //    updateDuration();
        //}

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

        public string DisplayHour
        {
            get
            {
                //return _hour.ToString("00") + "h" + _minutes.ToString("00");
                return _showHour + " h " + _showMinutes;
            }
        }

        public string DisplayLastHour
        {
            get
            {
                return _showEndHour + "h" + _showEndMinutes;
            }
        }

        public SolidColorBrush ColorFill
        {
            get { return colorRect; }
            set
            {
                colorRect = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ColorFill"));
            }
        }

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

        public int EmployerID
        {
            get { return employerID; }
            set
            {
                employerID = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("EmployerID"));
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

        public int Row
        {
            get { return _row; }
            set
            {
                _row = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Row"));
            }
        }

        public int Column
        {
            get { return _column; }
            set
            {
                _column = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Column"));
            }
        }

        public int ColumnSpan
        {
            get { return _columnSpan; }
            set
            {
                _columnSpan = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ColumnSpan"));
            }
        }

        public int RowSpan
        {
            get { return _rowSpan; }
            set
            {
                _rowSpan = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("RowSpan"));
            }
        }

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

        public bool Clashes(Event evt)
        {
            return evt.Start>= this.Start && evt.Start <= this.End ||
            evt.End >= this.Start && evt.End <= this.End ||
            evt.Start >= this.Start && evt.End <= this.End ||
            evt.Start <= this.Start && evt.End >= this.End;
        }

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

    }
}
