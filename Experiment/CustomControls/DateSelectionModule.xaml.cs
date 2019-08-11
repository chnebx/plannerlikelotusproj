using Experiment.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

namespace Experiment.CustomControls
{
    /// <summary>
    /// Logique d'interaction pour DateSelectionModule.xaml
    /// </summary>
    public partial class DateSelectionModule : UserControl, INotifyPropertyChanged
    {
        int[] Days = new int[31];
        List<string> monthNames;
        List<int> years = new List<int>();
        List<int> repeatYearsCount = new List<int>();
        bool initialized = false;
        Event actualEvt = null;
        private bool _isValid = false;
        bool increaseYearBy1 = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                _isValid = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsValid"));
            }
        }

        public DateSelectionModule(Event evt, bool increaseYear)
        {
            actualEvt = evt;
            increaseYearBy1 = increaseYear;
            this.DataContext = this;
            InitializeComponent();
            initializeLists();
            LoadMonthsCombos();
            LoadYearCombo();
            initalizeDaysArray();
            LoadDaysCombo();
            InitializeRepeatComboList();
            LoadRepeatYearsCombo();
            initialized = true;
        }

        public DateSelectionModule(DateTime evtTime, bool increaseYear)
        {
            actualEvt = new Event {
                Start = new DateTime(evtTime.Year, evtTime.Month, evtTime.Day, 0, 0, 0)
            };
            increaseYearBy1 = increaseYear;
            this.DataContext = this;
            InitializeComponent();
            initializeLists();
            LoadMonthsCombos();
            LoadYearCombo();
            initalizeDaysArray();
            LoadDaysCombo();
            InitializeRepeatComboList();
            LoadRepeatYearsCombo();
            initialized = true;
        }

        public void initializeLists()
        {
            monthNames = DateTimeFormatInfo.CurrentInfo.MonthNames.ToList<string>();
            monthNames.RemoveAt(monthNames.Count - 1);
            years = Enumerable.Range(DateTime.Now.Year, 100).ToList();
        }
        public void initalizeDaysArray()
        {
            int month = DateTimeFormatInfo.CurrentInfo.MonthNames.ToList().IndexOf(cmbMonths.SelectedValue.ToString()) + 1;
            Days = new int[DateTime.DaysInMonth(Convert.ToInt32(cmbYear.SelectedValue), month)];
            for (int i = 0; i < Days.Count(); i++)
            {
                Days[i] = i + 1;
            }
        }

        public void InitializeRepeatComboList()
        {
            for (int i = 1; i <= 10; i++)
            {
                repeatYearsCount.Add(i);
            }
        }

        public void LoadRepeatYearsCombo()
        {
            cmbRepeatCount.ItemsSource = repeatYearsCount;
            cmbRepeatCount.SelectedIndex = 0;
        }

        public void LoadDaysCombo()
        {
            int previousValue = -1;
            if (initialized)
            {
                previousValue = (int)cmbDays.SelectedValue;
            }
            cmbDays.ItemsSource = Days;
            
            if (initialized && previousValue - 1 < Days.Count())
            {
                cmbDays.SelectedValue = previousValue;

            } else 
            {
                if (actualEvt.Start.Day <= Days.Count())
                {
                    cmbDays.SelectedValue = actualEvt.Start.Day;
                } else
                {
                    cmbDays.SelectedValue = 1;
                }
                    
            }
            if (initialized)
            {
                IsValid = CheckValidity();
            }
        }

        private bool CheckValidity()
        {
            if (initialized)
            {
                DateTime timeCheck = GetDate();
                if (timeCheck < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0))
                {
                    WarningLabel.Text = "Date passée";
                    return false;
                }
            }
            WarningLabel.Text = "";
            return true;
        }

        public DateTime GetDate()
        {
            int day = (int)cmbDays.SelectedValue;
            int month = (int)cmbMonths.SelectedIndex + 1;
            int year = (int)cmbYear.SelectedValue;
            return new DateTime(year, month, day, 0, 0, 0);
        }

        public void LoadYearCombo()
        {
            cmbYear.ItemsSource = years;
            if (increaseYearBy1)
            {
                cmbYear.SelectedValue = actualEvt.Start.Year + 1;
            }
            else
            {
                cmbYear.SelectedValue = actualEvt.Start.Year;
            }
            
        }

        public void LoadMonthsCombos()
        {
            cmbMonths.ItemsSource = monthNames;
            cmbMonths.SelectedValue = DateTimeFormatInfo.CurrentInfo.GetMonthName(actualEvt.Start.Month);
        }

        private void cmdMonths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbMonths.SelectedValue != null)
            {
                initalizeDaysArray();
                LoadDaysCombo();
            }
        }

        private void cmbYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbYear.SelectedValue != null)
            {
                initalizeDaysArray();
                LoadDaysCombo();
            }
        }

        private void CmbDays_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsValid = CheckValidity();
        }
    }


}

