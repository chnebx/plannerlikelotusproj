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

        public void BuildMonthPlanner(DateTime date)
        {
            ActualMonth = date.Month;
            ActualYear = date.Year;
            if (eventsCollection != null || eventsCollection.Count > 0)
            {
                FilterModule.Instance.clearFilterResults(eventsCollection);
            }
            MonthDays.Clear();
            eventsCollection.Clear();
            eventsCollection = DBHandler.getEvents(date.Year, date.Month);
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
                    DBHandler.AddEventStack(freshEvent);
                }
            }
        }

        private void Event_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var elt = (Border)sender;
            EventStack evtStack = (EventStack)elt.DataContext;
            Point pointToWindow = Mouse.GetPosition(this);
            Point pointToScreen = PointToScreen(pointToWindow);
            addEventDialog addDialog = new addEventDialog(evtStack, pointToScreen, false);
            if (addDialog.ShowDialog() == true)
            {
                if (evtStack.Events.Count > 0)
                {
                    DBHandler.UpdateEventStack(evtStack);
                }
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

        private EventStack FindItem(int id)
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
            if (context is Day)
            {
                if (e.Data.GetDataPresent("EventFormat"))
                {
                    Event evt = e.Data.GetData("EventFormat") as Event;
                    Day droppedOnDay = (Day)((Border)sender).DataContext;
                    EventStack newEvtStack = new EventStack {
                        Current = droppedOnDay
                    };
                    
                    if (!Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        EventStack previousStack = fromEventStack;
                        //EventStack previousStack = FindItem(evt.parentStack.Id);
                        int indexOfPreviousEvt = previousStack.Events.IndexOf(evt);
                        previousStack.RemoveEvent(indexOfPreviousEvt);
                        evt.parentStack = newEvtStack;
                        if (previousStack.Events.Count < 1)
                        {
                            eventsCollection.Remove(previousStack);
                        }
                        newEvtStack.AddEvent(evt);
                        DBHandler.HandleDragEvent(previousStack, newEvtStack, evt);
                    }
                    else
                    {
                        Event copiedEvent = evt.DeepCopy();
                        newEvtStack.AddEvent(copiedEvent);
                    }
                    eventsCollection.Add(newEvtStack);
                    //DBHandler.AddEventStack(newEvtStack);
                }
                else if (e.Data.GetDataPresent("EventStackFormat")) //if "EventStackFormat"
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
                        for (int i = 0; i < evtStack.Events.Count; i++)
                        {
                            newEvtStack.AddEvent(evtStack.Events[i].DeepCopy());
                        }
                        eventsCollection.Add(newEvtStack);
                        DBHandler.AddEventStack(newEvtStack);
                    }
                }
            } else if (context is EventStack)
            {
                EventStack actualStack = (EventStack)((Border)sender).DataContext;
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
                        }
                        else
                        {
                            Event copiedEvent = evt.DeepCopy();
                            actualStack.AddEvent(copiedEvent);
                        }
                        DBHandler.UpdateEventStack(actualStack);
                    }
                }
                else
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
                        }
                        else
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
            //if (_isDraggingItem)
            //{
            //    _isDraggingItem = false;
            //}
            _isDraggingEvent = false;
            _isDraggingEventStack = false;
            draggedEvent = null;
            fromEventStack = null;
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
            //if (sender is Border || sender is Grid)
            //{
            //    _isDraggingItem = true;
            //}
            //var element = ((Border)sender).DataContext;
            //Console.WriteLine(element.GetType());
            draggedEvent = (Event)(((Border)sender).DataContext);
            _isDraggingEvent = true;
        }

        private void Event_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            //if (_isDraggingItem)
            //{
            //    Point mousePos = e.GetPosition(null);
            //    Vector diff = StartPoint - mousePos;

            //    if (e.LeftButton == MouseButtonState.Pressed)
            //    {
            //        if (
            //        Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
            //        Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            //        {

            //            if (sender is Border)
            //            {
            //                Event eventSelected = (Event)((Border)sender).DataContext;

            //                // Initialize the drag & drop operation
            //                DataObject dragData = new DataObject("EventFormat", eventSelected);
            //                DragDrop.DoDragDrop((Border)sender, dragData, DragDropEffects.Move | DragDropEffects.Copy);
            //            }
            //            else
            //            {
            //                EventStack eventStackSelected = (EventStack)((Grid)sender).DataContext;

            //                // Initialize the drag & drop operation
            //                DataObject dragData = new DataObject("EventStackFormat", eventStackSelected);
            //                DragDrop.DoDragDrop((Grid)sender, dragData, DragDropEffects.Move | DragDropEffects.Copy);
            //            }

            //        }

            //    }
            //}

        }

        private void EvtStack_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDraggingEventStack = true;
        }

        private void EvtStack_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = StartPoint - mousePos;
            if (e.LeftButton == MouseButtonState.Pressed)
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
                        return;
                    }

                }
            }

        }

        private void FullEvt_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            fromEventStack = ((EventStack)((Border)sender).DataContext);
        }
    }
}
