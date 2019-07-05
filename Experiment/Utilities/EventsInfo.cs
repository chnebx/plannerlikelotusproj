using Experiment.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Utilities
{
    public class EventsInfo : INotifyPropertyChanged
    {
        private EventStack _upcomingEvent;
        private string _header;

        public EventsInfo()
        {

        }

        public EventStack UpcomingEvent
        {
            get
            {
                return _upcomingEvent;
            }
            set
            {
                _upcomingEvent = value;
                if (_upcomingEvent != null)
                {
                    if (DateTime.Compare(_upcomingEvent.Current.Date, DateTime.Now.Date) > 0)
                    {
                        Header = _upcomingEvent.Current.DayTitle;
                    }
                    else
                    {
                        Header = "Aujourd'hui";
                    }
                }
                
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("UpcomingEvent"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Header"));
            }
        }

        public string Header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Header"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
