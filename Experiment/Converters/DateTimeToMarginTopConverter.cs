using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Experiment.Converters
{
    class DateTimeToMarginTopConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is DateTime)
            {
                DateTime start = (DateTime)value;
                double marginTop = 50 * (start.Hour + (start.Minute / 60.0));
                return new Thickness(0, marginTop, 0 , 0);
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
