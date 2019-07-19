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
        //public static void UpdateCrossingEvts(ObservableCollection<EventStack> eventsCollection, EventStack item, Day currentDay)
        //{
        //    foreach (EventStack element in eventsCollection)
        //    {
        //        if (element.Current.Date == currentDay.Date.AddDays(-1))
        //        {
        //            foreach (Event evt in element.Events)
        //            {
        //                if (evt.Over2Days)
        //                {
        //                    item.CrossingEvt = evt;
        //                }
        //            }
        //        }
        //    }
        //}

        //public static void UpdateCrossingEvts(ObservableCollection<EventStack> eventsCollection, EventStack currentEvtStack, Event crossingEvt)
        //{
        //    foreach (EventStack element in eventsCollection)
        //    {
        //        if (element.Current.Date == currentEvtStack.Current.Date.AddDays(1))
        //        {
        //            element.CrossingEvt = crossingEvt;
        //        }
        //    }
        //} 
        public static void UpdateLimits(EventStack evtStack)
        {
            //ObservableCollection<EventStack> events = DBHandler.getEvents(evtStack.Current.Date.Year);
            //EventStack previousEvtStack = events.FirstOrDefault<EventStack>(x => x.Current.Date.AddDays(1) == evtStack.Current.Date);
            //if (previousEvtStack != null && previousEvtStack.IsOver2Days())
            //{
            //    evtStack.LowerLimitHour = previousEvtStack.Events.Last().End;
            //}
            //EventStack nextEvtStack = events.FirstOrDefault<EventStack>(x => x.Current.Date.AddDays(-1) == evtStack.Current.Date);
            //if (nextEvtStack != null && nextEvtStack.Events.First().Start.Hour < 12)
            //{
            //    evtStack.UpperLimitHour = nextEvtStack.Events.First().Start;
            //}
        }

        public static void UpdateNeighborsLimits(EventStack evtStack)
        {
            //ObservableCollection<EventStack> events = DBHandler.getEvents(evtStack.Current.Date.Year);
            //EventStack previousEvtStack = events.FirstOrDefault<EventStack>(x => x.Current.Date.AddDays(1) == evtStack.Current.Date);
            //DateTime start;
            //DateTime end;
            //if (previousEvtStack != null && evtStack.Events.First().Start.Hour < 12)
            //{
            //    if (evtStack.Events.Count != 0)
            //    {
            //        start = evtStack.Events.FirstOrDefault<Event>().Start;
            //    } else
            //    {
            //        //start = new DateTime();
            //        return;
            //    }
            //    previousEvtStack.UpperLimitHour = start;
            //}
            //EventStack nextEvtStack = events.FirstOrDefault<EventStack>(x => x.Current.Date.AddDays(-1) == evtStack.Current.Date);
            //if (nextEvtStack != null && evtStack.IsOver2Days())
            //{
            //    if (evtStack.Events.Count != 0)
            //    {
            //        end = evtStack.Events.Last<Event>().End;
            //    } else
            //    {
            //        return;
            //    }
                
            //    nextEvtStack.LowerLimitHour = end;
            //}
        }
    }
}
