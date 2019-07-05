using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

/*
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
        private bool _over2Days;
        private bool _isFilterResult = false;

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

        /* --------------partie à isoler ------------------------------------------
        public Event(Band band, uint hour, uint minutes, int lengthHour, int lengthMinutes, string name, string locationName = null)
        {
            if (hour > 23 || minutes > 59)
            {
                throw new ArgumentException("Invalid time specified");
            }
            _hour = (int)hour;
            _minutes = (int)minutes;
            _lengthHour = lengthHour;
            _lengthMinutes = lengthMinutes;
            _band = band;
            int totalLength = (lengthHour * 60) + lengthMinutes;
            int totalHours = totalLength / 60;
            int totalMinutes = totalLength - (totalHours * 60);
            _endHour = (_hour + totalHours) % 24;
            _endMinutes = (_minutes + totalMinutes) % 60;
            _name = name;
            _rowSpan = 6;
            _row = 0;
            //_employer = actualEmployer;
            
            colorRect = new SolidColorBrush(Color.FromRgb(
                        (byte)randomColor.Next(0, 255),
                        (byte)randomColor.Next(0, 255),
                        (byte)randomColor.Next(0, 255)));
            employerID = 1;
            _location = new Location(locationName, null);
            _showHour = hour.ToString("00");
            _showMinutes = minutes.ToString("00");
            _showEndHour = _endHour.ToString("00");
            _showEndMinutes = _endMinutes.ToString("00");
            _formule = new Formule("");
            _over2Days = false;
            comment = "";
        }
        // ------------------------------------------

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
                return String.Format("{0:00}", Hour);
            }
            set
            {
                _showHour = value;
                Hour = Int32.Parse(value);
                if (Hour < 0)
                {
                    Hour = 24 + Hour;
                }
                Hour = Hour % 24;
                updateFinalHour();
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndMinutes"));
            }
        }

        

        public string ShowMinutes
        {
            get
            {
                return String.Format("{0:00}", Minutes);
            }
            set
            {
                _showMinutes = value;
                Minutes = Int32.Parse(value);
                if (Minutes < 0)
                {
                    Minutes = 60 + Minutes;
                }
                Minutes = Minutes % 60;
                updateFinalHour();
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowMinutes"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndMinutes"));
            }
        }

        public string ShowEndHour
        { 
            get
            {
                return String.Format("{0:00}", EndHour);
            }
            set
            {
                _showEndHour = value;
                EndHour = Int32.Parse(value);
                if (EndHour < 0)
                {
                    EndHour = 24 + EndHour;
                }
                EndHour = EndHour % 24;
                updateLength();
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthMinutes"));

            }
        }

        public string ShowEndMinutes
        {
            get
            {
                return String.Format("{0:00}", EndMinutes);
            }
            set
            {
                _showEndMinutes = value;
                EndMinutes = Int32.Parse(value);
                if (EndMinutes < 0)
                {
                    EndMinutes = 60 + EndMinutes;
                }
                EndMinutes = EndMinutes % 60;
                updateLength();
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowEndMinutes"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthHour"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ShowLengthMinutes"));

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
                updateFinalHour();
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
                updateFinalHour();
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

        private void updateFinalHour()
        {
            int totalLength = (_hour * 60) + _minutes + (_lengthHour * 60) + _lengthMinutes;
            EndHour = ((totalLength / 60) % 24);
            EndMinutes = ((totalLength - (_endHour * 60)) % 60);
        }

        private void updateLength()
        {
            int len = (_endHour * 60 + _endMinutes) - (_hour * 60 + _minutes);
            if (len > 0)
            {
                LengthHour = len / 60;
                LengthMinutes = len - ((len / 60) * 60);
                this.Over2Days = false;
            } else
            {
                int timeUntilMidnight = 1440 - (_hour * 60 + _minutes);
                int hoursUntilMidnight = timeUntilMidnight / 60;
                int minutesUntilMidnight = timeUntilMidnight - ((timeUntilMidnight / 60) * 60);
                LengthHour = hoursUntilMidnight + _endHour;
                LengthMinutes = minutesUntilMidnight + _endMinutes;
                this.Over2Days = true;
            }
        }

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
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Name"));
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

        public int StartHourTotalMinutes
        {
            get
            {
                int hour = Hour;
                int min = Minutes;
                return hour * 60 + min;
            }
        }

        public int EndHourTotalMinutes
        {
            get
            {
                int hour = EndHour;
                int min = EndMinutes;
                if (!Over2Days)
                {
                    return hour * 60 + min;
                } else
                {
                    return (24 - hour) * 60 + (hour * 60) + min;
                }
                
                //
            }
        }

        public int Length
        {
            get
            {
                return EndHourTotalMinutes - StartHourTotalMinutes;
            }
        }

        public bool Clashes(Event evt)
        {
            return
                evt.StartHourTotalMinutes >= StartHourTotalMinutes && evt.StartHourTotalMinutes <= EndHourTotalMinutes ||
                evt.EndHourTotalMinutes >= StartHourTotalMinutes && evt.EndHourTotalMinutes <= EndHourTotalMinutes ||
                evt.StartHourTotalMinutes >= StartHourTotalMinutes && evt.EndHourTotalMinutes <= EndHourTotalMinutes ||
                evt.StartHourTotalMinutes <= StartHourTotalMinutes && evt.EndHourTotalMinutes >= EndHourTotalMinutes
                ;
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
*/