using Experiment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Utilities
{
    public class GroupFilter
    {
        private List<Predicate<Event>> _filters;

        public Predicate<Event> Filter { get; private set; }

        public GroupFilter()
        {
            _filters = new List<Predicate<Event>>();
            Filter = InternalFilter;
        }

        public bool IsEmpty()
        {
            return _filters.Count == 0;
        }

        private bool InternalFilter(Event o)
        {
            foreach (var filter in _filters)
            {
                if (!filter(o))
                {
                    return false;
                }
            }

            return true;
        }

        public void AddFilter(Predicate<Event> filter)
        {
            _filters.Add(filter);
        }

        public void Clear()
        {
            _filters.Clear();
        }

        public void RemoveFilter(Predicate<Event> filter)
        {
            if (_filters.Contains(filter))
            {
                _filters.Remove(filter);
            }
        }

        public bool ContainsFilter(Predicate<Event> filter)
        {
            return _filters.Contains(filter);
        }
    }
}
