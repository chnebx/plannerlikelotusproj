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
        public Dictionary<Event, List<Event>> Clashes { get; set; }
        public bool IsSolved { get; set; }

        public ClashHandler(Day destinationDay, EventStack actualStack)
        {
            IsSolved = false;
            EventStack newStack = new EventStack
            {
                EventStackDay = destinationDay.Date
            };
            Clashes = newStack.FindClash(actualStack);
            if (Clashes.Count > 0)
            {
                ClashDialog clashPrompt = new ClashDialog(Clashes, true);
                if (clashPrompt.ShowDialog() == true)
                {
                    SolvedEvents = clashPrompt.SolvedEvents;
                    DeletedEvents = clashPrompt.DeletedEvents;
                    IsSolved = true;
                }
            }
        }

        public ClashHandler(Day destinationDay, Event evt)
        {
            IsSolved = false;
            EventStack newStack = new EventStack
            {
                EventStackDay = destinationDay.Date
            };
            Clashes = newStack.FindClash(evt);
            if (Clashes.Count > 0)
            {
                ClashDialog clashPrompt = new ClashDialog(Clashes, true);
                if (clashPrompt.ShowDialog() == true)
                {
                    SolvedEvents = clashPrompt.SolvedEvents;
                    DeletedEvents = clashPrompt.DeletedEvents;
                    IsSolved = true;
                }
            }
        }

        public ClashHandler(EventStack destinationStack, EventStack actualStack)
        {
            IsSolved = false;
            Clashes = destinationStack.FindClash(actualStack);
            if (Clashes.Count > 0)
            {
                ClashDialog clashPrompt = new ClashDialog(Clashes, true);
                if (clashPrompt.ShowDialog() == true)
                {
                    SolvedEvents = clashPrompt.SolvedEvents;
                    DeletedEvents = clashPrompt.DeletedEvents;
                    IsSolved = true;
                }
            }
        }

        public ClashHandler(EventStack destinationStack, Event evt)
        {
            IsSolved = false;
            Clashes = destinationStack.FindClash(evt);
            if (Clashes.Count > 0)
            {
                ClashDialog clashPrompt = new ClashDialog(Clashes, true);
                if (clashPrompt.ShowDialog() == true)
                {
                    SolvedEvents = clashPrompt.SolvedEvents;
                    DeletedEvents = clashPrompt.DeletedEvents;
                    IsSolved = true;
                }
            }
        }

        
    }
}
