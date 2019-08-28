using Experiment.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Utilities
{
    public class StateManager: INotifyPropertyChanged
    {
        private Stack<ICommand> _Undo;
        private Stack<ICommand> _Redo;
        private bool _UndoIsEmpty = true;
        private bool _RedoIsEmpty = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public StateManager()
        {
            Reset();
        }

        public void Reset()
        {
            _Undo = new Stack<ICommand>();
            _Redo = new Stack<ICommand>();
            _UndoIsEmpty = true;
            _RedoIsEmpty = true;
        }

        public void Do(ICommand cmd)
        {
            if (!RedoIsEmpty)
            {
                Reset();
            }
            cmd.Execute();
            _Undo.Push(cmd);
            _Redo.Clear();
            UndoIsEmpty = false;
            RedoIsEmpty = true;
        }

        public bool UndoIsEmpty {
            get
            {
                return _UndoIsEmpty;
            }
            set
            {
                _UndoIsEmpty = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("UndoIsEmpty"));
            }
        }

        public bool RedoIsEmpty {
            get
            {
                return _RedoIsEmpty;
            }
            set
            {
                _RedoIsEmpty = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("RedoIsEmpty"));
            }
        }

        public void Undo()
        {
            if (_Undo.Count > 0)
            {
                ICommand cmd = _Undo.Pop();
                cmd.Undo();
                _Redo.Push(cmd);
                RedoIsEmpty = false;
                if (_Undo.Count == 0)
                {
                    UndoIsEmpty = true;
                }
                //Console.WriteLine("undone");
            }
        }

        public void Redo()
        {
            if (_Redo.Count > 0)
            {
                ICommand cmd = _Redo.Pop();
                cmd.Execute();
                _Undo.Push(cmd);
                UndoIsEmpty = false;
                if (_Redo.Count == 0)
                {
                    RedoIsEmpty = true;
                }
                //Console.WriteLine("Redone");
            }
        }
    }
}
