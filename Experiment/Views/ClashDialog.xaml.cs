using Experiment.Models;
using Experiment.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Experiment.Views
{
    /// <summary>
    /// Logique d'interaction pour ClashDialog.xaml
    /// </summary>
    public partial class ClashDialog : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Event> _clashElements;
        private EventStack actualStack;
        private List<Event> _foundIndices;
        private Dictionary<string, Dictionary<Event, List<Event>>> _foundEvents;
        //public List<int> DeletedEventIds = new List<int>();
        public List<Event> DeletedEvents = new List<Event>();
        public List<Event> SolvedEvents { get; set; }
        public List<Event> ModifiedEvents { get; set; }
        public List<Event> DeletedExternalEvents = new List<Event>();
        public List<Event> ModifiedExternalEvents { get; set; }
        bool OutsideStacks = false;
        public ClashDialog(EventStack evtStack, List<Event> foundIndices, bool outsideStack)
        {
            InitializeComponent();
            DataContext = this;
            actualStack = evtStack;
            _foundIndices = foundIndices;
            ClashElements = new ObservableCollection<Event>();
            SolvedEvents = new List<Event>();
            HeaderTxt.Text = "Conflits présents :";
            OutsideStacks = outsideStack;
            buildClashElements();
        }

        public ClashDialog(List<Event> evts, bool outsideStack)
        {
            InitializeComponent();
            DataContext = this;
            ClashElements = evts != null ? new ObservableCollection<Event>(evts) : new ObservableCollection<Event>();
            SolvedEvents = new List<Event>();
            HeaderTxt.Text = "Conflits présents :";
            OutsideStacks = true;
        }

        public ClashDialog(Dictionary<string, Dictionary<Event, List<Event>>> evts, bool outsideStack)
        {
            InitializeComponent();
            DataContext = this;
            _foundEvents = evts;
            SolvedEvents = new List<Event>();
            buildClashElements();
            HeaderTxt.Text = "Conflits présents :";
            OutsideStacks = true;
        }

        public ObservableCollection<Event> ClashElements
        {
            get
            {
                return _clashElements;
            }
            set
            {
                _clashElements = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ClashElements"));
            }
        }

        private void buildClashElements()
        {
            if (ClashElements == null)
            {
                ClashElements = new ObservableCollection<Event>();
            }
            if (_foundIndices != null)
            {
                for (int i = 0; i < _foundIndices.Count; i++)
                {
                    //ClashElements.Add(actualStack.Events[_foundIndices[i]]);
                    ClashElements.Add(_foundIndices[i]);
                }
            } else if (_foundEvents != null)
            {
                foreach (Event evt in _foundEvents["Internal"].Keys)
                {
                    //SolvedEvents.Add(evt);
                    bool isAlreadySolved = true;
                    foreach (Event e in _foundEvents["Internal"][evt])
                    {
                        if (!ClashElements.Contains(e))
                        {
                            ClashElements.Add(e);
                            isAlreadySolved = false;
                        }
                    }
                    foreach (Event e in _foundEvents["External"][evt])
                    {
                        if (!ClashElements.Contains(e))
                        {
                            ClashElements.Add(e);
                            isAlreadySolved = false;
                        }
                    }
                    if (isAlreadySolved)
                    {
                        SolvedEvents.Add(evt);
                    }
                }
                SolvedEventsList.ItemsSource = SolvedEvents;
            }  
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DeleteEvtBtn_Click(object sender, RoutedEventArgs e)
        {
            Event clickedEvent = (Event)(((Button)sender).DataContext);
            //clickedEvent.parentStack.RemoveEvent(clickedEvent.parentStack.Events.IndexOf(clickedEvent));
            ClashElements.Remove(clickedEvent);
            if (_foundEvents != null)
            {
                foreach(string entries in _foundEvents.Keys)
                {
                    Dictionary<Event, List<Event>> result = _foundEvents[entries];
                    foreach (KeyValuePair<Event, List<Event>> entry in result)
                    {
                        if (entry.Value.Contains(clickedEvent))
                        {
                            entry.Value.Remove(clickedEvent);
                            if (entry.Value.Count == 0)
                            {
                                if (entries == "Internal")
                                {
                                    DeletedEvents.Add(clickedEvent);
                                } else
                                {
                                    DeletedExternalEvents.Add(clickedEvent);
                                }
                                SolvedEvents.Add(entry.Key);
                            }
                        }
                    }
                }
                SolvedEventsList.ItemsSource = SolvedEvents;
            }
            int validCount = 1;
            if (OutsideStacks)
            {
                validCount = 0;
            }
            if (ClashElements.Count == validCount)
            {
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
