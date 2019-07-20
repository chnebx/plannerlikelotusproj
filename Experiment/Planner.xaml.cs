using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Experiment.Models;
using Experiment.Utilities;
using Experiment.Views;

namespace Experiment
{
    /// <summary>
    /// Logique d'interaction pour Planner.xaml
    /// </summary>
    public partial class Planner : UserControl
    {
        public ObservableCollection<String> dayNames { get; set; }
        public ObservableCollection<String> monthNames { get; set; }
        public ObservableCollection<EventStack> eventsCollection { get; set; }
        public ObservableCollection<Day> yearDays { get; set; }
        private ObservableCollection<Border>monthControls { get; set; }
        private bool _isDraggingItem = false;
        private Point startPoint;
        public FilterModule filterModule { get; set; }
        public ObservableCollection<Formule> formules { get; set; }
        public Grid actual;
        public EventsInfo eventsInfo { get; set; }
        public int distanceFromUpcomingEvent { get; set; }
        private int hoveredMonth = -1;
        public CollectionViewSource evtsCvs = null;
        private static Planner _instance;
        public DateTime targetTimer
        {
            get { return (DateTime)GetValue(targetTimerProperty); }
            set { SetValue(targetTimerProperty, value); }
        }
        public Border previousCell;
        public Border selectedCell;
        public Predicate<Event> previousFormuleFilter = null;
        public GradientStop gradStop0 = new GradientStop();
        public LinearGradientBrush defaultMonthBrush = new LinearGradientBrush();
        public IDisposable formuleObservable;
       
        // Using a DependencyProperty as the backing store for targetTimer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty targetTimerProperty =
            DependencyProperty.Register("targetTimer", typeof(DateTime), typeof(Planner), new PropertyMetadata(DateTime.Now));

        

        public Planner()
        {
            InitializeComponent();
            DataContext = this;
            dayNames = CalendarSource.getDaysNames();
            monthNames = CalendarSource.getMonthNames();
            yearDays = new ObservableCollection<Day>();
            eventsInfo = new EventsInfo();
            eventsCollection = DBHandler.getEvents(DateTime.Now.Year);
            FindNextEventFromNow();
            hoveredDate.Text = "(Aucune date)";
            formules = new ObservableCollection<Formule>();
            //formules = new ObservableCollection<Formule>(formules.Concat(DBHandler.getFormules()));
            monthControls = new ObservableCollection<Border>()
            {
                JanvierHeader,
                FévrierHeader,
                MarsHeader,
                AvrilHeader,
                MaiHeader,
                JuinHeader,
                JuilletHeader,
                AoutHeader,
                SeptembreHeader,
                OctobreHeader,
                NovembreHeader,
                DecembreHeader
            };
            evtsCvs = (CollectionViewSource)this.FindResource("EventsViewSource");
        }

        public static Planner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Planner();
                }
                return _instance;
            }
        }

        private void FindNextEventFromNow()
        {
            //if (currentYear != DateTime.Now.Year)
            //{
            //    return;
            //}
            //List<EventStack> results = eventsCollection.OrderBy((x) => x.Current.Date)
            //    .Where((x) => DateTime.Compare(x.Current.Date, DateTime.Now.Date) >= 0).ToList<EventStack>();
            //if (results.Count > 0)
            //{
            //    eventsInfo.UpcomingEvent = results.FirstOrDefault<EventStack>();
            //} else
            //{
            //    eventsInfo.UpcomingEvent = null;
            //    eventsInfo.Header = "( aucune date )";
            //}
            
            //DateTime startTime = DateTime.Now;


            /*
            DateTime endTime = eventsInfo.UpcomingEvent.Current.Date;
            TimeSpan distance = endTime.Subtract(startTime);
            distanceFromUpcomingEvent = (int)distance.TotalMinutes;
            */
        }

        public int currentYear
        {
            get { return (int)GetValue(currentYearProperty); }
            set { SetValue(currentYearProperty, value); }
        }


        // Using a DependencyProperty as the backing store for currentYear.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty currentYearProperty =
            DependencyProperty.Register("currentYear", typeof(int), typeof(Planner), new PropertyMetadata(DateTime.Now.Year));


        public void BuildPlanner(DateTime targetTimer1)
        {
            currentYear = targetTimer1.Year;
            if (eventsCollection != null || eventsCollection.Count > 0)
            {
                FilterModule.Instance.clearFilterResults(eventsCollection);
            }
            eventsCollection = DBHandler.getEvents(currentYear);
            var evts = (CollectionViewSource)this.FindResource("EventsViewSource");
            //filterModule.FilteredCollection = eventsCollection;
            //filterModule.RefreshFilter();
            FilterModule.Instance.UpdateListWithFilterResults(eventsCollection);
            contentShow.ItemsSource = eventsCollection;
            if (evts.View != null)
            {
                evts.View.Refresh();
            }
            yearDays.Clear();
            DateTime d = new DateTime(targetTimer1.Year, 1, 1);

            for (int m = 1; m < 13; m++)
            {
                int offset = ((int)d.DayOfWeek == 0) ? 6 : (int)d.DayOfWeek - 1;
                int dayOfYear = 37 * (m - 1) + offset;
                int upTo = dayOfYear + DateTime.DaysInMonth(targetTimer1.Year, m) - 1;
                for (int day = dayOfYear; day <= upTo; day++)
                {
                    Day currentDay = new Day(d);
                    //currentDay.Date = d;
                    currentDay.DayType = Day._dayType.Enabled;
                    if (d == DateTime.Today)
                    {
                        currentDay.DayType = Day._dayType.IsToday;
                    }
                    if ((int)d.DayOfWeek == 0 || (int)d.DayOfWeek == 6)
                    {
                        currentDay.DayType = Day._dayType.IsWE;
                    }
                    currentDay.DayNum = d.Day.ToString();
                    yearDays.Add(currentDay);
                    d = d.AddDays(1);
                }
            }
        }

        private void handleMonthHover(Day actualDay)
        {
            if (actualDay is null)
            {
                monthControls[hoveredMonth].Background = CalendarSource.defaultMonthBrush;
                hoveredMonth = -1;
                return;
            }
            int actualDayMonth = actualDay.Date.Month - 1;
            if (hoveredMonth == -1)
            {
                hoveredMonth = actualDayMonth;
                monthControls[actualDayMonth].Background = CalendarSource.hoveredMonthBrush;
            }
            else if (actualDayMonth != hoveredMonth)
            {

                monthControls[hoveredMonth].Background = CalendarSource.defaultMonthBrush;
                monthControls[actualDayMonth].Background = CalendarSource.hoveredMonthBrush;
                hoveredMonth = actualDayMonth;
            }
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = ((Border)sender).DataContext;
            var actualDay = (Day)selectedItem;
            previousCell = ((Border)sender);

            ToggleDayBrush(previousCell, true);
            handleMonthHover(actualDay);
            hoveredDate.Text = actualDay.DayFullName;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            var prevCell = previousCell;
            if (prevCell.DataContext is Day)
            {
                var prevDay = (Day)prevCell.DataContext;
                if (selectedCell == null || prevDay != (Day)selectedCell.DataContext)
                {
                    ToggleDayBrush(previousCell, false);
                    hoveredDate.Text = "(Aucune date)";
                }
            }
            
        }

        private void ActualGrid_Loaded(object sender, RoutedEventArgs e)
        {
            actual = ((Grid)sender);
          
        }


        private void EventCreateHandler(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border)
            {

                var elt = (Border)sender;
                Day selectedDay = (Day)elt.DataContext;
                DateTime current = selectedDay.Date;
                Point pointToWindow = Mouse.GetPosition(this);
                Point pointToScreen = PointToScreen(pointToWindow);
                
                if (selectedCell != null)
                {
                    var previousDay = (Day)selectedCell.DataContext;
                    ToggleDayEditMode(selectedCell);
                    
                    if (selectedDay == previousDay)
                    {
                        previousCell = selectedCell;
                        selectedCell = null;
                        return;
                    }
                }   
                selectedCell = elt;
                ToggleDayEditMode(selectedCell);

            } else if (sender is Grid)
            {
                var elt = (Grid)sender;
                EventStack evtStack = (EventStack)elt.DataContext;
                if (selectedCell != null)
                {
                    ToggleDayEditMode(selectedCell);
                    selectedCell = null;
                }
                Point pointToWindow = Mouse.GetPosition(this);
                Point pointToScreen = PointToScreen(pointToWindow);
                addEventDialog addDialog = new addEventDialog(evtStack, pointToScreen, false);
                if (addDialog.ShowDialog() == true)
                {

                }
                if (evtStack.Events.Count <= 0)
                {
                    eventsCollection.Remove(evtStack);
                    DBHandler.DeleteEventStack(evtStack);
                    if (evtStack == eventsInfo.UpcomingEvent)
                    {
                        FindNextEventFromNow();
                    }
                }
            }
            
        }

        private void ToggleDayEditMode(Border actualCell)
        {
            var actualDay = (Day)actualCell.DataContext;
            if (actualDay.EditMode == true)
            {
                ToggleDayBrush(actualCell, false);
                actualDay.EditMode = false;
            } else
            {
                ToggleDayBrush(actualCell, true);
                actualDay.EditMode = true;
            }
        }

        private void ToggleDayBrush(Border actualCell, bool active)
        {
            if (!active)
            {
                actualCell.BorderBrush = Brushes.DarkGray;
                actualCell.BorderThickness = new Thickness(0.2);
            } else
            {
                actualCell.BorderBrush = Brushes.LightSteelBlue;
                actualCell.BorderThickness = new Thickness(3);
            }
            
        }

        private void EventsView_Filter(object sender, FilterEventArgs e)
        {  
            e.Accepted = (e.Item as EventStack).Current.Date.Year == currentYear;
        }

        private void AddEvent_Click(object sender, RoutedEventArgs e)
        {
            var elt = (Button)sender;
            Day selectedDay = (Day)elt.DataContext;
            selectedDay.EditMode = false;
            DateTime current = selectedDay.Date;
            Point pointToWindow = Mouse.GetPosition(this);
            Point pointToScreen = PointToScreen(pointToWindow);
            EventStack freshEvent = new EventStack {
                Current = selectedDay
            };
            EventsUtilities.UpdateLimits(freshEvent);
            addEventDialog addDialog = new addEventDialog(freshEvent, pointToScreen, true);
            if (addDialog.ShowDialog() == true)
            {
                if (freshEvent.Events.Count > 0)
                {
                    eventsCollection.Add(freshEvent);
                    DBHandler.AddEventStack(freshEvent);
                }
            }
            
            ToggleDayBrush(selectedCell, false);
            selectedCell = null;
            if (freshEvent.Current.Date < eventsInfo.UpcomingEvent.Current.Date && freshEvent.Current.Date > DateTime.Now)
            {
                FindNextEventFromNow();
            }
           
        }

        private void Events_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
            if (sender is Rectangle || sender is Border)
            {
                _isDraggingItem = true;
            }
        }

        public static T FindParent<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);

            if (parent == null) return null;

            var parentT = parent as T;
            return parentT ?? FindParent<T>(parent);
        }

        private void Events_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDraggingItem)
            {
                e.Handled = true;
                Point mousePos = e.GetPosition(null);
                Vector diff = startPoint - mousePos;

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (
                    Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                    {
                        if (sender is Rectangle)
                        {
                            Event eventSelected = (Event)((Rectangle)sender).DataContext;

                            // Initialize the drag & drop operation
                            DataObject dragData = new DataObject("EventFormat", eventSelected);
                            DragDrop.DoDragDrop((Rectangle)sender, dragData, DragDropEffects.Move | DragDropEffects.Copy);
                        }
                        else
                        {
                            EventStack eventStackSelected = (EventStack)((Border)sender).DataContext;

                            // Initialize the drag & drop operation
                            DataObject dragData = new DataObject("EventStackFormat", eventStackSelected);
                            DragDrop.DoDragDrop((Border)sender, dragData, DragDropEffects.Move | DragDropEffects.Copy);
                        }

                    }

                }
            }
        }

        private void Day_Drop(object sender, DragEventArgs e)
        {
            if (sender is Border)
            {
                if (e.Data.GetDataPresent("EventFormat"))
                {

                    Event evt = e.Data.GetData("EventFormat") as Event;
                    Day droppedOnDay = (Day)((Border)sender).DataContext;
                    EventStack newEvtStack = new EventStack {
                        Current = droppedOnDay
                    };
                    if(!Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        EventStack previousStack = evt.parentStack;
                        int indexOfPreviousEvt = previousStack.Events.IndexOf(evt);
                        previousStack.RemoveEvent(indexOfPreviousEvt);
                        DBHandler.UpdateEventStack(previousStack);
                        if (previousStack.Events.Count < 1)
                        {
                            eventsCollection.Remove(previousStack);
                            DBHandler.DeleteEventStack(previousStack);
                        }
                        newEvtStack.AddEvent(evt);
                    } else
                    {
                        Event copiedEvent = evt.DeepCopy();
                        newEvtStack.AddEvent(copiedEvent);
                    }
                    eventsCollection.Add(newEvtStack);
                    DBHandler.AddEventStack(newEvtStack);
                } else //if "EventStackFormat"
                {
                    EventStack evtStack = e.Data.GetData("EventStackFormat") as EventStack;
                    Day droppedOnDay = (Day)((Border)sender).DataContext;
                    if (!Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        evtStack.Current = droppedOnDay;
                        DBHandler.UpdateEventStack(evtStack);
                    }
                    else
                    {
                        EventStack newEvtStack = new EventStack {
                            Current = droppedOnDay
                        };
                        for(int i = 0; i < evtStack.Events.Count; i++)
                        {
                            newEvtStack.AddEvent(evtStack.Events[i].DeepCopy());
                        }
                        eventsCollection.Add(newEvtStack);
                        DBHandler.AddEventStack(newEvtStack);
                    }
                }
            } else if (sender is Grid)
            {
                EventStack actualStack = (EventStack)((Grid)sender).DataContext;
                if (e.Data.GetDataPresent("EventFormat"))
                {
                    Event evt = e.Data.GetData("EventFormat") as Event;
                    EventStack previousStack = evt.parentStack;
                    
                    if (actualStack != previousStack && actualStack.Events.Count < 3)
                    {
                        if (!Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            int indexOfPreviousEvt = previousStack.Events.IndexOf(evt);
                            previousStack.RemoveEvent(indexOfPreviousEvt);
                            DBHandler.UpdateEventStack(previousStack);
                            if (previousStack.Events.Count < 1)
                            {
                                eventsCollection.Remove(previousStack);
                                DBHandler.DeleteEventStack(previousStack);
                            }
                            actualStack.AddEvent(evt);
                        } else
                        {
                            Event copiedEvent = evt.DeepCopy();
                            actualStack.AddEvent(copiedEvent);
                        }
                        DBHandler.UpdateEventStack(actualStack);
                    } 
                } else
                {
                    EventStack evtStack = e.Data.GetData("EventStackFormat") as EventStack;
                    if (evtStack != actualStack && evtStack.Events.Count + actualStack.Events.Count <= 3)
                    {
                        if (!Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            for (int i = 0; i < evtStack.Events.Count; i++)
                            {
                                actualStack.AddEvent(evtStack.Events[i]);
                            }
                            eventsCollection.Remove(evtStack);
                            DBHandler.DeleteEventStack(evtStack);
                        } else
                        {
                            for (int i = 0; i < evtStack.Events.Count; i++)
                            {
                                actualStack.AddEvent(evtStack.Events[i].DeepCopy());
                            }
                        }
                        DBHandler.UpdateEventStack(actualStack);
                    }
                }
            }
            if (_isDraggingItem)
            {
                _isDraggingItem = false;
            }
            FindNextEventFromNow();
        }

        public class GenericCloner<T> where T : class
        {
            public T Clone(T obj)
            {
                using (var ms = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(ms, obj);
                    ms.Position = 0;
                    return (T)formatter.Deserialize(ms);
                }
            }
        }

        private void EventStack_FullMouseEnter(object sender, MouseEventArgs e)
        {
            EventStack evtStack = (EventStack)(((Grid)sender).DataContext);
            
            handleMonthHover(evtStack.Current);
            //hoveredDate.Text = evtStack.Current.DayFullName;
        }

        private void EventStack_FullMouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void Calendar_MouseLeave(object sender, MouseEventArgs e)
        {
            if (hoveredMonth >= 0)
            {
                handleMonthHover(null);
            }
        }

        //public bool EmployerFilter(object employer)
        //{
            
        //    if (String.IsNullOrEmpty(txtBoxFilterEmployer.Text))
        //        return true;

        //    EventStack evtStack = (EventStack)employer;


        //    if (evtStack.containsEmployer(txtBoxFilterEmployer.Text))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //public void Employer_Filter(object sender, FilterEventArgs e)
        //{

        //    if (String.IsNullOrEmpty(txtBoxFilterEmployer.Text))
        //        e.Accepted = true;

        //    EventStack evtStack = (EventStack)e.Item;

        //    if (evtStack.containsEmployer(txtBoxFilterEmployer.Text))
        //    {
        //        e.Accepted = true;
        //    }
           

        //}

        private void filteredEvent_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (Border)sender;
            Event selected = (Event)element.DataContext;
            Point pointToWindow = Mouse.GetPosition(this);
            Point pointToScreen = PointToScreen(pointToWindow);
            addEventDialog newEventDialog = new addEventDialog(selected, pointToScreen);
            if (newEventDialog.ShowDialog() == true)
            {

            }
        }

        //private void Reset_Click(object sender, RoutedEventArgs e)
        //{
        //    txtBoxFilterComment.Clear();
        //    txtBoxFilterEmployer.Clear();
        //    txtBoxFilterLength.Clear();
        //    txtBoxFilterLocation.Clear();
        //    txtBoxFilterTitle.Clear();
        //    if (comboBoxFormules.SelectedIndex != 0)
        //    {
        //        comboBoxFormules.SelectedIndex = 0;     
        //    }
        //}

        private void BackwardBtn_Click(object sender, RoutedEventArgs e)
        {
            int newYear = currentYear - 1;
            DateTime targetDate = new DateTime(newYear, 1, 1);
            currentYear = newYear;
            BuildPlanner(targetDate);
        }

        private void ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            int newYear = currentYear + 1;
            DateTime targetDate = new DateTime(newYear, 1, 1);
            currentYear = newYear;
            BuildPlanner(targetDate);
        }

        private void Planner_Loaded(object sender, RoutedEventArgs e)
        {
            if (FilterModule.Instance != null)
            {
                FilterModule.Instance.clearFilterResults(eventsCollection);
                FilterModule.Instance.UpdateListWithFilterResults(eventsCollection);
            }
            
        }
    }
}
