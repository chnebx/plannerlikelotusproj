﻿using Experiment.Utilities;
using System;
using System.Collections.Generic;
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
    /// Logique d'interaction pour Calendar.xaml
    /// </summary>
    public partial class Calendar : UserControl
    {
        public StateManager CalendarState;
        public Planner actualPlanner;
        public MonthPlanner actualMonthPlanner;
        public Calendar()
        {
            InitializeComponent();
            DBHandler.DbInit();
            CalendarState = new StateManager();
            actualPlanner = new Planner();
            actualMonthPlanner = new MonthPlanner();
            SearchFilterControl actualSearchFilter = new SearchFilterControl();
            actualMonthPlanner.CalendarState = CalendarState;
            actualPlanner.CalendarState = CalendarState;
            actualMonthPlanner.BuildMonthPlanner(DateTime.Now);
            actualPlanner.BuildPlanner(DateTime.Now);

            DataContext = this;
          
            monthModeTab.Content = actualMonthPlanner;
            yearModeTab.Content = actualPlanner;
            SearchModeTab.Content = actualSearchFilter;
            //Planner actualPlanner = Planner.Instance;
            //yearModeTab.Content = actualPlanner;
           
        }

        private void MonthModeTab_MouseUp(object sender, MouseButtonEventArgs e)
        {
            actualMonthPlanner.RefreshEvents();
        }

        private void YearModeTab_MouseUp(object sender, MouseButtonEventArgs e)
        {
            actualPlanner.RefreshEvents();
        }

        //private void BtnUndo_Click(object sender, RoutedEventArgs e)
        //{
        //CalendarState.Undo();
        //if (CalendarModule.SelectedContent is Planner)
        //{
        //    actualPlanner.RefreshEvents();
        //} else if (CalendarModule.SelectedContent is MonthPlanner)
        //{
        //    actualMonthPlanner.RefreshEvents();
        //}
        //}
    }
}
