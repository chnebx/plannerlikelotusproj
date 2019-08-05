using Experiment.CustomControls;
using Experiment.Models;
using Experiment.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Logique d'interaction pour DuplicateEventWindow.xaml
    /// </summary>
    public partial class DuplicateEventWindow : Window
    {
        private Event actualEvt = null;
        public ObservableCollection<DateSelectionModule> modules { get; set; }
        public DuplicateEventWindow(Event evt)
        {
            InitializeComponent();
            this.DataContext = this;
            actualEvt = evt;
            modules = new ObservableCollection<DateSelectionModule>();
            modules.Add(new DateSelectionModule(actualEvt));
        }

        private void BtnAddDuplicateDate_Click(object sender, RoutedEventArgs e)
        {
            modules.Add(new DateSelectionModule(actualEvt));
        }

        private void BtnRemoveDuplicateDate_Click(object sender, RoutedEventArgs e)
        {
            modules.RemoveAt(duplicateDateModulesList.SelectedIndex);
        }

        private List<EventStack> defineDuplicateEvents()
        {
            List<EventStack> evts = new List<EventStack>();
            int day;
            int month;
            int year;
            if (modules.Count > 0)
            {
                for (int i = 0; i < modules.Count; i++)
                {
                    int count = 1;
                    day = (int)modules[i].cmbDays.SelectedValue;
                    month = (int)modules[i].cmbMonths.SelectedIndex + 1;
                    year = (int)modules[i].cmbYear.SelectedValue;
                    if (modules[i].repeatCheckBox.IsChecked == true)
                    {
                        count = (int)modules[i].cmbRepeatCount.SelectedValue;
                    }
                    for (int j = 0; j < count; j++)
                    {
                        EventStack newEvtStack = new EventStack();
                        Event clonedEvt = actualEvt.Clone();
                        clonedEvt.Id = 0;
                        clonedEvt.Start = new DateTime(year, month, day, clonedEvt.Start.Hour, clonedEvt.Start.Minute, 0);
                        clonedEvt.End = new DateTime(year, month, day, clonedEvt.End.Hour, clonedEvt.End.Minute, 0);
                        newEvtStack.EventStackDay = new DateTime(year, month, day, 0, 0, 0);
                        if (actualEvt.Start.Day != actualEvt.End.Day)
                        {
                            clonedEvt.End = clonedEvt.End.AddDays(1);
                        }
                        newEvtStack.AddEvent(clonedEvt);
                        evts.Add(newEvtStack);
                        year++;
                    }
                }
            }
            return evts;
        }

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            List<EventStack> results = defineDuplicateEvents();
            if (results.Count > 0)
            {
                DBHandler.AddOrReplaceEventStacks(results);
            }
            this.Close();
        }
    }
}
