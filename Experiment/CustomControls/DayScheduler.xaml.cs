﻿using Experiment.Converters;
using Experiment.Models;
using Experiment.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Experiment.CustomControls
{
    /// <summary>
    /// Logique d'interaction pour DayScheduler.xaml
    /// </summary>
    public partial class DayScheduler : UserControl, INotifyPropertyChanged
    {

        public ObservableCollection<Event> GridEvents;
        //public Event SelectedEventItem { get; set; }
        public static string DayTitle { get; set; }
        public Border actualSelectedEventBorder { get; set; }
        public Border draggedItem;
        public Border previousItem;
        double registeredSize = 0;
        Point itemRelativePosition;
        Point OnEventPosition;
        private bool IsResizing;
        private bool IsDragging;
        private bool IsCreating;
        private bool IsDeleting;
        private string _scrollInfo;
        private DateTime _lowerLimit;
        private DateTime _upperLimit;
        public DateTime CurrentDay;
        private double GridSize = (double)50/60;
        public double LowerLimitMarginTop { get; set; }
        public double UpperLimitMarginTop { get; set; }
        private Line creationLine;
        private Border CreationInfo;
        private EventStack _evtStack;
        public ObservableCollection<Border> DrawnEventsList;


        public DayScheduler()
        {
            InitializeComponent();
            GridEvents = new ObservableCollection<Event>();
            DrawnEventsList = new ObservableCollection<Border>();
            //DrawnEventsList.CollectionChanged += new NotifyCollectionChangedEventHandler(DrawnEventsList_CollectionChanged);
            IsDragging = false;
            IsCreating = false;
            CurrentDay = new DateTime();
            creationLine = new Line();
            creationLine.Stroke = System.Windows.Media.Brushes.Red;
            creationLine.StrokeThickness = 2;
            creationLine.Visibility = Visibility.Collapsed;
            CreationInfo = new Border();
            CreationInfo.Width = 80;
            CreationInfo.Background = Brushes.Red;
            CreationInfo.CornerRadius = new CornerRadius(0, 0, 5, 5);
            TextBlock contentText = new TextBlock();
            contentText.HorizontalAlignment = HorizontalAlignment.Center;
            contentText.Foreground = Brushes.White;
            CreationInfo.Child = contentText;
            CreationInfo.Visibility = Visibility.Collapsed;
            //DrawEvents();
        }

        public EventStack evtStack
        {
            get
            {
                return _evtStack;
            }
            set
            {
                _evtStack = value;
                refreshGraph();
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("evtStack"));
            }
        }

        public void refreshGraph()
        {
            if (DrawnEventsList.Count > 0)
            {
                DrawnEventsList.Clear();
            }
            foreach (Event e in evtStack.Events)
            {
                DrawnEventsList.Add(createBorderEvent(e));
            }
            Refresh();
        }

        public DateTime LowerLimit
        {
            get
            {
                return _lowerLimit;
            }
            set
            {
                _lowerLimit = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("LowerLimit"));
            }
        }

        public DateTime UpperLimit
        {
            get
            {
                return _upperLimit;
            }
            set
            {
                _upperLimit = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("UpperLimit"));
            }
        }

        public DateTime ActualGridDay
        {
            get { return (DateTime)GetValue(ActualGridDayProperty); }
            set { SetValue(ActualGridDayProperty, value); }
        }

        public string ScrollInfo
        {
            get
            {
                return _scrollInfo;
            }
            set
            {
                _scrollInfo = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ScrollInfo"));
            }
        }

        // Using a DependencyProperty as the backing store for ActualGridDay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualGridDayProperty =
            DependencyProperty.Register("ActualGridDay", typeof(DateTime), typeof(DayScheduler), new PropertyMetadata(new PropertyChangedCallback(OnDayChanged)));

        private static void OnDayChanged(DependencyObject d,
         DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var instance = d as DayScheduler;
                //DayTitle = ((DateTime)e.NewValue).ToLongDateString();
                instance.CurrentDay = (DateTime)e.NewValue;
                instance.DrawLimits();
            }
        }


        public Event SelectedEventItem
        {
            get { return (Event)GetValue(SelectedEventItemProperty); }
            set { SetValue(SelectedEventItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedEvent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedEventItemProperty =
            DependencyProperty.Register("SelectedEventItem", typeof(Event), typeof(DayScheduler), new PropertyMetadata(new PropertyChangedCallback(OnSelectedEventChanged)));

        private static void OnSelectedEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
         
        }

        public void SelectDefault()
        {
            if (evtStack.Events.Count > 0)
            {
                ScrollToEvent(evtStack.Events[0]);
            }
        }

        public void AddAndUpdate(Event e)
        {
            if (DrawnEventsList.Count < 3)
            {
                DrawnEventsList.Add(createBorderEvent(e));
            }
            Refresh();
        }

        public void DeleteAndUpdate(Event e)
        {
            for (int i = 0; i < DrawnEventsList.Count; i++)
            {
                if (DrawnEventsList[i].DataContext == e)
                {
                    DrawnEventsList.RemoveAt(i);
                }
            }
            Refresh();
        }

        private Border createBorderEvent(Event e)
        {
            Border wEvent = new Border();
            Border containedTitle = new Border();
            TextBlock Title = new TextBlock();
            Title.FontSize = 16;
            Title.FontWeight = FontWeights.Bold;
            containedTitle.Child = Title;
            containedTitle.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#88ffffff"));
            containedTitle.Opacity = 0.5;
            containedTitle.Height = 20;
            containedTitle.VerticalAlignment = VerticalAlignment.Top;
            containedTitle.HorizontalAlignment = HorizontalAlignment.Center;
            containedTitle.CornerRadius = new CornerRadius(0, 0, 10, 10);
            containedTitle.IsHitTestVisible = false;
            Title.VerticalAlignment = VerticalAlignment.Center;
            Title.HorizontalAlignment = HorizontalAlignment.Center;
            Title.Padding = new Thickness(20);
            wEvent.Child = containedTitle;
            wEvent.DataContext = e;
            wEvent.Width = 320;
            wEvent.CornerRadius = new CornerRadius(5);
            Binding backgroundBinding = new Binding("ColorFill");
            backgroundBinding.Source = e;
            backgroundBinding.Converter = new BrushToGradientConverter();
            Binding marginTopBinding = new Binding("Start");
            marginTopBinding.Source = e;
            marginTopBinding.Converter = new DateTimeToMarginTopConverter();
            Binding heightBinding = new Binding("Duration");
            heightBinding.Source = e;
            heightBinding.Converter = new DurationToHeightConverter();
            Binding titleBinding = new Binding("Name");
            titleBinding.Source = e;
            wEvent.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ebbc2e"));
            Title.SetBinding(TextBlock.TextProperty, titleBinding);
            wEvent.SetBinding(Border.MarginProperty, marginTopBinding);
            wEvent.SetBinding(Border.BackgroundProperty, backgroundBinding);
            wEvent.SetBinding(Border.HeightProperty, heightBinding);
            wEvent.PreviewMouseLeftButtonDown += Event_PreviewMouseLeftButtonDown;
            wEvent.PreviewMouseLeftButtonUp += Event_PreviewMouseLeftButtonUp;
            wEvent.PreviewMouseMove += Event_PreviewMouseMove;
            wEvent.AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new RoutedEventHandler(Event_PreviewMouseDownOutsideCapturedElementEvent));
            wEvent.AddHandler(Mouse.PreviewMouseUpOutsideCapturedElementEvent, new RoutedEventHandler(Event_PreviewMouseUpOutsideCapturedElementEvent));
            return wEvent;
        }

        public string DetermineTimeOfDayScroll(double scrollValue)
        {
            if (scrollValue < 8 * 50)
            {
                return "Nuit";
            } else if (scrollValue >= 8 * 50 && scrollValue < 12 * 50)
            {
                return "Matin";
            } else if (scrollValue >= 12 * 50 && scrollValue < 13 * 50)
            {
                return "Midi";
            } else if (scrollValue > 13 * 50 && scrollValue < 17 * 50)
            {
                return "Après-Midi";
            } else if (scrollValue >= 17 * 50 && scrollValue < 24 * 50)
            {
                return "Soir";
            } else if (scrollValue >= 24 * 50 && scrollValue < 32 * 50)
            {
                return "Nuit";
            }
            else if (scrollValue >= 32 * 50 && scrollValue < 36 * 50)
            {
                return "Lendemain Matin";
            } else 
            {
                return "Lendemain Midi";
            }
        }

        // Using a DependencyProperty as the backing store for evtStack.  This enables animation, styling, binding, etc...
        

        // Using a DependencyProperty as the backing store for DrawnEventsList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrawnEventsListProperty =
            DependencyProperty.Register("DrawnEventsList", typeof(ObservableCollection<Event>), typeof(DayScheduler), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnEventsListChanged)));

        public event PropertyChangedEventHandler PropertyChanged;

        private static void OnEventsListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as DayScheduler;
            var old = (ObservableCollection<Border>)e.OldValue;
            if (old != null)
            {
                
                old.CollectionChanged -= instance.DrawnEventsList_CollectionChanged;
                
            }
            if (e.NewValue != null)
            {
                
                instance.DrawnEventsList = ((ObservableCollection<Border>)e.NewValue);
                instance.DrawnEventsList.CollectionChanged += instance.DrawnEventsList_CollectionChanged;
              
            }
            
        }

        private void DrawnEventsList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Refresh();
        }

        public void Refresh()
        {
            DrawEvents();
        }

        public void ScrollToEvent(Event evt)
        {
            if (evtStack.Events.Contains(evt))
            {
                double position = evt.Start.Hour * 50;
                if (position - 50 > 0)
                {
                    position -= 50;
                } 
                sv.ScrollToVerticalOffset(position);
            }
        }

        public void DrawLimits()
        {
            if (LowerLimit != null)
            {
                LowerLimitMarginTop = 50 * (LowerLimit.Hour + (LowerLimit.Minute / 60.0));
            } else
            {
                LowerLimitMarginTop = 0.0;
            }

            if (UpperLimit != null)
            {
                UpperLimitMarginTop = 50 * (UpperLimit.Hour + (UpperLimit.Minute / 60.0));
                if (UpperLimit.Day != CurrentDay.Day)
                {
                    UpperLimitMarginTop += 1199.55;
                }
            }
            else
            {
                UpperLimitMarginTop = 1199.5;
            }
            Line lower = new Line();
            lower.Stroke = Brushes.Red;
            lower.X1 = 0;
            lower.Y1 = LowerLimitMarginTop;
            lower.X2 = 320;
            lower.Y2 = LowerLimitMarginTop;
            lower.StrokeThickness = 2;

            Line upper = new Line();
            upper.Stroke = Brushes.Red;
            upper.X1 = 0;
            upper.Y1 = UpperLimitMarginTop;
            upper.X2 = 320;
            upper.Y2 = UpperLimitMarginTop;
            upper.StrokeThickness = 2;
            column.Children.Add(lower);
            column.Children.Add(upper);
        }

        private int getIndex(Event evt, List<Event> evtsList)
        {
            for (int i = 0; i < evtsList.Count; i++)
            {
                if (evt == evtsList[i])
                {
                    return i;
                }
            }
            return 0;
        }

        private void DrawEvents()
        {
            column.Children.Clear();
            //double columnWidth = EventsGrid.ColumnDefinitions[1].Width.Value;
            double columnWidth = 320;
            DrawLimits();
            //foreach (Event e in evtStack.Events)
            foreach(Border elem in DrawnEventsList)
            {
                //column.Width = columnWidth;

                double oneHourHeight = 50;// column.ActualHeight / 46;
                Event e = (Event)elem.DataContext;
                var concurrentEvents = evtStack.Events.Where(e1 => ((e1.Start <= e.Start && e1.End > e.Start) ||
                                                                (e1.Start > e.Start && e1.Start < e.End))).OrderBy(ev => ev.Start);
              
                double marginTop = oneHourHeight * (e.Start.Hour + (e.Start.Minute / 60.0));
                double width = columnWidth;
                double marginLeft = 0;
                if (concurrentEvents.Count() > 0)
                {
                    width = columnWidth / (concurrentEvents.Count());
                    marginLeft = width * getIndex(e, concurrentEvents.ToList());
                }

                Canvas.SetLeft(elem, marginLeft);
                elem.Width = width;
                column.Children.Add(elem);
            }
            column.Children.Add(creationLine);
            column.Children.Add(CreationInfo);
        }

        private void CleanDrag()
        {
            if (draggedItem != null && draggedItem.IsMouseCaptured)
            {
                draggedItem.ReleaseMouseCapture();
                if (infoPopup.IsOpen)
                {
                    infoPopup.IsOpen = false;
                }
                Canvas.SetZIndex(draggedItem, 1);
                Event selected = (Event)(draggedItem.DataContext);
                //ScrollToEvent(selected);
                for (int i = 0; i < evtStack.Events.Count; i++)
                {
                    if (selected.Clashes(evtStack.Events[i]))
                    {
                        Canvas.SetZIndex(draggedItem, Canvas.GetZIndex(draggedItem) + 1);
                    }
                }
                
                draggedItem.Opacity = 1;
                if (selected.End.Day > selected.Start.Day && selected.End.Hour >= 12)
                {
                    selected.End = new DateTime(selected.End.Year, selected.End.Month, selected.End.Day, 12, 0, 0);
                    selected.updateDuration();
                }
                updateDuration(selected);
                IsDragging = false;
                IsResizing = false;
                SelectedEventItem = (Event)(draggedItem).DataContext;
                DrawEvents();
            }
        }

        private void Event_PreviewMouseUpOutsideCapturedElementEvent(object sender, RoutedEventArgs e)
        {
            
            CleanDrag();
        }

        private void Event_PreviewMouseDownOutsideCapturedElementEvent(object sender, RoutedEventArgs e)
        {
            
            CleanDrag();
            
        }

        private void Event_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CleanDrag();
            if (!(sender is Border))
            {
                return;
            }
            previousItem = draggedItem;
            Border selection = (Border)sender;
            draggedItem = selection;
            registeredSize = draggedItem.Height;
            OnEventPosition = Mouse.GetPosition(draggedItem);
            if (OnEventPosition.Y < 5 || OnEventPosition.Y > draggedItem.Height - 5)
            {
                IsResizing = true;
            } else
            {
                IsDragging = true; 
            }
            
            draggedItem.CaptureMouse();
            itemRelativePosition = e.GetPosition(draggedItem);
        }

        private void Event_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsDeleting)
            {
                if ((((Event)((Border)sender).DataContext) != null)){
                    evtStack.Events.Remove(((Event)((Border)sender).DataContext));
                    evtStack.updateEvts();
                    DBHandler.DeleteEvent(((Event)((Border)sender).DataContext));
                   
                    DrawnEventsList.Remove(DrawnEventsList.First<Border>((x) => (Border)sender == x));
                    Refresh();
                }
            }

            if (draggedItem != null)
            {
                CleanDrag();
                return;
            } 

            
            //if (!IsDragging)
            //{            
            //    return;
            //} 

        }

        private void updateDuration(Event evt)
        {
            double value = evt.Duration;
            int hour = (int)(value / 60);
            int min = (int)(value % 60);
            evt.ShowHour = evt.Start.Hour.ToString("00");
            evt.ShowMinutes = evt.Start.Minute.ToString("00");
            evt.ShowLengthHour = hour.ToString("00");
            evt.ShowLengthMinutes = min.ToString("00");
            
        }

        private void Event_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point OnEventPositionHover = Mouse.GetPosition((Border)sender);
            if (OnEventPositionHover.Y < 5 || OnEventPositionHover.Y > ((Border)sender).Height - 5)
            {
                ((Border)sender).Cursor = Cursors.SizeNS;
            } else if (!IsResizing)
            {
                ((Border)sender).Cursor = Cursors.Hand;
            }
            Point canvasRelativePosition = Mouse.GetPosition(column);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (draggedItem == null)
                {
                    return;
                }
                infoPopup.IsOpen = true;
                Point viewerLocationPoint = Mouse.GetPosition(SchedulerContainerGrid);
                Event actualDraggedEvent = (Event)draggedItem.DataContext;
                double length = SnapToGrid(canvasRelativePosition.Y - itemRelativePosition.Y);
                infoPopup.VerticalOffset = canvasRelativePosition.Y;
                int hours = (int)length / 50;
                int minutes = (int)((length - (hours * 50)) * 60/50);
                Canvas.SetZIndex(draggedItem, 100);
                if (hours < LowerLimit.Hour || LowerLimit.Hour == hours && minutes < LowerLimit.Minute) // < 0
                {
                    hours = LowerLimit.Hour;
                    minutes = LowerLimit.Minute;
                    length = 0;
                }
                else if (hours > 23 || hours == 23 && minutes > 59)
                {
                    hours = 23;
                    minutes = 59;
                    length = 1199.5; // 23 * 50(var) + ( ( 59 * 50(var) ) / 60 )  
                }

                if (viewerLocationPoint.Y < 30)
                {
                    sv.ScrollToVerticalOffset(sv.VerticalOffset - 1);

                }
                if (viewerLocationPoint.Y > SchedulerContainerGrid.RowDefinitions[1].ActualHeight - 30)
                {
                    sv.ScrollToVerticalOffset(sv.VerticalOffset + 1);
                }
                    
                if (IsDragging)
                {
                    
                    draggedItem.Opacity = 0.5;
                    DateTime compareStart = new DateTime(actualDraggedEvent.Start.Year, actualDraggedEvent.Start.Month, actualDraggedEvent.Start.Day, hours % 24, minutes % 60, 0);
                    DateTime endStart = compareStart.Add(new TimeSpan(actualDraggedEvent.LengthHour, actualDraggedEvent.LengthMinutes, 0));
                    if (endStart >= UpperLimit)
                    {
                        compareStart = UpperLimit.Subtract(new TimeSpan(actualDraggedEvent.LengthHour, actualDraggedEvent.LengthMinutes, 0));
                        endStart = UpperLimit;
                    }
                    debutLabel.Text = compareStart.Hour.ToString("00") + " : " + compareStart.Minute.ToString("00");
                    finLabel.Text = endStart.Hour.ToString("00") + " : " + endStart.Minute.ToString("00");
                    actualDraggedEvent.Start = compareStart;

                } else if (IsResizing)
                {
                    ((Border)sender).Cursor = Cursors.SizeNS;
                    double EndLength = SnapToGrid((canvasRelativePosition.Y - itemRelativePosition.Y) + registeredSize);
                    int endHour = ((int)EndLength / 50);
                    int endMinutes = (int)((EndLength - (endHour * 50)) * 60 / 50);
                    if (OnEventPosition.Y < 5)
                    {
                        DateTime compare = new DateTime(actualDraggedEvent.Start.Year, actualDraggedEvent.Start.Month, actualDraggedEvent.Start.Day, hours % 24, minutes % 60, 0);
                        if (compare > actualDraggedEvent.End.AddMinutes(-20))
                        {
                            actualDraggedEvent.Start = actualDraggedEvent.End.AddMinutes(-20);
                        } else
                        {
                            actualDraggedEvent.Start = compare;
                        }
                        actualDraggedEvent.updateDuration();
                        debutLabel.Text = actualDraggedEvent.Start.Hour.ToString("00") + " : " + actualDraggedEvent.Start.Minute.ToString("00");
                        finLabel.Text = actualDraggedEvent.End.Hour.ToString("00") + " : " + actualDraggedEvent.End.Minute.ToString("00");
                    } else if (OnEventPosition.Y > registeredSize - 5)
                    {

                        if (endMinutes < 0 || endHour < 0)
                        {
                            return;
                        }
                        if (endHour >= 24 + UpperLimit.Hour) // > 36
                        {
                            endHour = 24 + UpperLimit.Hour;
                            endMinutes = 0;
                        }

                        DateTime compare = new DateTime(actualDraggedEvent.End.Year, actualDraggedEvent.End.Month, actualDraggedEvent.End.Day, endHour % 24, endMinutes % 60, 0); 
                            
                        if (endHour >= 24)
                        {
                            if (compare.Day == actualDraggedEvent.Start.Day)
                            {
                                compare = new DateTime(actualDraggedEvent.Start.Year, actualDraggedEvent.Start.Month, actualDraggedEvent.Start.Day, endHour % 24, endMinutes % 60, 0).AddDays(1);
                            }
                                
                        } else
                        {
                            compare = new DateTime(actualDraggedEvent.Start.Year, actualDraggedEvent.Start.Month, actualDraggedEvent.Start.Day, endHour % 24, endMinutes % 60, 0);
                        }

                        TimeSpan time = new TimeSpan(0, 20, 0);
                        if (compare < actualDraggedEvent.Start.Add(time))
                        {
                            actualDraggedEvent.End = actualDraggedEvent.Start.Add(time);
                            actualDraggedEvent.updateDuration();
                        }
                        else
                        {
                            actualDraggedEvent.End = compare;
                            actualDraggedEvent.updateDuration();
                        }
                        debutLabel.Text = actualDraggedEvent.Start.Hour.ToString("00") + " : " + actualDraggedEvent.Start.Minute.ToString("00");
                        finLabel.Text = (actualDraggedEvent.End.Hour % 24).ToString("00") + " : " + (actualDraggedEvent.End.Minute % 60).ToString("00");
                    }
                    //}
                }              
            }   
        }

        private double SnapToGrid(double offset)
        {
            double topOffset = offset;     
            double ySnap = topOffset % GridSize;

            if (ySnap <= GridSize / 2.0)
            {
                ySnap *= -1;
            }
            else
            {
                ySnap = GridSize - ySnap;
            }

            ySnap += topOffset;
            return ySnap;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsDragging || IsResizing)
            {
                CleanDrag();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            ScrollInfo = DetermineTimeOfDayScroll(Mouse.GetPosition(column).Y);
            if (IsCreating)
            { 
                Point canvasRelativePosition = Mouse.GetPosition(column);
                double length = SnapToGrid(canvasRelativePosition.Y);
                int hours = (int)length / 50;
                int minutes = (int)((length - (hours * 50)) * 60 / 50);
                if (hours < LowerLimit.Hour || LowerLimit.Hour == hours && minutes < LowerLimit.Minute) // < 0
                {
                    hours = LowerLimit.Hour;
                    minutes = LowerLimit.Minute;
                    length = 0;
                }
                else if (hours > 23 || hours == 23 && minutes > 59)
                {
                    hours = 23;
                    minutes = 59;
                    length = 1199.5; // 23 * 50(var) + ( ( 59 * 50(var) ) / 60 )  
                }
                creationLine.Visibility = Visibility.Visible;
                CreationInfo.Visibility = Visibility.Visible;
                creationLine.X1 = 0;
                creationLine.X2 = 320;
                creationLine.Y1 = length;
                creationLine.Y2 = length;
                ((TextBlock)CreationInfo.Child).Text = hours.ToString("00") + ":" + minutes.ToString("00");
                
                CreationInfo.Margin = new Thickness(120, length, 0, 0);
            } else
            {
               if (creationLine.Visibility == Visibility.Visible || CreationInfo.Visibility == Visibility.Visible)
                {
                    creationLine.Visibility = Visibility.Collapsed;
                    CreationInfo.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ComboBoxSnapType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int value = ((ComboBox)sender).SelectedIndex;
            switch (value)
            {
                case 0:
                    GridSize = 50;
                    break;
                case 1:
                    GridSize = 25;
                    break;
                case 2:
                    GridSize = (double)500 / 60;
                    break;
                case 3:
                    GridSize = (double)250 / 60;
                    break;
            }
        }

        private void SnapToGridCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            GridSize = (double)50 / 60;
        }

        private void CreateModeTgBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (IsDeleting)
            {
                DeleteModeTgBtn.IsChecked = false;
            }
            IsCreating = true;

        }

        private void CreateModeTgBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            IsCreating = false;
        }

        private void DeleteModeTgBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (IsCreating)
            {
                CreateModeTgBtn.IsChecked = false;
            }
            IsDeleting = true;
        }

        private void DeleteModeTgBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            IsDeleting = false;
        }

        private void Column_MouseMove(object sender, MouseEventArgs e)
        {

           
        }

        private void Sv_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsCreating)
            {
                Band actualBand = DBHandler.getDefaultBand();
                double length = creationLine.Y1;
                int hours = (int)length / 50;
                int minutes = (int)((length - (hours * 50)) * 60 / 50);
                DateTime init = new DateTime(LowerLimit.Date.Year, LowerLimit.Date.Month, LowerLimit.Date.Day, hours, minutes, 0);
                Event newEvent = new Event
                {
                    Band = actualBand,
                    CurrentFormule = actualBand.Formules[0],
                    Start = init,
                    End = init.Add(new TimeSpan(1, 0, 0)),
                    Name = "Aucun Titre",
                    LocationName = null
                };
                newEvent.updateDuration();
                evtStack.AddEvent(newEvent);
                AddAndUpdate(newEvent);
                IsCreating = false;
            }
            Refresh();
            CreateModeTgBtn.IsChecked = false;
        }




        //System.Windows.Point ScrollMousePoint1 = new System.Windows.Point();
        //double HorizontalOff1 = 1; double VerticalOff1 = 1;
        //private void sv_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    e.Handled = true;
        //    ScrollMousePoint1 = e.GetPosition(sv);
        //    HorizontalOff1 = sv.HorizontalOffset;
        //    VerticalOff1 = sv.VerticalOffset;
        //    sv.CaptureMouse();
        //}

        //private void sv_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    if (sv.IsMouseCaptured)
        //    {
        //        sv.ScrollToHorizontalOffset(HorizontalOff1 + (ScrollMousePoint1.X - e.GetPosition(sv).X));
        //        sv.ScrollToVerticalOffset(VerticalOff1 + (ScrollMousePoint1.Y - e.GetPosition(sv).Y));
        //    }
        //}

        //private void sv_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    sv.ReleaseMouseCapture();
        //}

        //private void sv_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        //{
        //    sv.ScrollToHorizontalOffset(sv.HorizontalOffset + e.Delta);
        //    sv.ScrollToVerticalOffset(sv.VerticalOffset + e.Delta);
        //}

    }
}
