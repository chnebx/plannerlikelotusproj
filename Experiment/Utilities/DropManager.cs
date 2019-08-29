using Experiment.Interfaces;
using Experiment.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Experiment.Utilities
{
    public class DropManager: Interfaces.ICommand
    {
        private ClashHandler _clashHandler;

        public DropManager(ClashHandler clashHandler)
        {
            _clashHandler = clashHandler;
        }

        public void Execute()
        { 
           FillEvents(ClashModule);
        }

        public void Undo()
        {
            UndoFillEvents(ClashModule);
        }

        public ClashHandler ClashModule
        {
            get
            {
                return _clashHandler;
            }
            set
            {
                _clashHandler = value;
            }
        }

        //public List<Event> duplicates;
        private void FillEvents(ClashHandler module)
        {
            //object source = module.OriginalSource;
            //object destination = module.OriginalDestination;
            EventStack dest;
            EventStack src;
            if (module.Destination is Day)
            {
                dest = new EventStack
                {
                    EventStackDay = ((Day)module.Destination).Date
                };
            }
            else
            {
                dest = (EventStack)module.Destination;
            }
            if (module.Source is EventStack)
            {
                src = (EventStack)module.Source;
            }
            else
            {
                DateTime parentId = ((Event)module.Source).EventStackId;
                src = DBHandler.getEventStackById(parentId);
            }
            List<Event> Solved = new List<Event>(module.SolvedEvents);
            List<Event> Deleted = new List<Event>(module.DeletedEvents.Concat(module.DeletedExternalEvents));
            if (module.copy)
            {
                //if (duplicates == null)
                if (module.CopiedEvents.Count == 0)
                {
                    //Making a list of events without ids to insert them with new ones
                    //duplicates = new List<Event>();
                    //foreach (Event e in Solved)
                    //{
                    //    Event evt = e.DeepCopy();
                    //    duplicates.Add(evt);
                    //}
                    //Solved = duplicates;
                    foreach(Event e in Solved)
                    {
                        Event evt = e.DeepCopy();
                        module.CopiedEvents.Add(evt);
                    }
                }
                Solved = module.CopiedEvents;
                //else
                //{
                //    // if a duplicates list is present, retrieve back the old events
                //    Solved = new List<Event>();
                //    foreach (Event e in duplicates)
                //    {
                //        Solved.Add(e);
                //    }
                //}
            }
            DBHandler.HandleDrag(Deleted, Solved, dest, src, module.copy);
            module.DestinationFinalId = dest.Id;
            if (module.CopiedEvents.Count == 0)
            {
                module.CopiedEvents = Solved.ToList<Event>();
            }
            //if (module.copy)
            //{
            //    Moved = Solved;
            //}
            //if (!copying)
            //{
            //    MoveEvents(Solved, dest, eventsList);
            //}
            //else
            //{
            //    CopyEvents(Solved, dest, eventsList);
            //}
            //DeleteEvents(module.DeletedEvents, src, eventsList);
            return;
        }


        private void UndoFillEvents(ClashHandler module)
        {
            object source = module.OriginalSource;
            object destination = module.OriginalDestination;
            
            EventStack dest;
            EventStack src;
            ObservableCollection<Event> evtsToDelete = new ObservableCollection<Event>();
            ObservableCollection<Event> evtsToMove = new ObservableCollection<Event>();
            ObservableCollection<Event> evtsToRestore;

            if (destination is Day)
            {
                dest = DBHandler.getEventStack(((Day)destination).Date);
            }
            else
            {
                dest = (EventStack)destination;
            }
            if (source is EventStack)
            {
                //src = (EventStack)source;
                //src = (EventStack)module.Source;
                //src = DBHandler.getEventStack(((EventStack)module.Source).Id);
                src = DBHandler.getEventStack(((EventStack)module.Source).EventStackDay);
                if (src == null)
                {
                    src = new EventStack
                    {
                        EventStackDay = ((EventStack)module.Source).EventStackDay
                    };
                }
            }
            else
            {
                Event srcEvent = (Event)source;
                src = DBHandler.getEventStack(srcEvent.Start.Date);
                if (src == null)
                {
                    src = new EventStack
                    {
                        EventStackDay = srcEvent.parentStack.EventStackDay
                    };
                }
            }
            //Console.WriteLine("----- Undo --- Source : " + src.EventStackDay + "--- destination : " + dest.EventStackDay);
            if (module.copy)
            {
                evtsToDelete = new ObservableCollection<Event>(module.CopiedEvents);
            }
            else
            {
                evtsToMove = new ObservableCollection<Event>(module.SolvedEvents);
            }

            evtsToRestore = new ObservableCollection<Event>(module.DeletedExternalEvents.Concat(module.DeletedEvents));
            DBHandler.HandleUndoDrag(evtsToDelete, evtsToMove, evtsToRestore, src, dest, module.copy);

        }

        
    }
}
