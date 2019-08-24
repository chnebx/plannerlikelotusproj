using Experiment.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Utilities
{
    public class StateManager
    {
        private Stack<ICommand> _Undo;
        private Stack<ICommand> _Redo;

        public StateManager()
        {
            Reset();
        }

        public void Reset()
        {
            _Undo = new Stack<ICommand>();
            _Redo = new Stack<ICommand>();
        }

        public void Do(ICommand cmd)
        {
            cmd.Execute();
            _Undo.Push(cmd);
            _Redo.Clear();
        }

        public void Undo()
        {
            if (_Undo.Count > 0)
            {
                ICommand cmd = _Undo.Pop();
                cmd.Undo();
                _Redo.Push(cmd);
            }
        }

        public void Redo()
        {
            if (_Redo.Count > 0)
            {
                ICommand cmd = _Redo.Pop();
                cmd.Execute();
                _Undo.Push(cmd);
            }
        }
    }
}
