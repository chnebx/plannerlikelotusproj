using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.ObjectModel;

namespace Experiment.Models
{
    public class Day : INotifyPropertyChanged
    {
        private DateTime _date;
        private bool isToday;
        private bool enabled;
        private string _daytitle;
        private int row;
        private int rowspan;
        private int column;
        private int _monthRow;
        private int _monthColumn;
        private int dayNumber;
        private bool _isWE;
        private bool _editMode = false;
        private int offset = 0;
        private string dayNum;
        private string _dayFullName;
        public enum _dayType { Enabled, IsWE, IsToday};

        public _dayType DayType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Day()
        {
           
        }

        public Day(DateTime date)
        {
            _date = date;
            _daytitle = Date.ToLongDateString().Substring(0, Date.ToLongDateString().Count() - 5);
            string[] dateParts = _daytitle.Split(null);
            string firstPart = dateParts[0].ToString();
            string lastPart = dateParts[2].ToString();
            firstPart = char.ToUpper(firstPart[0]) + firstPart.Substring(1);
            lastPart = char.ToUpper(lastPart[0]) + lastPart.Substring(1);
            isToday = date.CompareTo(DateTime.Today) == 0;
            _dayFullName = String.Format("{0} {1} {2} {3}", firstPart, dateParts[1], lastPart, date.Year);
        }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Date"));
            }
        }

        public bool IsToday
        {
            get { return isToday; }
            set
            {
                isToday = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsToday"));
            }
        }

        public int Length
        {
            get { return 1; }
            set
            {
                Length = value;

            }
        }

        public string DayNumber
        {
            get { return _date.Day.ToString(); }

        }

        public string DayNum
        {
            get
            {
                string day = _date.Day.ToString();
                return day;
            }
            set
            {
                dayNum = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DayNum"));
            }
        }

        public int RowSpan
        {
            get { return 2; }
            set
            {
                rowspan = value;

            }
        }

        public string DayTitle
        {
            get
            {
                /*
                string val = Date.ToLongDateString();
                return val.Substring(0, val.Count() - 5);
                */
                return _daytitle;
            }
        }

        public string DayFullName
        {
            get
            {
                return _dayFullName;
            }
        }

        public bool EditMode
        {
            get
            {
                return _editMode;
            }
            set
            {
                _editMode = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("EditMode"));
            }
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Enabled"));
            }
        }

        public static int CalculateYearRow(DateTime current)
        {
            return current.Month - 1;
        } 

        public static int CalculateYearColumn(DateTime current)
        {
            DateTime firstDay = new DateTime(current.Year, current.Month, 1);
            int offset = ((int)firstDay.DayOfWeek == 0) ? 6 : (int)firstDay.DayOfWeek - 1;
            return offset + current.Day - 1;
        }

        public static int CalculateMonthRow(DateTime current)
        {
            DateTime firstDay = new DateTime(current.Year, current.Month, 1);
            int offset = ((int)firstDay.DayOfWeek == 0) ? 6 : (int)firstDay.DayOfWeek - 1;
            return (offset + current.Day - 1) / 7; ;
        }

        public static int CalculateMonthColumn(DateTime current)
        {
            return ((int)current.DayOfWeek == 0) ? 6 : (int)current.DayOfWeek - 1;
        }

        public int Row
        {
            get {
                //return (_date.Month - 1) * 2;
                return _date.Month - 1;
            }
            set
            {
                row = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Row"));
            }
        }

        public int MonthRow
        {
            get
            {
                DateTime firstDay = new DateTime(_date.Year, _date.Month, 1);
                offset = ((int)firstDay.DayOfWeek == 0) ? 6 : (int)firstDay.DayOfWeek - 1;
                _monthRow = (offset + _date.Day - 1) / 7;
                return _monthRow;
            }
        }

        public int MonthColumn
        {
            get
            {
                //DateTime firstDay = new DateTime(_date.Year, _date.Month, 1);
                //offset = ((int)firstDay.DayOfWeek == 0) ? 6 : (int)firstDay.DayOfWeek - 1;
                //_monthColumn = (offset + _date.Day - 1) % 8;
                _monthColumn = ((int)_date.DayOfWeek == 0) ? 6 : (int)_date.DayOfWeek - 1;
                return _monthColumn;
            }
        }
        
        public int Column
        {
            get {
                DateTime firstDay = new DateTime(_date.Year, _date.Month, 1);
                offset = ((int)firstDay.DayOfWeek == 0) ? 6 : (int)firstDay.DayOfWeek - 1;
                return offset + _date.Day - 1;
            }
            set
            {
                column = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Column"));
            }
        }

        public bool IsWE
        {
            get { return weUtility(); }
            
        }

        private bool weUtility()
        {
            bool _isWE = false;
            if ((int)_date.DayOfWeek == 0 || (int)_date.DayOfWeek == 6)
            {
                _isWE = true;

            }
            return _isWE;
        }

       
    }
}
