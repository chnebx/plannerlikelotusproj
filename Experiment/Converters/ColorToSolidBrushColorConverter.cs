using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WpfColor = System.Windows.Media;

namespace Experiment.Converters
{
    class ColorToSolidBrushColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Binding.DoNothing;
            }

            if (value is WpfColor.SolidColorBrush)
            {
                WpfColor.SolidColorBrush color = (WpfColor.SolidColorBrush)value;
                return color.Color;
            }

            Type type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Binding.DoNothing;
            }

            if (value is WpfColor.Color)
            {
                WpfColor.Color color = (WpfColor.Color)value;
                return new WpfColor.SolidColorBrush(color);
            }

            Type type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]");
        }
    }
}
