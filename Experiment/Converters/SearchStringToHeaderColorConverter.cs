using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Experiment.Converters
{
    class SearchStringToHeaderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                if ((int)value == 0)
                {
                    return "#FF8484AA";
                } else
                {
                    return "#FFe7bf1d";
                }
            }
            var selected = (string)value;
            if (!String.IsNullOrEmpty(selected))
            {
                return "#FFe7bf1d";
            } else
            {
                return "#FF8484AA";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
