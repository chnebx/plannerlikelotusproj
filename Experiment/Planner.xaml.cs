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
        public Canvas previousCell;
        public Canvas selectedCell;
        public Predicate<Event> previousFormuleFilter = null;
        public GradientStop gradStop0 = new GradientStop();
        public LinearGradientBrush defaultMonthBrush = new LinearGradientBrush();
        public IDisposable formuleObservable;
        private EventStack fromEventStack;
        private int _previousHoveredRow = -1;

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

        public void RefreshEvents()
        {

            if (eventsCollection != null || eventsCollection.Count > 0)
            {
                FilterModule.Instance.clearFilterResults(eventsCollection);
            }
            eventsCollection = DBHandler.getEvents(currentYear);
            for (int i = 0; i < eventsCollection.Count; i++)
            {
                eventsCollection[i].updateEventsGrid();
            }
            var evts = (CollectionViewSource)this.FindResource("EventsViewSource");
            FilterModule.Instance.UpdateListWithFilterResults(eventsCollection);
            contentShow.ItemsSource = eventsCollection;
            if (evts.View != null)
            {
                evts.View.Refresh();
            }
        }

        public void BuildPlanner(DateTime targetTimer1)
        {
            currentYear = targetTimer1.Year;
            if (eventsCollection != null || eventsCollection.Count > 0)
            {
                FilterModule.Instance.clearFilterResults(eventsCollection);
            }
            eventsCollection = DBHandler.getEvents(currentYear);
            for (int i = 0; i < eventsCollection.Count; i++)
            {
                eventsCollection[i].updateEventsGrid();
            }
            var evts = (CollectionViewSource)this.FindResource("EventsViewSource");
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

        private void ToggleDayEditMode(Canvas actualCell)
        {
            if (actualCell.DataContext is Day)
            {
                var actualDay = (Day)actualCell.DataContext;
                if (actualDay.EditMode == true)
                {
                    actualDay.EditMode = false;
                }
                else
                {
                    actualDay.EditMode = true;
                }
            }
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = ((Canvas)sender).DataContext;
            var actualDay = (Day)selectedItem;
            if (_previousHoveredRow != -1)
            {
                monthControls[_previousHoveredRow].Background = CalendarSource.defaultMonthBrush;
            }
            monthControls[((Day)(((Canvas)sender).DataContext)).Row].Background = CalendarSource.hoveredMonthBrush;
            hoveredDate.Text = actualDay.DayFullName;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_previousHoveredRow == ((Day)(((Canvas)sender).DataContext)).Row)
            {
                return;
            }
            _previousHoveredRow = ((Day)(((Canvas)sender).DataContext)).Row;
            hoveredDate.Text = "(Aucune date)";
        }

        private void handleMonthHover(int actualRow)
        {
            if (_previousHoveredRow >= 0)
            {
                monthControls[_previousHoveredRow].Background = CalendarSource.defaultMonthBrush;
            }
            monthControls[actualRow].Background = CalendarSource.hoveredMonthBrush;
            _previousHoveredRow = actualRow;
        }

        private void ActualGrid_Loaded(object sender, RoutedEventArgs e)
        {
            actual = ((Grid)sender);
          
        }


        private void EventCreateHandler(object sender, MouseButtonEventArgs e)
        {
            _isDraggingItem = false;
            fromEventStack = null;
            if (sender is Canvas)
            {
                var elt = (Canvas)sender;
                Day selectedDay = (Day)elt.DataContext;
                DateTime current = selectedDay.Date;
                Point pointToWindow = Mouse.GetPosition(this);
                Point pointToScreen = PointToScreen(pointToWindow);
                
                if (selectedCell != null && selectedCell.DataContext is Day)
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
                EventsUtilities.UpdateLimits(evtStack);
                addEventDialog addDialog = new addEventDialog(evtStack, pointToScreen, false);
                if (addDialog.ShowDialog() == true)
                {
                    DBHandler.UpdateEventStack(evtStack);
                }
                if (evtStack.Events.Count <= 0)
                {
                    eventsCollection.Remove(evtStack);
                    DBHandler.DeleteEventStack(evtStack);
                    //if (evtStack == eventsInfo.UpcomingEvent)
                    //{
                    //    FindNextEventFromNow();
                    //}
                }
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
            selectedCell = null;
            //if (freshEvent.Current.Date < eventsInfo.UpcomingEvent.Current.Date && freshEvent.Current.Date > DateTime.Now)
            //{
            //    FindNextEventFromNow();
            //}
           
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
            object context;
            if (sender is Canvas)
            {
                context = ((Canvas)sender).DataContext;
            } else
            {
                context = ((Grid)sender).DataContext;
            }

            if (e.Data.GetDataPresent("EventFormat"))
            {
                EventsUtilities.DropHandler(context, e.Data.GetData("EventFormat"), fromEventStack, eventsCollection);
            } else
            {
                EventsUtilities.DropHandler(context, e.Data.GetData("EventStackFormat"), fromEventStack, eventsCollection);
            }
            RefreshEvents();
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
            
            handleMonthHover(evtStack.Row);
            //hoveredDate.Text = evtStack.Current.DayFullName;
        }

        private void EventStack_FullMouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void Calendar_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_previousHoveredRow >= 0)
            {
                monthControls[_previousHoveredRow].Background = CalendarSource.defaultMonthBrush;
            }
        }

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

        private void FullEvtStack_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            fromEventStack = ((EventStack)((Grid)sender).DataContext);
        }
    }
}
