using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Experiment.Models
{
    public class SubEvent : INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler PropertyChanged;
        /*
        private Day SubStartingDay;
        private int len;
        private int srow;
        private int scolumn;
        Event parent;

        public new Day StartingDay
        {
            get { return SubStartingDay;  }
            set
            {
                SubStartingDay = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SubStartingDay"));
            }
        }

        public new int Length
        {
            get { return len; }
            set
            {
                len = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SubLength"));
            }
        }

        public new int Row
        {
            get
            {
                return SubStartingDay.Row + parent.stackRow;
            }
            set
            {
                srow = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SubRow"));
            }
        }

        public new int Column
        {
            get { return SubStartingDay.Column; }
            set
            {
                scolumn = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SubColumn"));
            }
        }

        public Event parentElement
        {
            get { return parent; }
            set
            {
                parent = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SubColumn"));
            }
        }

        public override string ToString()
        {

            return "Event Starting Day : " + StartingDay.Date.ToLongDateString() + " , Event length : " + Length + ", parent: " + parent.EmployerID;
        }

        public SolidColorBrush fill
        {
            get { return parent.ColorFill;  }
        }
        */
    }
}
