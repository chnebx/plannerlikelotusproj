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

        public ClashHandler(Day destinationDay, EventStack actualStack)
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
                    SolvedEvents = clashPrompt.SolvedEvents;
                    DeletedEvents = clashPrompt.DeletedEvents;
                    IsSolved = true;
                }
            }
            InitBackupData(destinationDay, EventStack.Clone(actualStack));
        }

        public ClashHandler(Day destinationDay, Event evt)
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
                    SolvedEvents = clashPrompt.SolvedEvents;
                    DeletedEvents = clashPrompt.DeletedEvents;
                    IsSolved = true;
                }
            }
            InitBackupData(destinationDay, evt.Clone());
        }

        public ClashHandler(EventStack destinationStack, EventStack actualStack)
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
                    SolvedEvents = clashPrompt.SolvedEvents;
                    DeletedEvents = clashPrompt.DeletedEvents;
                    IsSolved = true;
                }
            }
            InitBackupData(EventStack.Clone(destinationStack), EventStack.Clone(actualStack));
        }

        public ClashHandler(EventStack destinationStack, Event evt)
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
                    SolvedEvents = clashPrompt.SolvedEvents;
                    DeletedEvents = clashPrompt.DeletedEvents;
                    IsSolved = true;
                }
            }
            InitBackupData(EventStack.Clone(destinationStack), evt.Clone());
        }

        private void InitBackupData(object destination, object source)
        {
            OriginalDestination = destination;
            OriginalSource = source;
        }
        
    }
}
