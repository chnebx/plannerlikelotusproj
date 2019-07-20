using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Models
{
    [Table("EventStacks")]
    public class EventStack : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Day _current;
        private int _row;
        private int _column;
        private int _monthRow;
        private int _monthColumn;
        private bool _isFull;
        private int _columnSpan;
        private int _rowSpan;
        private int _colSpan;
        private string _dayNumber;
        private Event _crossingEvt = null;
        private bool _isFilterResult;
        private DateTime _lowerLimitHour;
        private DateTime _upperLimitHour;
        private List<Event> _filteredEvents;
        private ObservableCollection<Event> _events;

        
        public EventStack()
        {

            //_current = current;
            //_row = current.Row;
            //_column = current.Column;
            //_monthRow = current.MonthRow;
            //_monthColumn = current.MonthColumn;
            //_rowSpan = 2;
            //_colSpan = 1;
            //_lowerLimitHour = new DateTime(_current.Date.Year, _current.Date.Month, _current.Date.Day, 0, 0, 0);
            //_upperLimitHour = _lowerLimitHour.AddDays(1).AddHours(12);
            _events = new ObservableCollection<Event>();
            //_dayNumber = current.Date.Day.ToString();
            _filteredEvents = new List<Event>();
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public DateTime EventStackDay { get; set; }

        [Ignore]
        public string DayNumber
        {
            get
            {
                return EventStackDay.Date.Day.ToString();
            }

        }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public ObservableCollection<Event> Events
        {
            get
            {
                return _events;
            }
            set
            {
                _events = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Events"));
            }
        }

        [Ignore]
        public bool IsFull
        {
            get
            {
                return Events.Count >= 3;
            }
        }

        [Ignore]
        public Day Current {
            get
            {
                return _current;
            }
            set
            {
                _current = value;
                _row = _current.Row;
                _column = _current.Column;
                _monthRow = _current.MonthRow;
                _monthColumn = _current.MonthColumn;
                _lowerLimitHour = new DateTime(_current.Date.Year, _current.Date.Month, _current.Date.Day, 0, 0, 0);
                _upperLimitHour = _lowerLimitHour.AddDays(1).AddHours(12);
                EventStackDay = _current.Date;
                _dayNumber = _current.Date.Day.ToString();
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Current"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Row"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Column"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("MonthRow"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("MonthColumn"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DayNum"));
            }
        }

        [Ignore]
        public DateTime LowerLimitHour
        {
            get
            {
                return _lowerLimitHour;
            }
            set
            {
                _lowerLimitHour = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("LowerLimitHour"));
            }
        }

        [Ignore]
        public DateTime UpperLimitHour
        {
            get
            {
                return _upperLimitHour;
            }
            set
            {
                _upperLimitHour = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("LowerLimitHour"));
            }
        }

        [Ignore]
        public Event CrossingEvt
        {
            get
            {
                return _crossingEvt;
            }
            set
            {
                _crossingEvt = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CrossingEvt"));
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
        public List<Event> FilteredEvents
        {
            get
            {
                return _filteredEvents;
            }
            set
            {
                _filteredEvents = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("FilteredEvents"));
            }
        }

        [Ignore]
        public string DayNum
        {
            get
            {
                return _dayNumber; 
            }
            set
            {
                _dayNumber = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DayNum"));
            }
        }

        
        

        [Ignore]
        public int RowSpan
        {
            get
            {
                return _rowSpan;
            }
            set
            {
                _rowSpan = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("RowSpan"));
            }
        }

        [Ignore]
        public int Row
        {
            get
            {
                
                return Day.CalculateYearRow(EventStackDay);
                //return Current.Row;
            }
            set
            {
                _row = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Row"));
            }
        }

        [Ignore]
        public int Column
        {
            get
            {
                //return _column;
                return Day.CalculateYearColumn(EventStackDay);
            }
            set
            {
                _column = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Column"));
            }
        }

        [Ignore]
        public int MonthRow
        {
            get
            {
                //return _monthRow;
                return Day.CalculateMonthRow(EventStackDay);
            }
            set
            {
                _monthRow = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("MonthRow"));
            }
        }

        [Ignore]
        public int MonthColumn
        {
            get
            {
                return Day.CalculateMonthColumn(EventStackDay); ;
            }
            set
            {
                _monthColumn = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("MonthColumn"));
            }
        }

        public void AddEvent(Event newEvent)
        {
            if (newEvent == null || IsFull)
            {
                return;
            }

            if (!IsFull)
            {
                newEvent.parentStack = this;
                //newEvent.Column = Events.Count;
                newEvent.updateDates(EventStackDay.Year, EventStackDay.Month, EventStackDay.Day);
                Events.Add(newEvent);
                updateEvts();
            }
        }

        public bool containsEmployer(string text)
        {
            for(int i = 0; i < Events.Count; i++)
            {
                if (Events[i].ActualEmployer.LastName.ToLower().StartsWith(text.ToLower()) || Events[i].ActualEmployer.FirstName.ToLower().StartsWith(text.ToLower()))
                {
                    FilteredEvents.Add(Events[i]);
                    return true;
                }
            }
            return false;
        }

        public bool FilterEvents(Predicate<Event> filterFunc)
        {

            for (int i = 0; i < Events.Count; i++)
            {
                if (filterFunc(Events[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public List<int> Filter(Predicate<Event> filterFunc)
        {
            List<int> resultsIndices = new List<int>();
            for (int i = 0; i < Events.Count; i++)
            {
                if (filterFunc(Events[i]))
                {
                    resultsIndices.Add(i);
                }
            }
            if (resultsIndices.Count > 0)
            {
                return resultsIndices;
            }
            return null;
        }

        public void clearFilter()
        {
            for (int i = 0; i < Events.Count; i++)
            {
                Events[i].IsFilterResult = false;
            }
        }

        public void RemoveEvent(int index)
        {
            if (index < 0 || Events.Count == 0)
            {
                return;
            }

            if (index >= Events.Count)
            {
                Console.WriteLine("index is too high");
            }

            Events.RemoveAt(index);
            updateEvts();
        }

        public List<int> CheckClash(Event evt)
        {
            List<int> foundIndices = new List<int>();
            if (evt != null)
            {
                for (int i = 0; i < Events.Count; i++)
                {
                    if (Events[i].Clashes(evt) == true)
                    {
                        foundIndices.Add(i);
                    }
                }
            }
            else
            {
                bool conflictFound;
                for (int i = 0; i < Events.Count - 1; i++)
                {
                    conflictFound = false;
                    for (int j = i + 1; j < Events.Count; j++)
                    {
                        if (Events[i].Clashes(Events[j]) == true)
                        {
                            conflictFound = true;
                        }
                    }
                    if (conflictFound == true)
                    {
                        foundIndices.Add(i);
                    }
                }
            }
            return foundIndices;
        }

        public void updateEvts()
        {
            if (Events.Count > 1)
            {
                sortEvents();
                int incrementRow = 0;
                double val = 6 / Events.Count;
                int spanLength = (int) Math.Floor(val);
                for (int i = 0; i < Events.Count; i++)
                {
                    Events[i].Row = incrementRow;
                    incrementRow += spanLength;
                    Events[i].RowSpan = spanLength;
                }
            } else if (Events.Count == 1)
            {
                Events[0].RowSpan = 6;
                Events[0].Row = 0;
            }
        }

        public bool IsOver2Days()
        {
            if (Events.Last().End.Day != Events.Last().Start.Day)
            {
                return true;
            }
            return false;
        }

        public void sortEvents()
        {
            int i = 0;
            int j = 0;
            int flag = 0;
            Event val;
            int n = Events.Count;
            for (i = 1; i < n; i++)
            {
                val = Events[i];
                flag = 0;
                for (j = i - 1; j >= 0 && flag != 1;)
                {
                    if (val.Start < Events[j].Start)
                    {
                        Events[j + 1] = Events[j];
                        j--;
                        Events[j + 1] = val;
                    }
                    else flag = 1;
                }
            }
        }

    }
}
