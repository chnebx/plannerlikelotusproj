using Experiment.Utilities;
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
        //private int _colSpan;
        private string _dayNumber;
        private Event _crossingEvt = null;
        private bool _isFilterResult;
        private DateTime _lowerLimitHour;
        private DateTime _upperLimitHour;
        private List<Event> _filteredEvents;
        private ObservableCollection<Event> _events;
        private Event _upperLimitEvent;
        private Event _lowerLimitEvent;
        private DateTime _EventStackDay;
        private int _spanLength;
        private bool _isOverlapping = false;
        
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
        public DateTime EventStackDay {
            get
            {
                return _EventStackDay;
            }
            set
            {
                _EventStackDay = value;
                _dayNumber = _EventStackDay.Day.ToString();
                _lowerLimitHour = new DateTime(EventStackDay.Year, EventStackDay.Month, EventStackDay.Day, 0, 0, 0);
                _upperLimitHour = _lowerLimitHour.AddDays(1).AddHours(12);
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("EventStackDay"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Row"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Column"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("MonthRow"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("MonthColumn"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Current"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DayNumber"));
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

        public bool IsOverlapping
        {
            get
            {
                return _isOverlapping;
            }
            set
            {
                _isOverlapping = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsOverlapping"));
            }
        }

        [Ignore]
        public int SpanLength
        {
            get
            {
                if (_spanLength == 0)
                {
                    _spanLength = CalculateSpanLength();
                }
                return _spanLength;
            }
            set
            {
                _spanLength = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SpanLength"));
            }
        }

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
        public bool IsFull
        {
            get
            {
                return Events.Count >= 3;
            }
        }

        [Ignore]
        public string DayNumber
        {
            get
            {
                return EventStackDay.Date.Day.ToString();
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

        public static EventStack Clone(EventStack evtStack)
        {
            EventStack cloned = new EventStack
            {
                EventStackDay = evtStack.EventStackDay
            };
            foreach(Event evt in evtStack.Events)
            {
                cloned.AddEvent(evt);
            }
            return cloned;
        }

        [Ignore]
        public Event LowerLimitEvent
        {
            get
            {
                return _lowerLimitEvent;
            }
            set
            {
                _lowerLimitEvent = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("LowerLimitEvent"));
            }
        }

        [Ignore]
        public Event UpperLimitEvent
        {
            get
            {
                return _upperLimitEvent;
            }
            set
            {
                _upperLimitEvent = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("UpperLimitEvent"));
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

        public int CalculateSpanLength()
        {
            double val = 6 / Events.Count;
            int spanLength = (int)Math.Floor(val);
            return spanLength;
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
                newEvent.updateDates(EventStackDay.Year, EventStackDay.Month, EventStackDay.Day);
                Events.Add(newEvent);
                updateEvts();
                UpdateIsOverlapping();
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
            if (Events.Count > 0)
            {
                UpdateIsOverlapping();
            }
            updateEvts();
        }

        

        public bool IsClashingEvent(Event evt)
        {
            if (evt != null)
            {
                for (int i = 0; i < Events.Count; i++)
                {
                    if (Events[i].Clashes(evt) == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Dictionary<Event, List<Event>> FindClash(object evt)
        {
            Dictionary<Event, List<Event>> foundEvents = new Dictionary<Event, List<Event>>();
            //Gathering lowerLimit and UpperLimit
            EventsUtilities.UpdateLimits(this);
            if (evt is Event)
            {
                Event e = (Event)evt;
                foundEvents.Add(e, new List<Event>());
                if (evt != null)
                {
                    if (LowerLimitEvent != null)
                    {
                        DateTime start = new DateTime(this.EventStackDay.Year, this.EventStackDay.Month, this.EventStackDay.Day, e.Start.Hour, e.Start.Minute, 0);
                        DateTime end = start.Add(e.End - e.Start);
                        if (LowerLimitEvent.Clashes(start, end))
                        {
                            //foundIndices.Add(LowerLimitEvent);
                            foundEvents[e].Add(LowerLimitEvent);
                        }
                    }
                    if (UpperLimitEvent != null)
                    {
                        DateTime start = new DateTime(this.EventStackDay.Year, this.EventStackDay.Month, this.EventStackDay.Day, e.Start.Hour, e.Start.Minute, 0);
                        DateTime end = start.Add(e.End - e.Start);
                        if (UpperLimitEvent.Clashes(start, end))
                        {
                            foundEvents[e].Add(UpperLimitEvent);
                        }
                    }
                    for (int i = 0; i < Events.Count; i++)
                    {
                        if (Events[i].Clashes(e))
                        {
                            foundEvents[e].Add(Events[i]);
                        }
                    }
                }
            }
            else if (evt is EventStack)
            {
                EventStack stack = (EventStack)evt;
                if (stack != null)
                {
                    for (int k = 0; k < stack.Events.Count; k++)
                    {
                        foundEvents.Add(stack.Events[k], new List<Event>());
                        if (LowerLimitEvent != null)
                        {
                            DateTime start = new DateTime(this.EventStackDay.Year, this.EventStackDay.Month, this.EventStackDay.Day, stack.Events[k].Start.Hour, stack.Events[k].Start.Minute, 0);
                            DateTime end = start.Add(stack.Events[k].End - stack.Events[k].Start);
                            if (LowerLimitEvent.Clashes(start, end))
                            {
                                foundEvents[stack.Events[k]].Add(LowerLimitEvent);
                            }

                        }
                        if (UpperLimitEvent != null)
                        {
                            DateTime start = new DateTime(this.EventStackDay.Year, this.EventStackDay.Month, this.EventStackDay.Day, stack.Events[k].Start.Hour, stack.Events[k].Start.Minute, 0);
                            DateTime end = start.Add(stack.Events[k].End - stack.Events[k].Start);
                            if (UpperLimitEvent.Clashes(start, end))
                            {
                                foundEvents[stack.Events[k]].Add(UpperLimitEvent);
                            }
                        }
                    }
                    for (int i = 0; i < Events.Count; i++)
                    {
                        for (int j = 0; j < stack.Events.Count; j++)
                        {
                            if (Events[i].Clashes(stack.Events[j]))
                            {
                                foundEvents[stack.Events[j]].Add(Events[i]);
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Events.Count - 1; i++)
                {
                    foundEvents.Add(Events[i], new List<Event>());
                    for (int j = i + 1; j < Events.Count; j++)
                    {
                        if (Events[i].Clashes(Events[j]) == true)
                        {
                            foundEvents[Events[i]].Add(Events[j]);
                        }
                    }
                }
            }
            bool containsClashes = false;
            foreach(KeyValuePair<Event, List<Event>> evts in foundEvents)
            {
                if (evts.Value.Count > 0)
                {
                    containsClashes = true;
                }
            }
            if (containsClashes)
            {
                return foundEvents;
            } else
            {
                return new Dictionary<Event, List<Event>>();
            }
            
        }

        public List<Event> CheckClash(object evt)
        {
            HashSet<Event> results = new HashSet<Event>();
            List<Event> foundIndices = new List<Event>();
            //Gathering lowerLimit and UpperLimit
            EventsUtilities.UpdateLimits(this);
            if (evt is Event)
            {
                Event e = (Event)evt;
                if (evt != null)
                {
                    if (LowerLimitEvent != null)
                    {
                        DateTime start = new DateTime(this.EventStackDay.Year, this.EventStackDay.Month, this.EventStackDay.Day, e.Start.Hour, e.Start.Minute, 0);
                        DateTime end = start.Add(e.End - e.Start);
                        if (LowerLimitEvent.Clashes(start, end))
                        {
                            foundIndices.Add(LowerLimitEvent);
                        }
                    }
                    if (UpperLimitEvent != null)
                    {
                        DateTime start = new DateTime(this.EventStackDay.Year, this.EventStackDay.Month, this.EventStackDay.Day, e.Start.Hour, e.Start.Minute, 0);
                        DateTime end = start.Add(e.End - e.Start);
                        if (UpperLimitEvent.Clashes(start, end))
                        {
                            foundIndices.Add(UpperLimitEvent);
                        }
                    }
                    for (int i = 0; i < Events.Count; i++)
                    {
                        if (Events[i].Clashes(e))
                        {
                            foundIndices.Add(Events[i]);
                        }
                    }
                }
            } else if (evt is EventStack)
            {
                EventStack stack = (EventStack)evt;
                if (stack != null)
                {
                    for (int k = 0; k < stack.Events.Count; k++)
                    {
                        if (LowerLimitEvent != null)
                        {
                            DateTime start = new DateTime(this.EventStackDay.Year, this.EventStackDay.Month, this.EventStackDay.Day, stack.Events[k].Start.Hour, stack.Events[k].Start.Minute, 0);
                            DateTime end = start.Add(stack.Events[k].End - stack.Events[k].Start);
                            if (LowerLimitEvent.Clashes(start, end))
                            {
                                foundIndices.Add(LowerLimitEvent);
                            }

                        }
                        if (UpperLimitEvent != null)
                        {
                            DateTime start = new DateTime(this.EventStackDay.Year, this.EventStackDay.Month, this.EventStackDay.Day, stack.Events[k].Start.Hour, stack.Events[k].Start.Minute, 0);
                            DateTime end = start.Add(stack.Events[k].End - stack.Events[k].Start);
                            if (UpperLimitEvent.Clashes(start, end))
                            {
                                foundIndices.Add(UpperLimitEvent);
                            }
                        }
                    }
                    for (int i = 0; i < Events.Count; i++)
                    {
                        for (int j = 0; j < stack.Events.Count; j++)
                        {
                            if (Events[i].Clashes(stack.Events[j]))
                            {
                                foundIndices.Add(Events[i]);
                            }
                        }
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
                            results.Add(Events[j]);
                            conflictFound = true;
                        }
                    }
                    if (conflictFound == true)
                    {
                        results.Add(Events[i]);
                    }
                }
                foundIndices = results.ToList<Event>();
            }
            
            return foundIndices;
        }

        public void updateEventsGrid()
        {
            if (Events.Count > 1)
            {
                int incrementRow = 0;
                double val = 6 / Events.Count;
                SpanLength = (int)Math.Floor(val);
                for (int i = 0; i < Events.Count; i++)
                {
                    Events[i].Row = incrementRow;
                    incrementRow += SpanLength;
                    Events[i].RowSpan = SpanLength;
                }
            }
            else if (Events.Count == 1)
            {
                Events[0].Row = 0;
                Events[0].RowSpan = 6;
                SpanLength = 6;
            }
        }

        public void updateEvts()
        {
            sortEvents();
            updateEventsGrid();
        }

        public bool IsOver2Days()
        {
            if (Events.Last().End.Day != Events.Last().Start.Day)
            {
                return true;
            }
            return false;
        }

        public void UpdateIsOverlapping()
        {
            if (Events.Last().End.Day != Events.Last().Start.Day)
            {
                _isOverlapping = true;
                return;
            }
            _isOverlapping = false;
        }

        public void sortEvents()
        {
            if (Events.Count > 1)
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
}
