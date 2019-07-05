using Experiment.Models;
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
    class SelectedEventToRadioButtonConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count() > 0 && values[0] != null && values[1] != null)
            {
                if (((Event)values[0]).Equals((Event)values[1]))
                {
                    return true;
                }
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            Console.WriteLine(value);
            return null;
        }
    }
}
