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
    public class FilterModule : INotifyPropertyChanged
    {
        private GroupFilter _groupFilter = null;
        private bool _isFilterActive = false;
        private bool _areFiltersEmpty;
        public bool connard = false;
        private ObservableCollection<Event> _results = null;
        private ObservableCollection<EventStack> _stacksResult = null;
        private ObservableCollection<EventStack> _providedEvtStackList = null;
        private static FilterModule _instance = null;


        public FilterModule(ObservableCollection<EventStack> events)
        {
            _groupFilter = new GroupFilter();
            _providedEvtStackList = events;
            _areFiltersEmpty = true;
            _results = new ObservableCollection<Event>();
            _stacksResult = new ObservableCollection<EventStack>();
        }

        public FilterModule()
        {
            _groupFilter = new GroupFilter();
            _providedEvtStackList = DBHandler.getEvents();
            _areFiltersEmpty = true;
            connard = true;
            _results = new ObservableCollection<Event>();
            _stacksResult = new ObservableCollection<EventStack>();
        }

        public static FilterModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FilterModule();
                }
                return _instance;
            }
            
        }

        public GroupFilter GF
        {
            get
            {
                return _groupFilter;
            }
            set
            {
                _groupFilter = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("GF"));
            }
        }

        public ObservableCollection<EventStack> FilteredCollection
        {
            get
            {
                return _providedEvtStackList;
            }
            set
            {
                _providedEvtStackList = value;
                //if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("FilteredCollection"));
            }
        }

        public ObservableCollection<Event> EventsResults
        {
            get
            {
                return _results;
            }
            set
            {
                _results = value;
                //if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("EventsResults"));
            }
        }

        public bool AreFiltersEmpty
        {
            get
            {
                return _areFiltersEmpty;
            }
            set
            {
                _areFiltersEmpty = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("AreFiltersEmpty"));
            }
        }

        public bool IsFilterActive
        {
            get
            {
                return _isFilterActive;
            }
            set
            {
                _isFilterActive = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsFilterActive"));
            }
        }

        public bool ContainsFilter(Predicate<Event> filter)
        {
            return GF.ContainsFilter(filter);
        }


        public void Clear()
        {
            EventsResults.Clear();
            GF.Clear();
        }

        public void AddFilter(Predicate<Event> filter)
        {
            if (!IsFilterActive)
            {
                IsFilterActive = true;
            }
            if (!GF.ContainsFilter(filter))
            {
                GF.AddFilter(filter);
            }
        }

        public void RemoveFilter(Predicate<Event> filter)
        {
            if (GF.ContainsFilter(filter))
            {
                GF.RemoveFilter(filter);
            }
        }

        public void RefreshFilter()
        {
            if (_stacksResult.Count > 0)
            {
                for (int i = 0; i < _stacksResult.Count; i++)
                {
                    _stacksResult[i].IsFilterResult = false;
                   _stacksResult[i].clearFilter();
                }
            }
            _stacksResult.Clear();
            EventsResults.Clear();
            if (!GF.IsEmpty())
            {
                _stacksResult = new ObservableCollection<EventStack>(_providedEvtStackList.Where(x => x.FilterEvents(GF.Filter)));
                foreach (EventStack evtStack in _stacksResult)
                {
                    evtStack.IsFilterResult = true;
                    //_stacksResult.Add(evtStack);
                    foreach(Event evt in evtStack.FilteredEvents)
                    {
                        EventsResults.Add(evt);
                    }
                }
            } else
            {
                IsFilterActive = false;
            }
        }

        public void UpdateListWithFilterResults(ObservableCollection<EventStack> events)
        {
            if (events.Count > 0)
            {
                for (int i = 0; i < events.Count; i++)
                {
                    events[i].IsFilterResult = false;
                    events[i].clearFilter();
                }
            }
            if (!GF.IsEmpty())
            {
                _stacksResult = new ObservableCollection<EventStack>(events.Where(x => x.FilterEvents(GF.Filter)));
                foreach (EventStack evtStack in _stacksResult)
                {
                    evtStack.IsFilterResult = true;  
                }
            }
            else
            {
                IsFilterActive = false;
            }

        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
