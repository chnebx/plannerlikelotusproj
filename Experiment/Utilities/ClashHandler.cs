using Experiment.Models;
using Experiment.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Utilities
{
    public class ClashHandler
    {

        public List<Event> SolvedEvents { get; set; }
        public List<Event> DeletedEvents { get; set; }
        public object OriginalSource;
        public object OriginalDestination;
        public Dictionary<Event, List<Event>> Clashes { get; set; }
        public bool IsSolved { get; set; }

        public void StackToDayClashHandler(EventStack actualStack, Day destinationDay)
        {
            
            SolvedEvents = new List<Event>();
            DeletedEvents = new List<Event>();
            EventStack newStack = new EventStack
            {
                EventStackDay = destinationDay.Date
            };
            Clashes = newStack.FindClash(actualStack);
            if (Clashes.Count > 0)
            {
                IsSolved = false;
                ClashDialog clashPrompt = new ClashDialog(Clashes, true);
                if (clashPrompt.ShowDialog() == true)
                {
                    IsSolved = true;
                }
                SolvedEvents = clashPrompt.SolvedEvents;
                DeletedEvents = clashPrompt.DeletedEvents;
            } else
            {
                foreach(Event evt in actualStack.Events)
                {
                    SolvedEvents.Add(evt);
                }
                IsSolved = true;
            }
            InitBackupData(destinationDay, EventStack.Clone(actualStack));
        }

        public void EventToDayClashHandler(Event evt, Day destinationDay)
        {
            SolvedEvents = new List<Event>();
            DeletedEvents = new List<Event>();
            EventStack newStack = new EventStack
            {
                EventStackDay = destinationDay.Date
            };
            Clashes = newStack.FindClash(evt);
            if (Clashes.Count > 0)
            {
                IsSolved = false;
                ClashDialog clashPrompt = new ClashDialog(Clashes, true);
                if (clashPrompt.ShowDialog() == true)
                {
                    IsSolved = true;
                }
                SolvedEvents = clashPrompt.SolvedEvents;
                DeletedEvents = clashPrompt.DeletedEvents;
            } else
            {
                SolvedEvents.Add(evt);
            }
            IsSolved = true;
            InitBackupData(destinationDay, evt.DeepCopy());
        }

        public void StackToStackHandler(EventStack actualStack, EventStack destinationStack)
        {
            SolvedEvents = new List<Event>();
            DeletedEvents = new List<Event>();
            Clashes = destinationStack.FindClash(actualStack);
            if (Clashes.Count > 0)
            {
                IsSolved = false;
                ClashDialog clashPrompt = new ClashDialog(Clashes, true);
                if (clashPrompt.ShowDialog() == true)
                {
                    IsSolved = true;
                }
                SolvedEvents = clashPrompt.SolvedEvents;
                DeletedEvents = clashPrompt.DeletedEvents;
            } else
            {
                foreach(Event e in actualStack.Events)
                {
                    SolvedEvents.Add(e);
                }
                IsSolved = true;
            }
            InitBackupData(EventStack.Clone(destinationStack), EventStack.Clone(actualStack));
        }

        public void EventToStackHandler(Event evt, EventStack destinationStack)
        {
            SolvedEvents = new List<Event>();
            DeletedEvents = new List<Event>();
            Clashes = destinationStack.FindClash(evt);
            if (Clashes.Count > 0)
            {
                IsSolved = false;
                ClashDialog clashPrompt = new ClashDialog(Clashes, true);
                if (clashPrompt.ShowDialog() == true)
                {
                    IsSolved = true;
                }
                SolvedEvents = clashPrompt.SolvedEvents;
                DeletedEvents = clashPrompt.DeletedEvents;
            } else
            {
                SolvedEvents.Add(evt);
                IsSolved = true;
            }
            InitBackupData(EventStack.Clone(destinationStack), evt.DeepCopy());
        }

        public ClashHandler(object source, object destination)
        {
            if (source is Event)
            {
                Event src = (Event)source;
                if (destination is Day)
                {
                    EventToDayClashHandler(src, (Day)destination);
                } else if (destination is EventStack)
                {
                    EventToStackHandler(src, (EventStack)destination);
                }
            } else if (source is EventStack)
            {
                EventStack src = (EventStack)source;
                if (destination is Day)
                {
                    StackToDayClashHandler(src, (Day)destination);
                }
                else if (destination is EventStack)
                {
                    StackToStackHandler(src, (EventStack)destination);
                }
            }
        }

        private void InitBackupData(object destination, object source)
        {
            OriginalDestination = destination;
            OriginalSource = source;
        }
        
    }
}
