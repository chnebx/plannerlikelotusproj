using Experiment.Models;
using Experiment.Utilities;
using Experiment.Views;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Experiment
{
    /// <summary>
    /// Logique d'interaction pour MonthPlanner.xaml
    /// </summary>
    public partial class MonthPlanner : UserControl
    {
        public ObservableCollection<Day> MonthDays { get; set; }
        public ObservableCollection<EventStack> eventsCollection { get; set; }
        private List<TextBlock> _dayHeaders;
        private int _previousHoveredColumn;
        private int _actualMonth;
        private int _actualYear;
        private string _currentMonth;
        private Point StartPoint;
        private bool _isDraggingItem = false;
        private bool _isDraggingEvent = false;
        private bool _isDraggingEventStack = false;
        private EventStack fromEventStack = null;
        private Event draggedEvent = null;
        public DateTime ActualDate;


        public MonthPlanner()
        {
            InitializeComponent();
            DataContext = this;
            MonthDays = new ObservableCollection<Day>();
            eventsCollection = new ObservableCollection<EventStack>();
            _previousHoveredColumn = -1;
            _dayHeaders = new List<TextBlock>()
            {
                LundiHeader,
                MardiHeader,
                MercrediHeader,
                JeudiHeader,
                VendrediHeader,
                SamediHeader,
                DimancheHeader
            };

            //BuildMonthPlanner(DateTime.Now);
        }

        public StateManager CalendarState { get; set; }

        private void FindAndLitHeader(int column)
        {
            _dayHeaders[_previousHoveredColumn].Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EFF3F3F3"));
        }

        public int ActualMonth
        {
            get
            {
                return _actualMonth;
            }
            set
            {
                _actualMonth = value;
            }
        }

        public int ActualYear
        {
            get
            {
                return _actualYear;
            }
            set
            {
                _actualYear = value;
            }
        }

        public string CurrentMonth
        {
            get
            {
                return _currentMonth;
            }
            set
            {
                _currentMonth = value;

            }
        }

        public void RefreshEvents()
        {
            if (eventsCollection != null || eventsCollection.Count > 0)
            {
                FilterModule.Instance.clearFilterResults(eventsCollection);
            }
            eventsCollection.Clear();
            eventsCollection = DBHandler.getEvents(ActualDate.Year, ActualDate.Month);
            for (int i = 0; i < eventsCollection.Count; i++)
            {
                eventsCollection[i].updateEventsGrid();
            }
            FilterModule.Instance.UpdateListWithFilterResults(eventsCollection);
            eventsShow.ItemsSource = eventsCollection;
        }

        public void BuildMonthPlanner(DateTime date)
        {
            ActualDate = date;
            ActualMonth = date.Month;
            ActualYear = date.Year;
            if (eventsCollection != null || eventsCollection.Count > 0)
            {
                FilterModule.Instance.clearFilterResults(eventsCollection);
            }
            MonthDays.Clear();
            eventsCollection.Clear();
            eventsCollection = DBHandler.getEvents(date.Year, date.Month);
            for (int i = 0; i < eventsCollection.Count; i++)
            {
                eventsCollection[i].updateEventsGrid();
            }
            FilterModule.Instance.UpdateListWithFilterResults(eventsCollection);
            eventsShow.ItemsSource = eventsCollection;
            DateTime d = new DateTime(date.Year, date.Month, 1);
            int offset = ((int)d.DayOfWeek == 0) ? 6 : (int)d.DayOfWeek - 1;
            for (int i = 0; i < DateTime.DaysInMonth(date.Year, date.Month); i++)
            {
                Day currentDay = new Day(d);
                currentDay.DayType = Day._dayType.Enabled;
                currentDay.DayNum = d.Day.ToString();
                MonthDays.Add(currentDay);
                d = d.AddDays(1);
            }
            lblMonth.Text = date.ToString("MMMM") + "  " + date.Year.ToString();
        }

        private void BackwardBtn_Click(object sender, RoutedEventArgs e)
        {
            DateTime newMonth = new DateTime(ActualYear, ActualMonth, 1);
            newMonth = newMonth.AddMonths(-1);
            BuildMonthPlanner(newMonth);
        }

        private void ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            DateTime newMonth = new DateTime(ActualYear, ActualMonth, 1);
            newMonth = newMonth.AddMonths(1);
            BuildMonthPlanner(newMonth);
        }

        private void Day_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var elt = (Border)sender;
            Day selectedDay = (Day)elt.DataContext;
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
                    EventStackManager stackManager = new EventStackManager(freshEvent, freshEvent, newStack: true);
                    if (stackManager != null)
                    {
                        CalendarState.Do(stackManager);
                    }
                    //DBHandler.AddEventStack(freshEvent);
                }
            }
        }

        private void Event_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDraggingEvent = false;
            _isDraggingEventStack = false;
            draggedEvent = null;
            var elt = (Border)sender;
            EventStack evtStack = (EventStack)elt.DataContext;
            Point pointToWindow = Mouse.GetPosition(this);
            Point pointToScreen = PointToScreen(pointToWindow);
            EventsUtilities.UpdateLimits(evtStack);
            EventStack originalStack = EventStack.FullClone(evtStack);
            addEventDialog addDialog = new addEventDialog(evtStack, pointToScreen, false);
            if (addDialog.ShowDialog() == true)
            {
                EventStackManager evtStackManager = null;
                if (evtStack.Events.Count <= 0)
                {
                    eventsCollection.Remove(evtStack);
                    evtStackManager = new EventStackManager(originalStack, deleted: true);
                    //DBHandler.DeleteEventStack(evtStack);
                    //if (evtStack == eventsInfo.UpcomingEvent)
                    //{
                    //    FindNextEventFromNow();
                    //}
                }

                if (evtStack.Events.Count > 0)
                {
                    evtStackManager = new EventStackManager(originalStack, evtStack);
                    //DBHandler.InsertOrReplaceEventStack(evtStack);
                }
                if (evtStackManager != null)
                {
                    CalendarState.Do(evtStackManager);
                }
            }
        }

        private void Day_MouseEnter(object sender, MouseEventArgs e)
        { 
            if (_previousHoveredColumn != -1)
            {
                _dayHeaders[_previousHoveredColumn].Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8484AA"));
            }
            _dayHeaders[((Day)(((Border)sender).DataContext)).MonthColumn].Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e6b10e"));
        }

        private void Day_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_previousHoveredColumn == ((Day)(((Border)sender).DataContext)).MonthColumn)
            {
                return;
            }
            _previousHoveredColumn = ((Day)(((Border)sender).DataContext)).MonthColumn;
        }

        private EventStack FindItem(DateTime id)
        {
            for (int i = 0; i < eventsCollection.Count; i++)
            {
                if (eventsCollection[i].Id == id)
                {
                    return eventsCollection[i];
                }
            }
            return null;
        }

       
        private void MonthDay_Drop(object sender, DragEventArgs e)
        {
            var context = ((Border)sender).DataContext;
            DropManager drop;
            if (_isDraggingEvent || _isDraggingEventStack)
            {
                if (e.Data.GetDataPresent("EventFormat"))
                {
                    drop = EventsUtilities.DropHandler(context, e.Data.GetData("EventFormat"), fromEventStack, eventsCollection);
                } else
                {
                    drop = EventsUtilities.DropHandler(context, e.Data.GetData("EventStackFormat"), fromEventStack, eventsCollection);
                }
                if (drop != null && drop.ClashModule.SolvedEvents.Count > 0)
                {
                    CalendarState.Do(drop);
                }
                _isDraggingEvent = false;
                _isDraggingEventStack = false;
                draggedEvent = null;
                fromEventStack = null;
                RefreshEvents();
            }
            
        }

       

        

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_previousHoveredColumn != -1)
            {
                _dayHeaders[_previousHoveredColumn].Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8484AA"));
            }
        }

        private void MonthPlanner_Loaded(object sender, RoutedEventArgs e)
        {
            if (FilterModule.Instance != null)
            {
                FilterModule.Instance.clearFilterResults(eventsCollection);
                FilterModule.Instance.UpdateListWithFilterResults(eventsCollection);
            }
        }

        private void Event_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            StartPoint = e.GetPosition(null);
            _isDraggingEventStack = false;
            draggedEvent = (Event)(((Border)sender).DataContext);
            _isDraggingEvent = true;
        }

        private void EvtStack_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            StartPoint = e.GetPosition(null);
            _isDraggingEvent = false;
            _isDraggingEventStack = true;
        }

        private void EvtStack_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = StartPoint - mousePos;
            if (e.LeftButton == MouseButtonState.Pressed && (_isDraggingEvent || _isDraggingEventStack))
            {
                if (
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {

                    if (_isDraggingEvent)
                    {
                        Event eventSelected = draggedEvent;

                        // Initialize the drag & drop operation
                        DataObject dragData = new DataObject("EventFormat", eventSelected);
                        DragDrop.DoDragDrop((Border)sender, dragData, DragDropEffects.Move | DragDropEffects.Copy);
                    }
                    else if (_isDraggingEventStack)
                    {
                        EventStack eventStackSelected = fromEventStack;

                        // Initialize the drag & drop operation
                        DataObject dragData = new DataObject("EventStackFormat", eventStackSelected);
                        DragDrop.DoDragDrop((Border)sender, dragData, DragDropEffects.Move | DragDropEffects.Copy);
                    } else
                    {
                        _isDraggingEvent = false;
                        _isDraggingEventStack = false;
                        draggedEvent = null;
                        return;
                    }

                }
            }

        }

        private void FullEvt_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            fromEventStack = ((EventStack)((Border)sender).DataContext);
        }

        private void BtnMonthUndo_Click(object sender, RoutedEventArgs e)
        {
            CalendarState.Undo();
            RefreshEvents();
        }

        private void BtnMonthRedo_Click(object sender, RoutedEventArgs e)
        {
            CalendarState.Redo();
            RefreshEvents();
        }
    }
}
