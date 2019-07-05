using Experiment.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Utilities
{
    public class EventsUtilities
    {
        public static void UpdateCrossingEvts(ObservableCollection<EventStack> eventsCollection, EventStack item, Day currentDay)
        {
            foreach (EventStack element in eventsCollection)
            {
                if (element.Current.Date == currentDay.Date.AddDays(-1))
                {
                    foreach (Event evt in element.Events)
                    {
                        if (evt.Over2Days)
                        {
                            item.CrossingEvt = evt;
                        }
                    }
                }
            }
        }

        public static void UpdateCrossingEvts(ObservableCollection<EventStack> eventsCollection, EventStack currentEvtStack, Event crossingEvt)
        {
            foreach (EventStack element in eventsCollection)
            {
                if (element.Current.Date == currentEvtStack.Current.Date.AddDays(1))
                {
                    element.CrossingEvt = crossingEvt;
                }
            }
        } 
    }
}
