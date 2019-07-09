using Experiment.Models;
using Experiment.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Experiment
{
    /// <summary>
    /// Logique d'interaction pour SearchFilterControl.xaml
    /// </summary>
    public partial class SearchFilterControl : UserControl
    {
        public Predicate<Event> previousFormuleFilter = null;
        public FilterModule FilterMod { get; set; }
        public ObservableCollection<Formule> formules { get; set; }

        public SearchFilterControl()
        {
            InitializeComponent();
            FilterMod = FilterModule.Instance;
            DataContext = this;
            formules = new ObservableCollection<Formule>();
            formules.Add(new Formule("Non définie"));
            formules = new ObservableCollection<Formule>(formules.Concat(DBHandler.getFormules()));
            
            var delay = 50;
            Observable.FromEventPattern<EventArgs>(txtBoxFilterEmployer, "TextChanged")
                .Select(ea => ((TextBox)ea.Sender).Text)
                .DistinctUntilChanged()
                .Throttle(TimeSpan.FromMilliseconds(delay))
                .Subscribe(text => {

                    Predicate<Event> employerFilter = new Predicate<Event>((x) => x.ActualEmployer.FirstName.ToLower().StartsWith(txtBoxFilterEmployer.Text) || x.ActualEmployer.LastName.ToLower().StartsWith(txtBoxFilterEmployer.Text));
                    this.Dispatcher.Invoke(new Action<Predicate<Event>, TextBox>((employer, element) => TextChangedHandler(employer, element)), new object[] { employerFilter, txtBoxFilterEmployer });
                });

            Observable.FromEventPattern<EventArgs>(comboBoxFormules, "SelectionChanged")
             .Select(ea => ((ComboBox)ea.Sender).SelectedValue)
             .DistinctUntilChanged()
             .Throttle(TimeSpan.FromMilliseconds(delay))
             .Subscribe(text =>
             {
                 Formule formule = (Formule)text;
                 Predicate<Event> formulesFilter = null;
                 formulesFilter = new Predicate<Event>((x) => x.CurrentFormule.Name == formule.Name);
                 if (previousFormuleFilter != null)
                 {
                     FilterModule.Instance.RemoveFilter(previousFormuleFilter);
                 }
                 this.Dispatcher.Invoke(new Action(() => TextChangedHandler(formulesFilter, comboBoxFormules)));
                 previousFormuleFilter = formulesFilter;
             });

            Observable.FromEventPattern<EventArgs>(txtBoxFilterLocation, "TextChanged")
                .Select(ea => ((TextBox)ea.Sender).Text)
                .DistinctUntilChanged()
                .Throttle(TimeSpan.FromMilliseconds(delay))
                .Subscribe(text => {
                    Predicate<Event> locationFilter = new Predicate<Event>((x) => x.LocationName.TownName.ToLower().Contains(txtBoxFilterLocation.Text.ToLower()));
                    this.Dispatcher.Invoke(new Action(() => TextChangedHandler(locationFilter, txtBoxFilterLocation)));
                });

            Observable.FromEventPattern<EventArgs>(txtBoxFilterTitle, "TextChanged")
                .Select(ea => ((TextBox)ea.Sender).Text)
                .DistinctUntilChanged()
                .Throttle(TimeSpan.FromMilliseconds(delay))
                .Subscribe(text => {
                    Predicate<Event> titleFilter = new Predicate<Event>((x) => x.Name.ToLower().Contains(txtBoxFilterTitle.Text.ToLower())); ;
                    this.Dispatcher.Invoke(new Action<Predicate<Event>, TextBox>((title, element) => TextChangedHandler(title, element)), new object[] { titleFilter, txtBoxFilterTitle });
                });

            Observable.FromEventPattern<EventArgs>(txtBoxFilterComment, "TextChanged")
                .Select(ea => ((TextBox)ea.Sender).Text)
                .DistinctUntilChanged()
                .Throttle(TimeSpan.FromMilliseconds(delay))
                .Subscribe(text => {
                    Predicate<Event> commentFilter = new Predicate<Event>((x) => x.Comment.ToLower().Contains(txtBoxFilterComment.Text.ToLower()));
                    this.Dispatcher.Invoke(new Action<Predicate<Event>, TextBox>((comment, element) => TextChangedHandler(comment, element)), new object[] { commentFilter, txtBoxFilterComment });
                });
        }

        public void TextChangedHandler(Predicate<Event> filterFunc, object element)
        {
            if (!FilterMod.ContainsFilter(filterFunc))
            {
                FilterMod.AddFilter(filterFunc);
            }

            if (element is ComboBox)
            {
                if (((ComboBox)element).SelectedIndex == -1)
                {
                    FilterMod.RemoveFilter(filterFunc);
                }
            }
            else
            {
                if (String.IsNullOrEmpty(((TextBox)element).Text))
                {
                    FilterMod.RemoveFilter(filterFunc);
                }
            }

            FilterMod.RefreshFilter();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            txtBoxFilterComment.Clear();
            txtBoxFilterEmployer.Clear();
            txtBoxFilterLength.Clear();
            txtBoxFilterLocation.Clear();
            txtBoxFilterTitle.Clear();
            if (comboBoxFormules.SelectedIndex != 0)
            {
                comboBoxFormules.SelectedIndex = 0;
            }
        }
    }
}
