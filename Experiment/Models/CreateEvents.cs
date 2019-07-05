using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Experiment.Models
{
    public class CreateEvents
    {
        private ObservableCollection<Event> events;
        public CreateEvents()
        {
          
            //events = new ObservableCollection<Event>();
            /*
            Day fromDay = new Day
            {
                Date = new DateTime(2019, 01, 01)
            };
            Random random = new Random();
            Random colorRandom = new Random();
            for (int i = 0; i < 120; i++)
            {
                int num = random.Next(4, 7);
                fromDay.Date = fromDay.Date.AddDays(num);
                Day startDay = new Day
                {
                    Date = fromDay.Date
                };
                fromDay.Date = fromDay.Date.AddDays(random.Next(2));
                Day endDay = new Day
                {
                    Date = fromDay.Date
                };

                Event evt = new Event(startDay, endDay, "Hey new event made");
                evt.EmployerID = i;
                evt.ColorFill = new SolidColorBrush(Color.FromRgb(
                        (byte)colorRandom.Next(0, 255),
                        (byte)colorRandom.Next(0, 5),
                        (byte)colorRandom.Next(0, 5)));   
                events.Add(evt);
            }
            */
        }

        public ObservableCollection<Event> getEvents()
        {
            if (events != null)
            {
                return events;
            }
            return null;
        }

    }
}
