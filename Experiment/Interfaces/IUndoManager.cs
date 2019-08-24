using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Interfaces
{
    interface IUndoManager
    {
        void Execute(ICommand cmd);

        void Undo();

        void Redo();
    }
}
