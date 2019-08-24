using Experiment.Models;
using Experiment.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public static DropManager DropHandler(object context, object data, EventStack FromStack, ObservableCollection<EventStack> eventsCollection)
        {
            EventStack previousStack = FromStack;
            object source = data;
            object destination = context; 
            bool isCopying = Keyboard.IsKeyDown(Key.RightCtrl);

            if (context is EventStack)
            {
                EventStack actualStack = (EventStack)context;
                destination = actualStack;
                if (data is Event && (actualStack == previousStack || actualStack.Events.Count >= 3))
                {
                    return null;  
                }
                if (data is EventStack && ((EventStack)data == actualStack || ((EventStack)data).Events.Count + actualStack.Events.Count > 3))
                {
                    return null;
                }
            }
            ClashHandler clashModule = new ClashHandler(source, destination, isCopying);
            DropManager Drop = new DropManager(clashModule);
            return Drop;
            //FillEvents(clashModule, isCopying);

            //StateManager<ClashHandler>.Undo.Push(act);
        }


        private static void DeleteEvents(List<Event> evtsToDelete, EventStack fromStack, ObservableCollection<EventStack> eventsList)
        { 
            EventStack mainStack;
            if (evtsToDelete.Count == 0)
            {
                return;
            }
            mainStack = (EventStack)eventsList.FirstOrDefault<EventStack>(x => x.Id == fromStack.Id);

            foreach (Event e in evtsToDelete)
            {
                EventStack parent;
                if (mainStack != null && e.EventStackId == mainStack.Id)
                {
                    parent = mainStack;
                } else
                {
                    parent = eventsList.FirstOrDefault<EventStack>(x => x.Id == e.EventStackId);
                }
                parent.RemoveEvent(parent.GetIndex(e));
                if (parent.Events.Count == 0)
                {
                    eventsList.Remove(parent);
                }
            }
        }

        private static void MoveEvents(ObservableCollection<Event> evtsToMove, EventStack ToStack, ObservableCollection<EventStack> eventsList)
        {
            if (evtsToMove.Count == 0)
            {
                return;
            }
            DateTime parent = evtsToMove.FirstOrDefault<Event>().parentStack.EventStackDay;
            EventStack source = eventsList.FirstOrDefault<EventStack>(x => x.EventStackDay == parent);
            if (ToStack.Events.Count == 0)
            {
                eventsList.Add(ToStack);
            }
            foreach ( Event e in evtsToMove )
            {
                source.RemoveEvent(source.Events.IndexOf(e));
                ToStack.AddEvent(e);
            }
            if (source.Events.Count == 0)
            {
                eventsList.Remove(source);
            }
        }

        private static void CopyEvents(ObservableCollection<Event> evtsToMove, EventStack ToStack, ObservableCollection<EventStack> eventsList)
        {
            if (evtsToMove.Count == 0)
            {
                return;
            }
            if (ToStack.Events.Count == 0)
            {
                eventsList.Add(ToStack);
            }
            foreach ( Event e in evtsToMove )
            {
                ToStack.AddEvent(e);
            }
        }

        

        public static void UpdateLimits(EventStack evtStack)
        {
            ObservableCollection<EventStack> events = DBHandler.getEventsFrom(evtStack.EventStackDay.Date.AddYears(-1).Year);
            EventStack previousEvtStack = events.FirstOrDefault<EventStack>(x => x.EventStackDay.Date.AddDays(1) == evtStack.EventStackDay.Date);
            if (previousEvtStack != null && previousEvtStack.IsOver2Days())
            {
                evtStack.LowerLimitHour = previousEvtStack.Events.Last().End;
                evtStack.LowerLimitEvent = previousEvtStack.Events.Last();
            } else
            {
                evtStack.LowerLimitHour = new DateTime(evtStack.EventStackDay.Year, evtStack.EventStackDay.Month, evtStack.EventStackDay.Day, 0, 0, 0);
            }
            EventStack nextEvtStack = events.FirstOrDefault<EventStack>(x => x.EventStackDay.Date.AddDays(-1) == evtStack.EventStackDay.Date);
            if (nextEvtStack != null && nextEvtStack.Events.First().Start.Hour < 12)
            {
                evtStack.UpperLimitHour = nextEvtStack.Events.First().Start;
                evtStack.UpperLimitEvent = nextEvtStack.Events.First();
            } else
            {
                evtStack.UpperLimitHour = new DateTime(evtStack.EventStackDay.Year, evtStack.EventStackDay.Month, evtStack.EventStackDay.Day, 12, 0, 0).AddDays(1);
            }

        }

        public static List<EventStack> FindClashingEvtStacks(EventStack item, List<EventStack> orderedList)
        {
            int min = 0;
            int max = orderedList.Count - 1;
            int previousGuess = 0;
            if (item.EventStackDay < orderedList[min].EventStackDay 
                || item.EventStackDay > orderedList[max].EventStackDay
                || item.Events.Count == 0)
            {
                return null;
            }
            int guess = (min + max) / 2;
            int count = 1;
            while (min <= max)
            {
                if (item.EventStackDay < orderedList[guess].EventStackDay)
                {
                    max = guess - 1;
                }
                if (item.EventStackDay > orderedList[guess].EventStackDay)
                {
                    min = guess + 1;
                }
                if (item.EventStackDay == orderedList[guess].EventStackDay)
                {
                    List<EventStack> results = FindNeighbours(guess, item, orderedList);

                    if (orderedList[guess].IsClashingEvent(item.Events.First())) {
                        results.Add(orderedList[guess]);
                    }
                    //Console.WriteLine("found after " + count + " attempts");
                    if (results.Count > 0)
                    {
                        return results;
                    }
                    return null;
                }
                previousGuess = guess;
                guess = (min + max) / 2;
                count++;
            }
            List<EventStack> lastResults = FindNeighbours(previousGuess, item, orderedList);
            if (lastResults.Count > 0)
            {
                return lastResults;
            }
            //Console.WriteLine("not found after " + count + " attempts");
            return null;
        } 

        private static List<EventStack> FindNeighbours(int lastGuess, EventStack item, List<EventStack> orderedList)
        {
            List<EventStack> result = new List<EventStack>();
            int evtStackBeforeIndex = lastGuess - 1;
            int evtStackAfterIndex = lastGuess + 1;

            if (orderedList[lastGuess].EventStackDay.AddDays(-1) == item.EventStackDay)
            {
                evtStackAfterIndex = lastGuess;
                if (orderedList[evtStackAfterIndex - 1].EventStackDay.AddDays(1) == item.EventStackDay)
                {
                    evtStackBeforeIndex = evtStackAfterIndex - 1;
                }
            } else if (orderedList[lastGuess].EventStackDay.AddDays(1) == item.EventStackDay)
            {
                evtStackBeforeIndex = lastGuess;
                if (orderedList[evtStackBeforeIndex + 1].EventStackDay.AddDays(-1) == item.EventStackDay)
                {
                    evtStackAfterIndex = evtStackBeforeIndex + 1;
                }
            }
            
            if (evtStackBeforeIndex >= 0)
            {
                if (orderedList[evtStackBeforeIndex].IsOverlapping
                    && orderedList[evtStackBeforeIndex].Events.Last().End > item.Events.First().Start)
                {
 
                    result.Add(orderedList[evtStackBeforeIndex]);
                }
            }
            if (evtStackAfterIndex <= orderedList.Count - 1)
            {
                if (orderedList[evtStackAfterIndex].Events.First().Start < item.Events.Last().End)
                {
                    result.Add(orderedList[evtStackAfterIndex]);
                }
            }
            return result;
        }
        //public static void UpdateNeighborsLimits(EventStack evtStack)
        //{
        //    ObservableCollection<EventStack> events = DBHandler.getEvents(evtStack.EventStackDay.Date.Year);
        //    EventStack previousEvtStack = events.FirstOrDefault<EventStack>(x => x.EventStackDay.Date.AddDays(1) == evtStack.EventStackDay.Date);
        //    DateTime start;
        //    DateTime end;
        //    List<EventStack> results = new List<EventStack>();
        //    if (previousEvtStack != null && evtStack.Events.First().Start.Hour < 12)
        //    {
        //        if (evtStack.Events.Count != 0)
        //        {
        //            start = evtStack.Events.FirstOrDefault<Event>().Start;
        //            previousEvtStack.UpperLimitHour = start;
        //            results.Add(previousEvtStack);
        //        }
        //    }
        //    EventStack nextEvtStack = events.FirstOrDefault<EventStack>(x => x.EventStackDay.Date.AddDays(-1) == evtStack.EventStackDay.Date);
        //    if (nextEvtStack != null && evtStack.IsOver2Days())
        //    {
        //        if (evtStack.Events.Count != 0)
        //        {
        //            end = evtStack.Events.Last<Event>().End;
        //            nextEvtStack.LowerLimitHour = end;
        //            results.Add(nextEvtStack);
        //        }  
        //    }
        //    if (results.Count > 0)
        //    {
        //        DBHandler.UpdateList(results);
        //    }
        //}
    }
}
