using Experiment.Models;
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
        public ClashDialog(EventStack evtStack, List<int> foundIndices)
        {
            InitializeComponent();
            DataContext = this;
            actualStack = evtStack;
            _foundIndices = foundIndices;
            ClashElements = new ObservableCollection<Event>();
            HeaderTxt.Text = "Il existe des conflits :";
            buildClashElements();
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
            this.DialogResult = true;
            Close();
        }
    }
}
