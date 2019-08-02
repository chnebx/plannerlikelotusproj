using Experiment.Models;
using Experiment.Utilities;
using SQLite;
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
        public List<Event> eventsResults { get; set; }
        public CollectionViewSource evts = null;
        private List<string> queries;
        private List<string> queriesArgs;
        private string formulesQuery = "";
        private string employerQuery = "";
        private string locationQuery = "";
        private string titleQuery = "";
        private string commentQuery = "";

        public SearchFilterControl()
        {
            InitializeComponent();
            FilterMod = FilterModule.Instance;
            DataContext = this;
            queries = new List<string>();
            queriesArgs = new List<string>();
            //EventSetter = new List<Event>();
            evts = (CollectionViewSource)this.FindResource("filteredResults");
            formules = new ObservableCollection<Formule>();
            formules = new ObservableCollection<Formule>(formules.Concat(DBHandler.getFormules()));
            comboBoxFormules.SelectedIndex = 0;
            var delay = 50;
            Observable.FromEventPattern<EventArgs>(txtBoxFilterEmployer, "TextChanged")
                .Select(ea => ((TextBox)ea.Sender).Text)
                .DistinctUntilChanged()
                .Throttle(TimeSpan.FromMilliseconds(delay))
                .Subscribe(text => {
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        employerQuery = "EmployerID IN (Select Id FROM Employers WHERE FirstName LIKE ? OR LastName LIKE ?)";
                    } else
                    {
                        employerQuery = "";
                    }
                    Predicate<Event> employerFilter = new Predicate<Event>((x) => x.ActualEmployer != null && (x.ActualEmployer.FirstName.ToLower().StartsWith(txtBoxFilterEmployer.Text) || x.ActualEmployer.LastName.ToLower().StartsWith(txtBoxFilterEmployer.Text)));
                    this.Dispatcher.Invoke(new Action<Predicate<Event>, TextBox>((employer, element) => TextChangedHandler(employer, element)), new object[] { employerFilter, txtBoxFilterEmployer });
                });

            Observable.FromEventPattern<EventArgs>(comboBoxFormules, "SelectionChanged")
             .Select(ea => ((ComboBox)ea.Sender).SelectedValue)
             .DistinctUntilChanged()
             .Throttle(TimeSpan.FromMilliseconds(delay))
             .Subscribe(text =>
             {
                 Formule formule = (Formule)text;
                 if (formule.Id != 1)
                 {
                     formulesQuery = "FormuleID IN (Select Id FROM Formules WHERE Name LIKE ?)";
                 } else
                 {
                     formulesQuery = "";
                 }
                 Predicate<Event> formulesFilter = null;
                 formulesFilter = new Predicate<Event>((x) => x.CurrentFormule != null && x.CurrentFormule.Name == formule.Name);
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
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        locationQuery = "LocationID IN (Select Id From Locations WHERE TownName LIKE ?)";
                    }
                    else
                    {
                        locationQuery = "";
                    }
                    Predicate<Event> locationFilter = new Predicate<Event>((x) => x.LocationName != null && x.LocationName.TownName.ToLower().Contains(txtBoxFilterLocation.Text.ToLower()));
                    this.Dispatcher.Invoke(new Action(() => TextChangedHandler(locationFilter, txtBoxFilterLocation)));
                });

            Observable.FromEventPattern<EventArgs>(txtBoxFilterTitle, "TextChanged")
                .Select(ea => ((TextBox)ea.Sender).Text)
                .DistinctUntilChanged()
                .Throttle(TimeSpan.FromMilliseconds(delay))
                .Subscribe(text => {
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        titleQuery = "Name Like ?";
                    }
                    else
                    {
                        titleQuery = "";
                    }
                    Predicate<Event> titleFilter = new Predicate<Event>((x) => x.Name != null && x.Name.ToLower().Contains(txtBoxFilterTitle.Text.ToLower())); ;
                    this.Dispatcher.Invoke(new Action<Predicate<Event>, TextBox>((title, element) => TextChangedHandler(title, element)), new object[] { titleFilter, txtBoxFilterTitle });
                });

            Observable.FromEventPattern<EventArgs>(txtBoxFilterComment, "TextChanged")
                .Select(ea => ((TextBox)ea.Sender).Text)
                .DistinctUntilChanged()
                .Throttle(TimeSpan.FromMilliseconds(delay))
                .Subscribe(text => {
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        commentQuery = "Comment LIKE ?";
                    }
                    else
                    {
                        commentQuery = "";
                    }
                    Predicate<Event> commentFilter = new Predicate<Event>((x) => x.Comment != null && x.Comment.ToLower().Contains(txtBoxFilterComment.Text.ToLower()));
                    this.Dispatcher.Invoke(new Action<Predicate<Event>, TextBox>((comment, element) => TextChangedHandler(comment, element)), new object[] { commentFilter, txtBoxFilterComment });
                });
        }

        public void TextChangedHandler(Predicate<Event> filterFunc, object element)
        {
            //string employerQuery = txtBoxFilterEmployer.Text;
            //string titleQuery = txtBoxFilterTitle.Text;
            //string locationQuery = txtBoxFilterLocation.Text;
            //string commentQuery = txtBoxFilterComment.Text;
            queries.Clear();
            queriesArgs.Clear();
            if (!FilterMod.ContainsFilter(filterFunc))
            {
                FilterMod.AddFilter(filterFunc);
            }

            if (element is ComboBox)
            {
                //formulesQuery = ((Formule)((ComboBox)element).SelectedValue).Name;
          
                if (((ComboBox)element).SelectedIndex == 0)
                {
                    FilterMod.RemoveFilter(filterFunc);
                    //formulesQuery = "";
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
            FillQueries();
            if (FilterMod.IsFilterActive)
            {
                //eventsResults = DBHandler.QueryDB(employerQuery, formulesQuery, locationQuery, titleQuery, commentQuery);
                eventsResults = DBHandler.QueryDB2(queries, queriesArgs);
            }
            else
            {
                eventsResults = null;
            };
            FilterResultsListBox.ItemsSource = eventsResults;
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

        private void FillQueries()
        {
            if (employerQuery != "")
            {
                queries.Add(employerQuery);
                queriesArgs.Add(txtBoxFilterEmployer.Text + "%");
                queriesArgs.Add(txtBoxFilterEmployer.Text + "%");
            }
            if (locationQuery != "")
            {
                queries.Add(locationQuery);
                queriesArgs.Add(txtBoxFilterLocation.Text + "%");
            }
            if (titleQuery != "")
            {
                queries.Add(titleQuery);
                queriesArgs.Add(txtBoxFilterTitle.Text + "%");
            }
            if (commentQuery != "")
            {
                queries.Add(commentQuery);
                queriesArgs.Add("%" + txtBoxFilterComment.Text + "%");
            }
            if (formulesQuery != "")
            {
                queries.Add(formulesQuery);
                queriesArgs.Add(((Formule)comboBoxFormules.SelectedValue).Name);
            }
        }
    }
}
