using Experiment.CustomControls;
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
        public ObservableCollection<DateSelectionModule> modules { get; set; }
        public DuplicateEventWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            modules = new ObservableCollection<DateSelectionModule>();
            modules.Add(new DateSelectionModule());
        }

        private void BtnAddDuplicateDate_Click(object sender, RoutedEventArgs e)
        {
            modules.Add(new DateSelectionModule());
        }

        private void BtnRemoveDuplicateDate_Click(object sender, RoutedEventArgs e)
        {
            modules.RemoveAt(duplicateDateModulesList.SelectedIndex);
        }
    }
}
