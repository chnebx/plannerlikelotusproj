using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Experiment.Models
{
    public class EventBckup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /*
        private Day startDay;
        private Day endDay;
        private int length;
        private ObservableCollection<SubEvent> events;
        private string comment;
        private int employerID;
        private int stackrow;
        private int row;
        private int column;
        private SolidColorBrush colorRect;

        
        
        public EventBckup(Day start, Day end, string comment)
        {
            startDay = start;
            endDay = end;
            Comment = comment;
            colorRect = Brushes.Red;
            stackrow = 0;
            Events = new ObservableCollection<SubEvent>();
            this.BuildEvents();
        }

        public int Row
        {
            get
            {
                return startDay.Row;
            }
            set
            {
                row = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("EvtRow"));
            }
        }

        public int Column
        {
            get { return startDay.Column; }
            set
            {
                column = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("EvtColumn"));
            }
        }

        public Day startingDay
        {
            get { return startDay; }
            set
            {
                startDay = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("StartDay"));
            }
        }

        public Day endingDay
        {
            get { return endDay; }
            set {
                endDay = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("EndDay"));
            }
        }

        public int RealLength
        {
            get { return (endingDay.Date - startingDay.Date).Days + 1; }
            set
            {
                length = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("RealLength"));
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

        public void BuildEvents()
        {
            int monthsNumber = 0;
            if (events == null)
            {
                events = new ObservableCollection<SubEvent>();
            }
            if (startDay.Date.Month != endDay.Date.Month)
            {
                monthsNumber = endingDay.Date.Month - startDay.Date.Month;
                if (monthsNumber < 0)
                {
                    monthsNumber = ((endingDay.Date.Year - startDay.Date.Year) * 12) + endingDay.Date.Month - startDay.Date.Month;
                }
            }
            
            Day fromDay = new Day { Date = startDay.Date };
            if (monthsNumber == 0)
            {
                SubEvent data = new SubEvent 
                {
                    StartingDay = new Day
                    {
                        Date = fromDay.Date
                    },
                    Length = RealLength,
                    parentElement = this
                };
                Events.Add(data);
            }
            else
            {
                int length = DateTime.DaysInMonth(fromDay.Date.Year, fromDay.Date.Month) - fromDay.Date.Day + 1;

                int remainingLength = RealLength;

                do
                {
                    SubEvent data = new SubEvent
                    {
                        StartingDay = new Day
                        {
                            Date = fromDay.Date
                        },
                        Length = length,
                        parentElement = this
                    };
                    Events.Add(data);

                    remainingLength -= length;
                    fromDay.Date = fromDay.Date.AddDays(length);

                    if (DateTime.DaysInMonth(fromDay.Date.Year, fromDay.Date.Month) < remainingLength)
                    {
                        length = DateTime.DaysInMonth(fromDay.Date.Year, fromDay.Date.Month);
                    }
                    else
                    {
                        length = remainingLength;
                    }

                    monthsNumber--;
                } while (monthsNumber >= 0);

            }

        }

        public ObservableCollection<SubEvent> Events
        {
            get { return events; }
            set
            {
                events = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Events"));
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

        public int EmployerID
        {
            get { return employerID; }
            set
            {
                employerID = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("EmployerID"));
            }
        }

        public int stackRow
        {
            get { return stackrow; }
            set
            {
                stackrow = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("secondRow"));
            }
        }
        */
    }
}
