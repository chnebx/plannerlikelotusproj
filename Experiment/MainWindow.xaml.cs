using Experiment.Models;
using Experiment.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Experiment
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        //private ObservableCollection<EventStack> events = new ObservableCollection<EventStack>();
        public MainWindow()
        {
            InitializeComponent();
            /*
            for (int i = -10; i < 10; i++)
            {
                comboYear.Items.Add(DateTime.Today.AddYears(i).Year);
            }
            comboYear.SelectedItem = DateTime.Today.Year;

            comboYear.SelectionChanged += (o, e) => refreshCalendar();
            events = DBHandler.getEvents();

            actualPlanner.BuildPlanner(DateTime.Now);
            */
        }

        private void refreshCalendar()
        {
            /*
            if (comboYear.SelectedItem == null) return;
            int year = (int)comboYear.SelectedItem;
            
            DateTime targetDate = new DateTime(year, 1, 1);
            actualPlanner.currentYear = year;
            actualPlanner.BuildPlanner(targetDate);
            */
        }
    }
}
