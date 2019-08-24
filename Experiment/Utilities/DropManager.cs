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

        private static void FillEvents(ClashHandler module)
        {
            object source = module.OriginalSource;
            object destination = module.OriginalDestination;
            EventStack dest;
            EventStack src;
            ObservableCollection<Event> duplicates = new ObservableCollection<Event>();
            if (destination is Day)
            {
                dest = new EventStack
                {
                    EventStackDay = ((Day)destination).Date
                };
            }
            else
            {
                dest = (EventStack)destination;
            }
            if (source is EventStack)
            {
                src = (EventStack)source;
            }
            else
            {
                int parentId = ((Event)source).EventStackId;
                src = DBHandler.getEventStack(parentId);
            }
            ObservableCollection<Event> Solved = new ObservableCollection<Event>(module.SolvedEvents);
            ObservableCollection<Event> Deleted = new ObservableCollection<Event>(module.DeletedEvents.Concat(module.DeletedExternalEvents));
            if (module.copy)
            {
                foreach (Event e in Solved)
                {
                    Event evt = e.DeepCopy();
                    duplicates.Add(evt);
                }
                Solved = duplicates;
            }
            int lastCreatedID;
            DBHandler.HandleDrag(Deleted, Solved, null, dest, src, out lastCreatedID, module.copy);
            if (lastCreatedID > 0)
            {
                module.DestinationFinalId = dest.Id;
            }

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


        public void UndoFillEvents(ClashHandler module)
        {
            object source = module.OriginalSource;
            object destination = module.OriginalDestination;
            EventStack dest;
            EventStack src;
            ObservableCollection<Event> evtsToDelete = new ObservableCollection<Event>();
            ObservableCollection<Event> evtsToMove = new ObservableCollection<Event>();
            ObservableCollection<Event> evtsToRestore = new ObservableCollection<Event>();

            if (destination is Day)
            {
                dest = new EventStack
                {
                    EventStackDay = ((Day)destination).Date
                };
            }
            else
            {
                dest = (EventStack)destination;
            }
            if (source is EventStack)
            {
                src = (EventStack)source;
            }
            else
            {
                int parentId = ((Event)source).EventStackId;
                src = DBHandler.getEventStack(parentId);
            }
            foreach (Event e in module.SolvedEvents)
            {
                e.EventStackId = module.DestinationFinalId;
            }
            if (module.copy)
            {
                evtsToDelete = new ObservableCollection<Event>(module.SolvedEvents);
            }
            else
            {
                evtsToMove = new ObservableCollection<Event>(module.SolvedEvents);
            }

            evtsToRestore = new ObservableCollection<Event>(module.DeletedExternalEvents.Concat(module.DeletedEvents));
            int lastCreatedID;
            DBHandler.HandleDrag(evtsToDelete, evtsToMove, evtsToRestore, src, dest, out lastCreatedID, false);
        }

        
    }
}
