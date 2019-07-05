using Experiment.Utilities;
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
        public Calendar()
        {
            InitializeComponent();
            DBHandler.DbInit();
            Planner actualPlanner = new Planner();
            MonthPlanner actualMonthPlanner = new MonthPlanner();
            actualMonthPlanner.BuildMonthPlanner(DateTime.Now);
            actualPlanner.BuildPlanner(DateTime.Now);
            DataContext = this;
            monthModeTab.Content = actualMonthPlanner;
            yearModeTab.Content = actualPlanner;
            //Planner actualPlanner = Planner.Instance;
            //yearModeTab.Content = actualPlanner;
           
        }
    }
}
