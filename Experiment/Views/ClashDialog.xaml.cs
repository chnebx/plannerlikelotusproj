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
        private List<int> _foundIndices;
        //public List<int> DeletedEventIds = new List<int>();
        public List<Event> DeletedEvents = new List<Event>();
        bool OutsideStacks = false;
        public ClashDialog(EventStack evtStack, List<int> foundIndices)
        {
            InitializeComponent();
            DataContext = this;
            actualStack = evtStack;
            _foundIndices = foundIndices;
            ClashElements = new ObservableCollection<Event>();
            HeaderTxt.Text = "Conflits présents :";
            buildClashElements();
        }

        public ClashDialog(List<Event> evts)
        {
            InitializeComponent();
            DataContext = this;
            ClashElements = evts != null ? new ObservableCollection<Event>(evts) : new ObservableCollection<Event>();
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
            for (int i = 0; i < _foundIndices.Count; i++)
            {
                ClashElements.Add(actualStack.Events[_foundIndices[i]]);
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
            clickedEvent.parentStack.RemoveEvent(clickedEvent.parentStack.Events.IndexOf(clickedEvent));
            ClashElements.Remove(clickedEvent);
            int validCount = 1;
            if (OutsideStacks)
            {
                validCount = 0;
                //DeletedEventIds.Add(clickedEvent.Id);
                DeletedEvents.Add(clickedEvent);
                DBHandler.DeleteEvent(clickedEvent); 
            }
            if (ClashElements.Count == validCount)
            {
                this.DialogResult = true;
                this.Close();
            }

        }
    }
}
