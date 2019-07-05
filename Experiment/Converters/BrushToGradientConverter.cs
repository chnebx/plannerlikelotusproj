using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;


namespace Experiment.Converters
{

    [ValueConversion(typeof(System.Windows.Media.SolidColorBrush), typeof(LinearGradientBrush))]
    class BrushToGradientConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var brush = new LinearGradientBrush();
            //var color = ((SolidColorBrush)value).ToString();
            var color = ((SolidColorBrush)value).Color;
            var secondColor = ChangeColorBrightness(color, 0.3f);
            var thirdColor = ChangeColorBrightness(color, 0.4f);
            
           
            brush.StartPoint = new Point(0.5, 0);
            brush.EndPoint = new Point(0.5, 1);

            brush.GradientStops.Add(new GradientStop(color, 0));
            brush.GradientStops.Add(new GradientStop(thirdColor, 0.65));
            brush.GradientStops.Add(new GradientStop(secondColor, 1));
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }
    }
}
