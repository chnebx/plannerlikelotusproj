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
    /// Logique d'interaction pour DefineLocationDialogWindow.xaml
    /// </summary>
    public partial class DefineLocationDialogWindow : Window, INotifyPropertyChanged
    {
        private bool _createLocationsPanelActive = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public Location SelectedLocation { get; set; }
        public ObservableCollection<Location> locations { get; set; }
        public ICollectionView _locationcvs { get; set; }
        public ICollectionView _CvsFromLocationsList { get; set; }

        public DefineLocationDialogWindow(Location currentLocation)
        {
            InitializeComponent();
            if (currentLocation != null)
            {
                SelectedLocation = currentLocation;
            }
            DataContext = this;
            locations = new ObservableCollection<Location>(DBHandler.getLocations().OrderBy((x) => x.TownName));
            LocationsList.ItemsSource = locations;
            comboLocationPlaceType.ItemsSource = Location.PlaceTypes;
            comboBoxFilterLocationType.ItemsSource = Location.PlaceTypes;
            comboLocationPlaceType.SelectedIndex = 0;
            _CvsFromLocationsList = CollectionViewSource.GetDefaultView(LocationsList.ItemsSource);
            _CvsFromLocationsList.Filter = LocationNameFilter;
            _CvsFromLocationsList.SortDescriptions.Add(new SortDescription("TownName", ListSortDirection.Ascending));
            FilterLocationTypeCombo.SelectedIndex = 0;
            var delay = 30;
            Observable.FromEventPattern<EventArgs>(txtBoxFilterLocationName, "TextChanged")
                .Select(ea => ((TextBox)ea.Sender).Text)
                .DistinctUntilChanged()
                .Throttle(TimeSpan.FromMilliseconds(delay))
                .Subscribe(text => {
                    this.Dispatcher.Invoke(new Action(() => handleFilters())); ;
                });

            Observable.FromEventPattern<EventArgs>(comboBoxFilterLocationType, "SelectionChanged")
                .Select(ea => ((ComboBox)ea.Sender).SelectedItem)
                .DistinctUntilChanged()
                .Throttle(TimeSpan.FromMilliseconds(delay))
                .Subscribe(text => {
                    this.Dispatcher.Invoke(new Action(() => handleFilters())); ;
                });
        }

        public bool CreateLocationsPanelActive
        {
            get
            {
                return _createLocationsPanelActive;
            }
            set
            {
                _createLocationsPanelActive = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("CreateLocationsPanelActive"));
            }
        }

        public void handleFilters(string filterType = null)
        {
            _CvsFromLocationsList.Refresh();
        }

        private void toggleLocationCreationPanel()
        {
            if (!CreateLocationsPanelActive)
            {
                AddLocationBtn.Content = "Annuler";
                CreateLocationsPanelActive = true;
            }
            else
            {
                AddLocationBtn.Content = "Ajouter";
                CreateLocationsPanelActive = false;
            }

        }
        
        private bool LocationNameFilter(object item)
        {
            if (String.IsNullOrEmpty(txtBoxFilterLocationName.Text))
                return true;
            else
                return ((Location)item).TownName.ToLower().Contains(txtBoxFilterLocationName.Text.ToLower());
        }

        private bool LocationTypeFilter(object item)
        {
            if (comboBoxFilterLocationType.SelectedIndex == -1)
                return true;
            else
                return ((Location)item).PlaceType.Contains(comboBoxFilterLocationType.SelectedItem.ToString());
        }

        private bool LocationAddressFilter(object item)
        {
            if (String.IsNullOrEmpty(txtBoxFilterLocationName.Text))
                return true;
            else
                return ((Location)item).Address.ToLower().Contains(txtBoxFilterLocationName.Text.ToLower());
        }


        private void FilterLocationTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            switch (combo.SelectedIndex)
            {
                case (0):
                    _CvsFromLocationsList.Filter = LocationNameFilter;
                    break;
                case (1):
                    _CvsFromLocationsList.Filter = LocationTypeFilter;
                    break;
                case (2):
                    _CvsFromLocationsList.Filter = LocationAddressFilter;
                    break;
            }
        }

        private void AddLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            toggleLocationCreationPanel();
        }

        private void CreateLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtLocationTownName.Text) &&
                !String.IsNullOrEmpty(txtLocationAddress.Text))
            {
                Location newLocation = new Location {
                    TownName = txtLocationTownName.Text,
                    Address = txtLocationAddress.Text
                };
                newLocation.PlaceType = comboLocationPlaceType.SelectedItem.ToString();
                DBHandler.AddLocation(newLocation);
                txtLocationTownName.Clear();
                txtLocationAddress.Clear();
                comboLocationPlaceType.SelectedIndex = 0;
                toggleLocationCreationPanel();
                LocationsList.ItemsSource = DBHandler.getLocations();
                LocationsList.SelectedItem = newLocation;
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
    }
}
