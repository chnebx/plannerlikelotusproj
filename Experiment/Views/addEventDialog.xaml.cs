using Experiment.Models;
using Experiment;
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
using System.ComponentModel;
using Experiment.Converters;
using Experiment.Utilities;
using Experiment.CustomControls;
using WinForm = System.Windows.Forms;

namespace Experiment.Views
{
    /// <summary>
    /// Logique d'interaction pour addEventDialog.xaml
    /// </summary>
    public partial class addEventDialog : Window, INotifyPropertyChanged
    {
        private DateTime val;
        private string _additionalTimeInfo;
        private Point mouseLocation;
        public ObservableCollection<Event> eventsFromStack { get; set; }
        private Event _selectedEvent;
        public Day actualDay { get; set; }
        private Event crossingEvt = null;
        private bool creatingMode = false;
        private bool EventStackDidntExist = false;
        public Binding defaultVisibilityBinding = null;
        private LinearGradientBrush manageDayColor(Day currentDay)
        {
            if (currentDay.IsWE)
            {
                LinearGradientBrush color = new LinearGradientBrush();
                color.StartPoint = new Point(0.5, 0);
                color.EndPoint = new Point(0.5, 1);
                Color col0 = (Color)ColorConverter.ConvertFromString("#FFF0CD72");
                Color col1 = (Color)ColorConverter.ConvertFromString("#FFA2873F");
                color.GradientStops = new GradientStopCollection();
                color.GradientStops.Add(new GradientStop(col0, 0));
                color.GradientStops.Add(new GradientStop(col1, 1));
                return color;
            }
            else
            {
                LinearGradientBrush color = new LinearGradientBrush();
                color.StartPoint = new Point(0.5, 0);
                color.EndPoint = new Point(0.5, 1);
                Color col0 = (Color)ColorConverter.ConvertFromString("#FFE6E6E6");
                Color col1 = (Color)ColorConverter.ConvertFromString("#FFC1C1C1");
                color.GradientStops = new GradientStopCollection();
                color.GradientStops.Add(new GradientStop(col0, 0));
                color.GradientStops.Add(new GradientStop(col1, 1));
                return color;
            }
        }

        private void refreshVisibilityBinding(string property)
        {
            defaultVisibilityBinding = new Binding(property);
            defaultVisibilityBinding.Source = eventsList;
            defaultVisibilityBinding.Converter = new SelectedItemToVisibilityConverter();
        }

        public string AdditionalTimeInfo
        {
            get
            {
                return _additionalTimeInfo;
            }
            set
            {
                _additionalTimeInfo = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("AdditionalTimeInfo"));
            }
        }

        private EventStack _actualEventStack;
        public EventStack actualEventStack
        {
            get
            {
                return _actualEventStack;
            }
            set
            {
                _actualEventStack = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("actualEventStack"));
            }
        }

        private Event initializeNewEvent()
        {
            Band actualBand = DBHandler.getDefaultBand();
            Event newEvt = new Event
            {
                Band = actualBand,
                CurrentFormule = actualBand.Formules[0],
                Start = new DateTime(actualDay.Date.Year, actualDay.Date.Month, actualDay.Date.Day, 19, 00, 0),
                End = new DateTime(actualDay.Date.Year, actualDay.Date.Month, actualDay.Date.Day, 20, 00, 0),
                Name = "Aucun Titre",
                LocationName = null
            };
            newEvt.updateDuration();
            return newEvt;
            //SelectedEvent = newEvt;
            //actualEventStack.AddEvent(SelectedEvent);
        }

        private void initDialog(object parameterProvided, Point mouseLocation, bool automaticSelection)
        {
            
       
            DateTime val;
            
            if (parameterProvided is EventStack)
            {
                
                actualEventStack = (EventStack)parameterProvided;
                val = actualEventStack.EventStackDay;
                actualDay = new Day(val);
                eventsFromStack = actualEventStack.Events;
                if (eventsFromStack.Count == 0)
                {
                    SelectedEvent = initializeNewEvent();
                    actualEventStack.AddEvent(SelectedEvent);
                }
                refreshVisibilityBinding("SelectedItem");
                
                if (automaticSelection)
                {
                    toggleCreateMode();
                }
            } else //if parameterProvided isEvent
            {
                Console.WriteLine("event");
                actualEventStack = (EventStack)((Event)parameterProvided).parentStack;
                val = actualEventStack.EventStackDay;
                actualDay = new Day(val);
                eventsFromStack = actualEventStack.Events;
                eventsList.SelectedValue = (Event)parameterProvided;
                refreshVisibilityBinding("SelectedItem");
            }
            actualEventStack.sortEvents();
            LinearGradientBrush dayColor = manageDayColor(actualDay);
            titleHeaderContainer.Background = dayColor;
            startingDay = val.ToLongDateString().ToUpper();
            startDateText.Text = StartingDay;  
            if (actualDay.IsWE)
            {
                startDateText.Foreground = Brushes.White;
            }
            else
            {
                startDateText.Foreground = Brushes.DarkSlateGray;
            }
            Left = mouseLocation.X;
            Top = mouseLocation.Y;
            if (GlobalSettings.IsGraphModeAdvancedByDefault)
            {
                AdvancedModeBtn.IsChecked = true;
            } else
            {
                StandardModeBtn.IsChecked = true;
            }
            ActualDayScheduler.evtStack = actualEventStack;
            ActualDayScheduler.CurrentDay = actualDay.Date;
            ActualDayScheduler.LowerLimit = actualEventStack.LowerLimitHour;
            ActualDayScheduler.UpperLimit = actualEventStack.UpperLimitHour;
            ActualDayScheduler.Refresh();
            ActualDayScheduler.SelectDefault();
            
        }

        public addEventDialog(EventStack currentEvts, Point mouseLocation, bool automaticSelection)
        {
            InitializeComponent();
            DataContext = this;
            initDialog(currentEvts, mouseLocation, automaticSelection);
           
        }

        public addEventDialog(Event selectedEvent, Point mouseLocation)
        {
            InitializeComponent();
            DataContext = this;
            initDialog(selectedEvent, mouseLocation, false);
        }



        public Event newEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        public Event SelectedEvent
        {
            get
            {
                return _selectedEvent;
            }
            set
            {
                _selectedEvent = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SelectedEvent"));
            }
        }

        public Event result { get { return newEvent; } }

        private string startingDay;

       

        public string StartingDay
        {
            get { return startingDay; }
            set { startingDay = value; }
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            actualEventStack.updateEvts();
            List<Event> invalids = new List<Event>();
            for (int i = 0; i < actualEventStack.Events.Count; i++)
            {
                if (
                    actualEventStack.Events[i].ActualEmployer == null ||
                    actualEventStack.Events[i].LocationName == null
                    )
                { 
                    invalids.Add(actualEventStack.Events[i]);
                    actualEventStack.Events[i].IsValid = false;
                }
            }
            if (invalids.Count > 0)
            {
                string results = "";
                for (int j = 0; j < invalids.Count; j++)
                {
                    results += invalids[j].Name;
                    if (j < invalids.Count - 1)
                    {
                        results += ", ";
                    }
                }
                MessageBoxResult errorMsgResult = MessageBox.Show(String.Format("Les évènements suivants sont incomplets : {0} . Fermer quand même ??", results), "Attention", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (errorMsgResult)
                {
                    case MessageBoxResult.Yes:
                        break;
                    case MessageBoxResult.No:
                        return;
                }
            }
            else
            {
                for (int j = 0; j < actualEventStack.Events.Count; j++)
                {
                    actualEventStack.Events[j].IsValid = true;
                }
            }
            EventsUtilities.UpdateNeighborsLimits(actualEventStack);
            this.DialogResult = true;
            //Close();

        }

        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            //Close();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (creatingMode && eventsList.SelectedItem != null)
            {
                toggleCreateMode();
            }
        }

        private void BtnAddEvent_Click(object sender, RoutedEventArgs e)
        {
            toggleCreateMode();
            SelectedEvent = initializeNewEvent();
            actualEventStack.AddEvent(SelectedEvent);
            ActualDayScheduler.refreshGraph();
        }

        private void toggleCreateMode()
        {
            if (!creatingMode)
            {
                creatingMode = true;
                greenAddBtn.Visibility = Visibility.Collapsed;
                if (eventsList.SelectedItem != null)
                {
                    eventsList.SelectedIndex = -1;
                    eventsList.SelectedItem = null;
                }
                if (InfosPanel.Visibility != Visibility.Visible)
                {
                    InfosPanel.Visibility = Visibility.Visible;
                }
                DefaultDialogBtns.Visibility = Visibility.Hidden;
                BtnAddConfirm.Visibility = Visibility.Visible;
            } else
            {
                creatingMode = false;
                greenAddBtn.Visibility = Visibility.Visible;
                if (eventsList.SelectedItem == null)
                {
                    clearTxtBoxes();
                }
                InfosPanel.SetBinding(UIElement.VisibilityProperty, defaultVisibilityBinding);
                BtnAddConfirm.Visibility = Visibility.Hidden;
                DefaultDialogBtns.Visibility = Visibility.Visible;
            }
        }


        private void BtnAddConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtTitle.Text)
                && comboFormule.SelectedIndex != -1
                && SelectedEvent.ActualEmployer != null
                && SelectedEvent.CurrentFormule != null)
                {
                    actualEventStack.AddEvent(SelectedEvent);
                    toggleCreateMode();
                }
            
        }

        private void BtnCancelConfirm_Click(object sender, RoutedEventArgs e)
        {
            toggleCreateMode();
        }

        private void clearTxtBoxes()
        {
            txtTitle.Clear();
        }

        private void RedDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            DBHandler.DeleteEvent(SelectedEvent);
            actualEventStack.RemoveEvent(eventsList.SelectedIndex);
            ActualDayScheduler.refreshGraph();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            List<int> valuesConflicting = actualEventStack.CheckClash(null);
            if (valuesConflicting.Count > 0)
            {
                ClashDialog newClashDialog = new ClashDialog(actualEventStack, valuesConflicting);

                if (newClashDialog.ShowDialog() == true)
                {
                    newClashDialog.Close();
                    e.Cancel = true;
                    return;
                }
                e.Cancel = true;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        private void TitleHeaderContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void LengthSetting_Checked(object sender, RoutedEventArgs e)
        {
            BindingOperations.ClearBinding(LengthOrEndHourControl, HourSelector.HourValueProperty);
            BindingOperations.ClearBinding(LengthOrEndHourControl, HourSelector.MinutesValueProperty);
            Binding lengthHourOptionBinding = new Binding("ShowLengthHour");
            Binding lengthMinutesOptionBinding = new Binding("ShowLengthMinutes");
            lengthHourOptionBinding.Mode = BindingMode.TwoWay;
            lengthMinutesOptionBinding.Mode = BindingMode.TwoWay;
            LengthOrEndHourControl.SetBinding(HourSelector.HourValueProperty, lengthHourOptionBinding);
            LengthOrEndHourControl.SetBinding(HourSelector.MinutesValueProperty, lengthMinutesOptionBinding);
            EndLabel.Content = "Durée";
        }

        private void EndHourSetting_Checked(object sender, RoutedEventArgs e)
        {
            BindingOperations.ClearBinding(LengthOrEndHourControl, HourSelector.HourValueProperty);
            BindingOperations.ClearBinding(LengthOrEndHourControl, HourSelector.MinutesValueProperty);
            Binding lengthHourOptionBinding = new Binding("ShowEndHour");
            Binding lengthMinutesOptionBinding = new Binding("ShowEndMinutes");
            lengthHourOptionBinding.Mode = BindingMode.TwoWay;
            lengthMinutesOptionBinding.Mode = BindingMode.TwoWay;
            LengthOrEndHourControl.SetBinding(HourSelector.HourValueProperty, lengthHourOptionBinding);
            LengthOrEndHourControl.SetBinding(HourSelector.MinutesValueProperty, lengthMinutesOptionBinding);
            EndLabel.Content = "Fin";
        }

        private void ToggleBtnDesigns(bool EmployerDefined)
        {
            if (!EmployerDefined)
            {
                DefineEmployerBtn.Width = 80;
                DefineEmployerBtn.Content = "Définir";
                RemoveEmployerBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                DefineEmployerBtn.Content = "";
                DefineEmployerBtn.Width = 25;
                RemoveEmployerBtn.Visibility = Visibility.Visible;
            }
        }

        private void DefineEmployerBtn_Click(object sender, RoutedEventArgs e)
        {
            DefineEmployerDialogWindow newEmployerwindow = null;
            if (SelectedEvent != null && SelectedEvent.ActualEmployer != null)
            {
                newEmployerwindow = new DefineEmployerDialogWindow(SelectedEvent.ActualEmployer);
            } else
            {
                newEmployerwindow = new DefineEmployerDialogWindow(null);
            }
            
            newEmployerwindow.ShowDialog();
            if (newEmployerwindow.DialogResult == true)
            {
                SelectedEvent.ActualEmployer = newEmployerwindow.SelectedEmployer;
            }
        }

        private void RemoveEmployerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEvent != null)
            {
                SelectedEvent.ActualEmployer = null;
            }
        }

        private void DefineLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            DefineLocationDialogWindow newLocationWindow = null;
            if (SelectedEvent != null && SelectedEvent.LocationName != null)
            {
                newLocationWindow = new DefineLocationDialogWindow(SelectedEvent.LocationName);
            }
            else
            {
                newLocationWindow = new DefineLocationDialogWindow(null);
            }
            newLocationWindow.ShowDialog();
            if (newLocationWindow.DialogResult == true)
            {
                SelectedEvent.LocationName = newLocationWindow.SelectedLocation;
            }
        }

        private void RemoveLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEvent != null)
            {
                SelectedEvent.LocationName = null;
            }
        }

        private void BtnPaint_Click(object sender, RoutedEventArgs e)
        {
            WinForm.ColorDialog colorDialogBox = new WinForm.ColorDialog();
            var result = colorDialogBox.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SelectedEvent.SelectedColor = String.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", colorDialogBox.Color.A, colorDialogBox.Color.R, colorDialogBox.Color.G, colorDialogBox.Color.B);
                //SelectedEvent.ColorFill = new SolidColorBrush(Color.FromArgb(colorDialogBox.Color.A, colorDialogBox.Color.R, colorDialogBox.Color.G, colorDialogBox.Color.B));
            }
        }

        //private void GraphEventBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    Button actualBtn = (Button)sender;
        //    Event actualEvt = (Event)(actualBtn.DataContext);
        //    ActualDayScheduler.SelectedEventItem = actualEvt;
        //    ActualDayScheduler.ScrollToEvent(actualEvt);
        //}

        private void EventRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton actualBtn = (RadioButton)sender;
            Event actualEvt = (Event)(actualBtn.DataContext);
            ActualDayScheduler.SelectedEventItem = actualEvt;
            ActualDayScheduler.ScrollToEvent(actualEvt);
        }

       

        /*
        private void ChckBoxTomorrow_Checked(object sender, RoutedEventArgs e)
        {
            
            var checkedElement = ((CheckBox)sender).DataContext;
            var selectedEvent = (Event)checkedElement;

            if (crossingEvt != null)
            {
                crossingEvt.Over2Days = false;
                Console.WriteLine("before : " + crossingEvt.Name + ", " + crossingEvt.Over2Days);
                crossingEvt = selectedEvent;
                crossingEvt.Over2Days = true;
                Console.WriteLine("after : " + crossingEvt.Name + ", " + crossingEvt.Over2Days);
            } else
            {
                crossingEvt = selectedEvent;
                Console.WriteLine(selectedEvent.Name);
            }
            EventsUtilities.UpdateCrossingEvts(Planner.eventsCollection, actualEventStack, crossingEvt);
            //(Event)eventsList.SelectedItem
        }
        */
    }
}
