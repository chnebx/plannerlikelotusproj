using Experiment.Models;
using Experiment.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
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
    /// Logique d'interaction pour DefineEmployerDialogWindow.xaml
    /// </summary>
    public partial class DefineEmployerDialogWindow : Window, INotifyPropertyChanged
    {
        private bool _createEmployerPanelActive = false;
        public Employer SelectedEmployer { get; set; }
        public ObservableCollection<Employer> employers { get; set; }
        public ICollectionView _employercvs { get; set; }
        public ICollectionView _CvsFromEventsList { get; set; }
        public DefineEmployerDialogWindow(Employer CurrentExisitingEmployer = null)
        {
            InitializeComponent();
            if (CurrentExisitingEmployer != null)
            {
                SelectedEmployer = CurrentExisitingEmployer;
            }
            DataContext = this;
            employers = DBHandler.getEmployers();
            EmployersList.ItemsSource = employers;
            _CvsFromEventsList = CollectionViewSource.GetDefaultView(EmployersList.ItemsSource);
            _CvsFromEventsList.Filter = EmployerLastNameFilter;
            _CvsFromEventsList.SortDescriptions.Add(new SortDescription("LastName", ListSortDirection.Ascending));
            FilterEmployerTypeCombo.SelectedIndex = 0;
            var delay = 30;
            Observable.FromEventPattern<EventArgs>(txtBoxFilterEmployerLastName, "TextChanged")
                .Select(ea => ((TextBox)ea.Sender).Text)
                .DistinctUntilChanged()
                .Throttle(TimeSpan.FromMilliseconds(delay))
                .Subscribe(text => {
                    //Predicate<Employer> filterLastName = new Predicate<Employer>((x) => x.LastName.Contains(txtBoxFilterEmployerLastName.Text));
                    this.Dispatcher.Invoke(new Action(() => handleFilters())); ;
                });
        }

        public void handleFilters(string filterType = null)
        {
            _CvsFromEventsList.Refresh();
        }

        private bool EmployerLastNameFilter(object item)
        {
            if (String.IsNullOrEmpty(txtBoxFilterEmployerLastName.Text))
                return true;
            else
                return ((Employer)item).LastName.ToLower().StartsWith(txtBoxFilterEmployerLastName.Text.ToLower());
        }

        private bool EmployerFirstNameFilter(object item)
        {
            if (String.IsNullOrEmpty(txtBoxFilterEmployerLastName.Text))
                return true;
            else
                return ((Employer)item).FirstName.ToLower().StartsWith(txtBoxFilterEmployerLastName.Text.ToLower());
        }

        private bool EmployerPhoneNumberFilter(object item)
        {
            if (String.IsNullOrEmpty(txtBoxFilterEmployerLastName.Text))
                return true;
            else
                return ((Employer)item).PhoneNumber.ToLower().Contains(txtBoxFilterEmployerLastName.Text.ToLower());
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public bool CreateEmployerPanelActive
        {
            get
            {
                return _createEmployerPanelActive;
            }
            set
            {
                _createEmployerPanelActive = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CreateEmployerPanelActive"));
            }
        }

        private void AddEmployerBtn_Click(object sender, RoutedEventArgs e)
        {
            //NewEmployerGrid.Visibility = Visibility.Visible;
            toggleEmployerCreationPanel();

        }

        private void toggleEmployerCreationPanel()
        {
            if (!CreateEmployerPanelActive)
            {
                AddEmployerBtn.Content = "Annuler";
                CreateEmployerPanelActive = true;
            } else
            {
                AddEmployerBtn.Content = "Ajouter";
                CreateEmployerPanelActive = false;
            }
           
        }
        
        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CreateEmployerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtEmployerName.Text) && 
                !String.IsNullOrEmpty(txtEmployerPrenom.Text) &&
                !String.IsNullOrEmpty(txtEmployerPhone.Text))
            {
                Employer newEmployer = new Employer(txtEmployerPrenom.Text, txtEmployerName.Text, txtEmployerPhone.Text);
                DBHandler.AddEmployer(newEmployer);
                txtEmployerName.Clear();
                txtEmployerPrenom.Clear();
                txtEmployerPhone.Clear();
                toggleEmployerCreationPanel();
                EmployersList.ItemsSource = DBHandler.getEmployers();
                EmployersList.SelectedItem = newEmployer;
            }
        }

        private void FilterEmployerTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            ComboBox combo = (ComboBox)sender;
            switch (combo.SelectedIndex)
            {
                case (0):
                    _CvsFromEventsList.Filter = EmployerLastNameFilter;
                    break;
                case (1):
                    _CvsFromEventsList.Filter = EmployerFirstNameFilter;
                    break;
                case (2):
                    _CvsFromEventsList.Filter = EmployerPhoneNumberFilter;
                    break;
            }

        }
    }
}
