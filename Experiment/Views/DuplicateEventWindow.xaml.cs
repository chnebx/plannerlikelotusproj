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
            modules.Add(new DateSelectionModule(actualEvt, true));
        }

        private void BtnAddDuplicateDate_Click(object sender, RoutedEventArgs e)
        {
            modules.Add(new DateSelectionModule(actualEvt, true));
        }

        private void BtnRemoveDuplicateDate_Click(object sender, RoutedEventArgs e)
        {
            modules.RemoveAt(duplicateDateModulesList.SelectedIndex);
        }

        private List<EventStack> defineDuplicateEvents()
        {
            List<EventStack> evts = new List<EventStack>();
            List<Event> conflictingEvents = new List<Event>();
            Dictionary<Event, EventStack> ClashingElements = new Dictionary<Event, EventStack>();
            Dictionary<EventStack, DateSelectionModule> ClashingModules = new Dictionary<EventStack, DateSelectionModule>();
            List<EventStack> evtsCollection = DBHandler.getEventsFrom(DateTime.Now.Year).OrderBy((x) => x.EventStackDay).ToList<EventStack>();
            int year;
            
            if (modules.Count > 0)
            {
                for (int i = 0; i < modules.Count; i++)
                {
                    int count = 0;
                    DateTime timeCheck = modules[i].GetDate();
                    year = timeCheck.Year;
                    if (timeCheck >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0))
                    {
                        
                        if (modules[i].repeatCheckBox.IsChecked == true)
                        {
                            count = (int)modules[i].cmbRepeatCount.SelectedValue;
                        }
                        for (int j = 0; j <= count; j++)
                        {
                            
                            EventStack newEvtStack = new EventStack();
                            Event clonedEvt = actualEvt.Clone();
                            clonedEvt.Id = 0;
                            clonedEvt.Start = new DateTime(timeCheck.Year, timeCheck.Month, timeCheck.Day, clonedEvt.Start.Hour, clonedEvt.Start.Minute, 0);
                            clonedEvt.End = new DateTime(timeCheck.Year, timeCheck.Month, timeCheck.Day, clonedEvt.End.Hour, clonedEvt.End.Minute, 0);
                            newEvtStack.EventStackDay = timeCheck;
                            if (actualEvt.Start.Day != actualEvt.End.Day)
                            {
                                clonedEvt.End = clonedEvt.End.AddDays(1);
                            }
                            newEvtStack.AddEvent(clonedEvt);
                            timeCheck = timeCheck.AddYears(1);
                            List<EventStack> conflictingStacks = CheckIfPossibleClashes(newEvtStack, evtsCollection);
                            if (conflictingStacks is null)
                            {
                                // No conflicting EventStacks
                                
                                evts.Add(newEvtStack);
                                if (count >= 1 && modules[i].repeatCheckBox.IsChecked == true)
                                {
                                    modules[i].cmbRepeatCount.SelectedValue = count - 1;
                                }

                            } else
                            {
                                for (int k = 0; k < conflictingStacks.Count; k++)
                                {
                                    for (int l = 0; l < conflictingStacks[k].Events.Count; l++)
                                    {
                                        if (conflictingStacks[k].Events[l].Clashes(newEvtStack.Events.First()))
                                        {
                                            conflictingEvents.Add(conflictingStacks[k].Events[l]);
                                            ClashingElements.Add(conflictingStacks[k].Events[l], newEvtStack);
                                            ClashingModules.Add(newEvtStack, modules[i]);
                                        }
                                    }
                                }
                            }
                        }
                    }  
                }
                modules.Clear();
                if (conflictingEvents.Count > 0)
                {
                    
                    List<DateTime> DatesToRedistribute = new List<DateTime>();
                    ClashDialog clashPrompt = new ClashDialog(conflictingEvents);
                    if (clashPrompt.ShowDialog() == false)
                    {
                        for (int i = 0; i < clashPrompt.DeletedEvents.Count; i++)
                        {
                            if (ClashingElements.ContainsKey(clashPrompt.DeletedEvents[i]))
                            {
                                evts.Add(ClashingElements[clashPrompt.DeletedEvents[i]]);
                                ClashingElements.Remove(clashPrompt.DeletedEvents[i]);
                            }
                        }
                        foreach (KeyValuePair<EventStack, DateSelectionModule> val in ClashingModules)
                        {
                            DateSelectionModule clashingMod = val.Value;
                            if ((bool)clashingMod.repeatCheckBox.IsChecked)
                            {
                                if ((int)clashingMod.cmbRepeatCount.SelectedValue >= 2)
                                {
                                    clashingMod.cmbRepeatCount.SelectedValue = (int)clashingMod.cmbRepeatCount.SelectedValue - 1;
                                    if ((int)clashingMod.cmbRepeatCount.SelectedValue == 1)
                                    {
                                        clashingMod.repeatCheckBox.IsChecked = false;
                                    }
                                }
                            }
                            else
                            {
                                modules.Remove(clashingMod);
                            }
                            if (ClashingElements.ContainsValue(val.Key))
                            {
                                DatesToRedistribute.Add(val.Key.EventStackDay);
                            }
                        }
       
                    }
                    foreach(DateTime date in DatesToRedistribute)
                    {
                        DateSelectionModule newModule = new DateSelectionModule(date, false);
                        newModule.IsValid = false;
                        newModule.WarningLabel.Text = "Conflit";
                        modules.Add(newModule);
                    }
                }
            }
            
            return evts;
        }

        private List<EventStack> CheckIfPossibleClashes(EventStack actual, List<EventStack> eventStacksList)
        {
            List<EventStack> clashingEvtStacks = EventsUtilities.FindClashingEvtStacks(actual, eventStacksList);
            if (clashingEvtStacks != null)
            {
                //Console.WriteLine("items found : " + clashingEvtStacks.Count);
                //foreach(EventStack evtStack in clashingEvtStacks)
                //{
                //    Console.WriteLine(evtStack.EventStackDay);
                //    foreach(Event evt in evtStack.Events)
                //    {
                //        Console.WriteLine(evt.Name);
                //    }
                //}
                return clashingEvtStacks;
            } else
            {
                return null;
            }
        }

        private bool CheckDatesValidity()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (!modules[i].IsValid)
                {
                    return false;
                }
            }
            return true;
        }

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            if (CheckDatesValidity())
            {
                //this.DialogResult = true;
                List<EventStack> results = defineDuplicateEvents();
                if (results.Count > 0)
                {
                    DBHandler.AddOrReplaceEventStacks(results);
                }
                //this.Close();
            } else
            {
                MessageBox.Show("Une ou plusieurs dates présentent un problème", "Dates non valides", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
