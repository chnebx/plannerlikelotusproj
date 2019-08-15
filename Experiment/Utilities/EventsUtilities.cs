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

        public static void DropHandler(object context, object data, EventStack FromStack, ObservableCollection<EventStack> eventsCollection)
        {
            EventStack previousStack = FromStack;
            bool isCopying = Keyboard.IsKeyDown(Key.RightCtrl);
            if (context is Day)
            {
                Day droppedOnDay = (Day)context;
                if (data is Event)
                {
                    Event evt = (Event)data;
                    EventStack newEvtStack = new EventStack
                    {
                        EventStackDay = droppedOnDay.Date
                    };
                    ClashHandler clashModule = new ClashHandler(droppedOnDay, evt);
                    FillEvents(clashModule, previousStack, droppedOnDay, eventsCollection, isCopying);

                    //if (!isCopying)
                    //{
                    //    int indexOfPreviousEvt = previousStack.Events.IndexOf(evt);
                    //    previousStack.RemoveEvent(indexOfPreviousEvt);
                    //    if (previousStack.Events.Count < 1)
                    //    {
                    //        eventsCollection.Remove(previousStack);
                    //    }
                    //    newEvtStack.AddEvent(evt);
                    //    DBHandler.HandleDragEvent(previousStack, newEvtStack, evt);
                    //}
                    //else
                    //{

                    //    Event copiedEvent = evt.Clone();
                    //    newEvtStack.AddEvent(copiedEvent);
                    //    DBHandler.HandleDragEvent(FromStack, newEvtStack, copiedEvent, copy: true);
                    //}
                    //eventsCollection.Add(newEvtStack);
                }
                else if (data is EventStack)
                {
                    EventStack evtStack = (EventStack)data;
                    EventStack conflictTest = new EventStack
                    {
                        EventStackDay = droppedOnDay.Date
                    };
                    ClashHandler clashModule = new ClashHandler(droppedOnDay, evtStack);
                    FillEvents(clashModule, evtStack, droppedOnDay, eventsCollection, isCopying);
                    
                    //List<Event> clashingEvents = conflictTest.CheckClash(evtStack);
                    //Dictionary<Event, List<Event>> clashingEvents = conflictTest.FindClash(evtStack);
                    //if (clashingEvents.Count > 0)
                    //{
                    //    bool solved;
                    //    List<Event> solvedEvents;
                    //    HandleClashes(null, clashingEvents, eventsCollection, out solved, out solvedEvents);
                    //    if (!solved)
                    //    {
                    //        if (solvedEvents != null)
                    //        {
                    //            if (!isCopying)
                    //            {
                    //                EventStack newEvtStack = new EventStack
                    //                {
                    //                    EventStackDay = droppedOnDay.Date
                    //                };
                    //                for (int i = 0; i < solvedEvents.Count; i++)
                    //                {
                    //                    newEvtStack.AddEvent(solvedEvents[i]);
                    //                    evtStack.RemoveEvent(evtStack.Events.IndexOf(solvedEvents[i]));
                    //                    DBHandler.DeleteEvent(solvedEvents[i]);
                    //                }
                    //                eventsCollection.Add(newEvtStack);
                    //                DBHandler.AddEventStack(newEvtStack);
                    //            }
                    //        }
                    //        return;
                    //    }
                    //}
                    if (!isCopying)
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
                        int indexOfPreviousEvt = previousStack.Events.IndexOf(evt);
                        ClashHandler clashModule = new ClashHandler(actualStack, evt);
                        FillEvents(clashModule, evt, actualStack, eventsCollection, isCopying);
                       
                        
                        
                        //List<Event> clashingIndices = actualStack.CheckClash(evt);

                        //if (clashingIndices.Count > 0)
                        //{
                        //    bool solved;
                        //    actualStack = HandleClashes(actualStack, clashingIndices, eventsCollection, out solved);
                        //    if (!solved)
                        //    {
                        //        return;
                        //    }
                        //}

                        //if (!isCopying)
                        //{
                        //    actualStack.AddEvent(evt);
                        //    previousStack.RemoveEvent(indexOfPreviousEvt);
                        //    if (previousStack.Events.Count < 1)
                        //    {
                        //        eventsCollection.Remove(previousStack);
                        //    }
                        //    DBHandler.HandleDragEvent(previousStack, actualStack, evt, copy: false);
                        //}
                        //else
                        //{
                        //    Event copiedEvent = evt.Clone();
                        //    actualStack.AddEvent(copiedEvent);
                        //    DBHandler.HandleDragEvent(previousStack, actualStack, copiedEvent, copy: true);
                        //}
                    }
                }
                else
                {
                    EventStack evtStack = (EventStack)data;
                    if (evtStack != actualStack && evtStack.Events.Count + actualStack.Events.Count <= 3)
                    {
                        ClashHandler clashModule = new ClashHandler(actualStack, evtStack);
                        FillEvents(clashModule, evtStack, actualStack, eventsCollection, isCopying);
                      

                        //List<Event> clashingIndices = actualStack.CheckClash(evtStack);
                        //if (clashingIndices.Count > 0)
                        //{
                        //    bool solved;
                        //    actualStack = HandleClashes(actualStack, clashingIndices, eventsCollection, out solved);
                        //    if (!solved)
                        //    {
                        //        return;
                        //    }
                        //}


                        if (!isCopying)
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
                            }
                            DBHandler.HandleDragEventStack(evtStack, actualStack, copy: true);
                        }
                    }
                }
            }
        }

        private static void DeleteEvents(List<Event> evtsToDelete, object fromStack, ObservableCollection<EventStack> eventsList)
        { 
            EventStack mainStack;
            if (fromStack is Event)
            {
                mainStack = (EventStack)eventsList.FirstOrDefault<EventStack>(x => x.Id == ((Event)fromStack).parentStack.Id);
            } else
            {
                mainStack = (EventStack)fromStack;
            }

            foreach (Event e in evtsToDelete)
            {
                EventStack parent;
                if (e.EventStackId != mainStack.Id)
                {
                    parent = eventsList.FirstOrDefault<EventStack>(x => x.Id == e.parentStack.Id);
                } else
                {
                    parent = mainStack;
                }
                //parent.RemoveEvent(parent.Events.IndexOf(parent.Events.Where(x => x.Id == e.Id).FirstOrDefault<Event>()));
                parent.RemoveEvent(parent.Events.IndexOf(e));
                if (parent.Events.Count == 0)
                {
                    eventsList.Remove(parent);
                    DBHandler.DeleteEventStack(parent);
                }
                DBHandler.DeleteEvent(e);
            }
        }

        private static void MoveEvents(List<Event> evtsToMove, object ToStack, ObservableCollection<EventStack> eventsList)
        {
            if (evtsToMove.Count == 0)
            {
                return;
            }
            EventStack destination;
            int parentId = evtsToMove.FirstOrDefault<Event>().parentStack.Id;
            EventStack source = eventsList.FirstOrDefault<EventStack>(x => x.Id == parentId);
            if (ToStack is EventStack)
            {
                destination = (EventStack)ToStack;
            } else
            {
                destination = new EventStack
                {
                    EventStackDay = ((Day)ToStack).Date
                };
                DBHandler.AddEventStack(destination);
                eventsList.Add(destination);
            }

            foreach ( Event e in evtsToMove )
            {
                destination.AddEvent(e);
                source.RemoveEvent(source.Events.IndexOf(e));
                DBHandler.MoveEvent(e, destination);
            }
            if (source.Events.Count == 0)
            {
                eventsList.Remove(source);
                DBHandler.DeleteEventStack(source);
            }
        }

        private static void CopyEvents(List<Event> evtsToMove, object ToStack, ObservableCollection<EventStack> eventsList)
        {
            EventStack destination;
            if (ToStack is EventStack)
            {
                destination = (EventStack)ToStack;
            }
            else
            {
                destination = new EventStack
                {
                    EventStackDay = ((Day)ToStack).Date
                };
                DBHandler.AddEventStack(destination);
                eventsList.Add(destination);
            }

            foreach ( Event e in evtsToMove )
            {
                destination.AddEvent(e.Clone());
                DBHandler.DuplicateEvent(e, destination);
            }
        }



        private static void FillEvents(ClashHandler module, object source, object destination, ObservableCollection<EventStack> eventsList, bool copying)
        {
            if (!copying)
            {
                MoveEvents(module.SolvedEvents, destination, eventsList);
            }
            else
            {
                CopyEvents(module.SolvedEvents, destination, eventsList);
            }
            
            DeleteEvents(module.DeletedEvents, source, eventsList);
            
            return;
        }

        //private static EventStack HandleClashes(object destination, Dictionary<Event, List<Event>> clashingIndices, ObservableCollection<EventStack> eventsList, out bool solved, out List<Event> solvedEvents)
        //{
        //    solvedEvents = null;
        //    if (destination is EventStack)
        //    {
        //        EventStack destinationStack = (EventStack)destination;
        //        ClashDialog clashPrompt = new ClashDialog(destinationStack, clashingIndices, true);
        //        if (clashPrompt.ShowDialog() == true)
        //        {
        //            DateTime currentActualStackDay = destinationStack.EventStackDay;
        //            foreach (Event e in clashPrompt.DeletedEvents)
        //            {
        //                if (e.parentStack.EventStackDay != destinationStack.EventStackDay)
        //                {
        //                    // if conflicting stack is not the destination one
        //                    EventStack found = eventsList.FirstOrDefault<EventStack>(x => x.Id == e.parentStack.Id);
        //                    if (found != null)
        //                    {
        //                        found.RemoveEvent(found.Events.IndexOf(found.Events.Where(x => x.Id == e.Id).FirstOrDefault<Event>()));
        //                        if (found.Events.Count == 0)
        //                        {
        //                            eventsList.Remove(found);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    e.parentStack.RemoveEvent(e.parentStack.Events.IndexOf(e));
        //                }
        //                DBHandler.DeleteEvent(e);
        //            }
        //            if (destinationStack.Events.Count == 0)
        //            {
        //                eventsList.Remove(destinationStack);
        //                destination = new EventStack
        //                {
        //                    EventStackDay = currentActualStackDay
        //                };
        //                eventsList.Add(destinationStack);
        //            }
        //            solved = true;
        //        }
        //        else
        //        {
        //            List<Event> results = clashPrompt.SolvedEvents;
        //            if (results != null)
        //            {
        //                if (results.Count > 0)
        //                {
        //                    solvedEvents = results;
        //                }
        //            }
        //            solved = false;
        //        }
        //        return destinationStack;
        //    }
        //    else
        //    {
        //        ClashDialog clashPrompt = new ClashDialog(clashingIndices, true);
        //        if (clashPrompt.ShowDialog() == true)
        //        {
        //            solved = true;
        //        }
        //        else
        //        {
        //            List<Event> results = clashPrompt.SolvedEvents;
        //            if (results != null)
        //            {
        //                if (results.Count > 0)
        //                {
        //                    solvedEvents = results;
        //                }
        //            }
        //            solved = false;
        //        }
        //        foreach (Event e in clashPrompt.DeletedEvents)
        //        {
        //            EventStack found = eventsList.FirstOrDefault<EventStack>(x => x.Id == e.parentStack.Id);
        //            if (found != null)
        //            {

        //                found.RemoveEvent(found.Events.IndexOf(found.Events.Where(x => x.Id == e.Id).FirstOrDefault<Event>()));
        //                if (found.Events.Count == 0)
        //                {
        //                    eventsList.Remove(found);
        //                }
        //            }
        //        }
        //        return null;
        //    }
        //}

        //private static EventStack HandleClashes(object destination, List<Event> clashingIndices, ObservableCollection<EventStack> eventsList, out bool solved)
        //{
        //    if (destination is EventStack)
        //    {
        //        EventStack destinationStack = (EventStack)destination;
        //        ClashDialog clashPrompt = new ClashDialog(destinationStack, clashingIndices, true);
        //        if (clashPrompt.ShowDialog() == true)
        //        {
        //            DateTime currentActualStackDay = destinationStack.EventStackDay;
        //            foreach (Event e in clashPrompt.DeletedEvents)
        //            {
        //                if (e.parentStack.EventStackDay != destinationStack.EventStackDay)
        //                {
        //                    // if conflicting stack is not the destination one
        //                    EventStack found = eventsList.FirstOrDefault<EventStack>(x => x.Id == e.parentStack.Id);
        //                    if (found != null)
        //                    {
        //                        found.RemoveEvent(found.Events.IndexOf(found.Events.Where(x => x.Id == e.Id).FirstOrDefault<Event>()));
        //                        if (found.Events.Count == 0)
        //                        {
        //                            eventsList.Remove(found);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    e.parentStack.RemoveEvent(e.parentStack.Events.IndexOf(e));
        //                }
        //            }
        //            if (destinationStack.Events.Count == 0)
        //            {
        //                eventsList.Remove(destinationStack);
        //                destination = new EventStack
        //                {
        //                    EventStackDay = currentActualStackDay
        //                };
        //                eventsList.Add(destinationStack);
        //            }
        //            solved = true;
        //        }
        //        else
        //        {
        //            solved = false;
        //        }
        //        return destinationStack;
        //    }
        //    else
        //    {
        //        ClashDialog clashPrompt = new ClashDialog(clashingIndices, true);
        //        if (clashPrompt.ShowDialog() == true)
        //        {
        //            foreach (Event e in clashPrompt.DeletedEvents)
        //            {
        //                EventStack found = eventsList.FirstOrDefault<EventStack>(x => x.Id == e.parentStack.Id);
        //                if (found != null)
        //                {

        //                    found.RemoveEvent(found.Events.IndexOf(found.Events.Where(x => x.Id == e.Id).FirstOrDefault<Event>()));
        //                    if (found.Events.Count == 0)
        //                    {
        //                        eventsList.Remove(found);
        //                    }
        //                }
        //            }
        //            solved = true;
        //        } else
        //        {
        //            solved = false;
        //        }
        //        return null;
        //    }
        //}

        public static void UpdateLimits(EventStack evtStack)
        {
            ObservableCollection<EventStack> events = DBHandler.getEventsFrom(evtStack.EventStackDay.Date.AddYears(-1).Year);
            EventStack previousEvtStack = events.FirstOrDefault<EventStack>(x => x.EventStackDay.Date.AddDays(1) == evtStack.EventStackDay.Date);
            if (previousEvtStack != null && previousEvtStack.IsOverlapping)
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
