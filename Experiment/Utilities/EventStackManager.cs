using Experiment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Utilities
{
    public class EventStackManager : Interfaces.ICommand
    {
        private EventStack _Original;
        private EventStack _Modified;

        public EventStackManager(EventStack original, EventStack final, bool newStack = false, bool deleted = false)
        {
            _Original = original;
            _Modified = final;
            Deleted = deleted;
            IsNew = newStack;
        }

        public EventStackManager(EventStack final, bool newStack = false, bool deleted = false)
        {
            _Modified = final;
            Deleted = deleted;
            IsNew = newStack;
        }

        public void Execute()
        {
            if (Deleted)
            {
                DBHandler.DeleteEventStack(Modified);
            } else if (IsNew)
            {
                DBHandler.AddEventStack(Modified);
            } else 
            {
                DBHandler.InsertOrReplaceEventStack(Modified);
            } 
        }

        public void Undo()
        {
            if (Deleted)
            {
                DBHandler.AddEventStack(Modified);
            } else if (IsNew)
            {
                DBHandler.DeleteEventStack(Modified);
            } else 
            {
                DBHandler.UpdateEventStack(Original);
            }
        }

        public bool Deleted { get; set; }
        public bool IsNew { get; set; }

        public EventStack Original
        {
            get
            {
                return _Original;
            }
            set
            {
                _Original = value;
            }
        }

        public EventStack Modified
        {
            get
            {
                return _Modified;
            }
            set
            {
                _Modified = value;
            }
        }

    }
}
