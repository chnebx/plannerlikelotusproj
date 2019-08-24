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
        public List<Event> ModifiedEvents { get; set; }
        public List<Event> DeletedExternalEvents { get; set; }
        public List<Event> ModifiedExternalEvents { get; set; }
        public ClashDialog clashPrompt;
        public bool copy;
        public object OriginalSource;
        public object OriginalDestination;
        public int DestinationFinalId;
        //public Dictionary<Event, List<Event>> Clashes { get; set; }
        public Dictionary<string, Dictionary<Event, List<Event>>> Clashes { get; set; }
        public bool IsSolved { get; set; }

        public void StackToDayClashHandler(EventStack actualStack, Day destinationDay)
        {
            EventStack newStack = new EventStack
            {
                EventStackDay = destinationDay.Date
            };
            Clashes = newStack.FindClash(actualStack);
            if (Clashes.Count > 0)
            {
                IsSolved = false;
                clashPrompt = new ClashDialog(Clashes, true);
                if (clashPrompt.ShowDialog() == true)
                {
                    IsSolved = true;
                }
            } else
            {
                foreach(Event evt in actualStack.Events)
                {
                    SolvedEvents.Add(evt);
                }
                IsSolved = true;
            }
            //InitBackupData(destinationDay, EventStack.Clone(actualStack));
        }

        public void EventToDayClashHandler(Event evt, Day destinationDay)
        {
            EventStack newStack = new EventStack
            {
                EventStackDay = destinationDay.Date
            };
            Clashes = newStack.FindClash(evt);
            if (Clashes.Count > 0)
            {
                IsSolved = false;
                clashPrompt = new ClashDialog(Clashes, true);
                if (clashPrompt.ShowDialog() == true)
                {
                    IsSolved = true;
                }
            } else
            {
                SolvedEvents.Add(evt);
            }
            IsSolved = true;
            //InitBackupData(destinationDay, evt.DeepCopy());
        }

        public void StackToStackHandler(EventStack actualStack, EventStack destinationStack)
        {
            Clashes = destinationStack.FindClash(actualStack);
            if (Clashes.Count > 0)
            {
                IsSolved = false;
                clashPrompt = new ClashDialog(Clashes, true);
                if (clashPrompt.ShowDialog() == true)
                {
                    IsSolved = true;
                }
            } else
            {
                foreach(Event e in actualStack.Events)
                {
                    SolvedEvents.Add(e);
                }
                IsSolved = true;
            }
            //InitBackupData(EventStack.Clone(destinationStack), EventStack.Clone(actualStack));
        }

        public void EventToStackHandler(Event evt, EventStack destinationStack)
        {
            Clashes = destinationStack.FindClash(evt);
            if (Clashes.Count > 0)
            {
                IsSolved = false;
                clashPrompt = new ClashDialog(Clashes, true);
                if (clashPrompt.ShowDialog() == true)
                {
                    IsSolved = true;
                }
            } else
            {
                SolvedEvents.Add(evt);
                IsSolved = true;
            }
            //InitBackupData(EventStack.Clone(destinationStack), evt.DeepCopy());
        }

        public ClashHandler(object source, object destination, bool isCopying)
        {
            SolvedEvents = new List<Event>();
            DeletedEvents = new List<Event>();
            DeletedExternalEvents = new List<Event>();
            copy = isCopying;
            InitBackupData(destination, source);
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
            if (clashPrompt != null)
            {
                SolvedEvents = clashPrompt.SolvedEvents;
                DeletedEvents = clashPrompt.DeletedEvents;
                DeletedExternalEvents = clashPrompt.DeletedExternalEvents;
            }
            
        }

        private void InitBackupData(object destination, object source)
        {
            OriginalDestination = destination;
            OriginalSource = source;
        }
        
    }
}
