﻿using Experiment.Models;
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

        public static void DropHandler(object context, object data, EventStack FromStack, ObservableCollection<EventStack> eventsCollection)
        {
            EventStack previousStack = FromStack;
            if (context is Day)
            {
                Day droppedOnDay = (Day)context;
                if (data is Event)
                {
                    Event evt = (Event)data;
                    EventStack newEvtStack = new EventStack
                    {
                        Current = droppedOnDay
                    };

                    if (!Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        int indexOfPreviousEvt = previousStack.Events.IndexOf(evt);
                        previousStack.RemoveEvent(indexOfPreviousEvt);
                        if (previousStack.Events.Count < 1)
                        {
                            eventsCollection.Remove(previousStack);
                        }
                        newEvtStack.AddEvent(evt);
                        DBHandler.HandleDragEvent(previousStack, newEvtStack, evt);
                    }
                    else
                    {

                        Event copiedEvent = evt.Clone();
                        newEvtStack.AddEvent(copiedEvent);
                        DBHandler.HandleDragEvent(FromStack, newEvtStack, copiedEvent, copy: true);
                    }
                    eventsCollection.Add(newEvtStack);
                }
                else if (data is EventStack)
                {
                    EventStack evtStack = (EventStack)data;

                    if (!Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        evtStack.EventStackDay = droppedOnDay.Date;
                        for (int i = 0; i < evtStack.Events.Count; i++)
                        {
                            evtStack.Events[i].updateDates(droppedOnDay.Date.Year, droppedOnDay.Date.Month, droppedOnDay.Date.Day);
                        }
                        DBHandler.HandleDragEventStack(evtStack, null, copy: false);
                    }
                    else
                    {
                        EventStack newEvtStack = new EventStack
                        {
                            EventStackDay = droppedOnDay.Date
                        };
                        for (int i = 0; i < evtStack.Events.Count; i++)
                        {
                            newEvtStack.AddEvent(evtStack.Events[i].Clone());
                        }
                        eventsCollection.Add(newEvtStack);
                        DBHandler.AddEventStack(newEvtStack);
                    }
                }
            }
            else if (context is EventStack)
            {
                EventStack actualStack = (EventStack)context;
                if (data is Event)
                {
                    Event evt = (Event)data;
                    if (actualStack != previousStack && actualStack.Events.Count < 3)
                    {
                        if (!Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            int indexOfPreviousEvt = previousStack.Events.IndexOf(evt);
                            previousStack.RemoveEvent(indexOfPreviousEvt);
                            if (previousStack.Events.Count < 1)
                            {
                                eventsCollection.Remove(previousStack);
                            }
                            actualStack.AddEvent(evt);
                            DBHandler.HandleDragEvent(previousStack, actualStack, evt, copy: false);
                        }
                        else
                        {
                            Event copiedEvent = evt.Clone();
                            actualStack.AddEvent(copiedEvent);
                            DBHandler.HandleDragEvent(previousStack, actualStack, copiedEvent, copy: true);
                        }
                    }
                }
                else
                {
                    EventStack evtStack = (EventStack)data;
                    if (evtStack != actualStack && evtStack.Events.Count + actualStack.Events.Count <= 3)
                    {
                        if (!Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            for (int i = 0; i < evtStack.Events.Count; i++)
                            {
                                actualStack.AddEvent(evtStack.Events[i]);
                            }
                            eventsCollection.Remove(evtStack);
                            DBHandler.HandleDragEventStack(evtStack, actualStack, copy: false);
                        }
                        else
                        {
                            for (int i = 0; i < evtStack.Events.Count; i++)
                            {
                                actualStack.AddEvent(evtStack.Events[i].Clone());
                                DBHandler.HandleDragEventStack(evtStack, actualStack, copy: true);
                            }
                        }
                    }
                }
            }
        }


        public static void UpdateLimits(EventStack evtStack)
        {
            ObservableCollection<EventStack> events = DBHandler.getEvents(evtStack.EventStackDay.Date.Year);
            EventStack previousEvtStack = events.FirstOrDefault<EventStack>(x => x.EventStackDay.Date.AddDays(1) == evtStack.EventStackDay.Date);
            if (previousEvtStack != null && previousEvtStack.IsOver2Days())
            {
                evtStack.LowerLimitHour = previousEvtStack.Events.Last().End;
            }
            EventStack nextEvtStack = events.FirstOrDefault<EventStack>(x => x.EventStackDay.Date.AddDays(-1) == evtStack.EventStackDay.Date);
            if (nextEvtStack != null && nextEvtStack.Events.First().Start.Hour < 12)
            {
                evtStack.UpperLimitHour = nextEvtStack.Events.First().Start;
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
                        results.Add(item);
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
