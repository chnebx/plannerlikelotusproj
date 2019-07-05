using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Models
{
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
        private List<Event> _filteredEvents;
        
        private ObservableCollection<Event> _events;

        public EventStack(Day current)
        {
            _current = current;
            _row = current.Row;
            _column = current.Column;
            _monthRow = current.MonthRow;
            _monthColumn = current.MonthColumn;
            _rowSpan = 2;
            _colSpan = 1;
            _events = new ObservableCollection<Event>();
            _dayNumber = current.Date.Day.ToString();
            _filteredEvents = new List<Event>();
        }

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
                _dayNumber = _current.Date.Day.ToString();
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Current"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Row"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Column"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("MonthRow"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("MonthColumn"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DayNum"));
            }
        }

        public int Row
        {
            get
            {
                return _row;
                //return Current.Row;
            }
            set
            {
                _row = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Row"));
            }
        }

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

        public int Column
        {
            get
            {
                return _column;
            }
            set
            {
                _column = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Column"));
            }
        }

        public int MonthRow
        {
            get
            {
                return _monthRow;
            }
            set
            {
                _monthRow = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("MonthRow"));
            }
        }

        public int MonthColumn
        {
            get
            {
                return _monthColumn;
            }
            set
            {
                _monthColumn = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("MonthColumn"));
            }
        }

        public bool IsFull
        {
            get
            {
                return Events.Count >= 3;
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
                newEvent.Column = Events.Count;
                newEvent.updateDates(Current.Date.Year, Current.Date.Month, Current.Date.Day);
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
                    FilteredEvents.Add(Events[i]);
                    Events[i].IsFilterResult = true;
                    return true;
                }
            }
            return false;
        }

        public void clearFilter()
        {
            for(int i = 0; i < FilteredEvents.Count; i++)
            {
                FilteredEvents[i].IsFilterResult = false;
            }
            _filteredEvents.Clear();
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

        public ObservableCollection<Event> Events
        {
            get
            {
                return _events;
            }
            set
            {
                _events = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("EvtStackEvts"));
            }
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
