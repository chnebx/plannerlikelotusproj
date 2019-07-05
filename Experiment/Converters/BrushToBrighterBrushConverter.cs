using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Experiment.Converters
{
    class BrushToBrighterBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LinearGradientBrush)
            {
                LinearGradientBrush brush = (LinearGradientBrush)value;
                var color1 = brush.GradientStops[0].Color;
                var color2 = brush.GradientStops[1].Color;
                var color3 = brush.GradientStops[2].Color;
                LinearGradientBrush newBrush = new LinearGradientBrush();
                newBrush.StartPoint = new Point(0.5, 0);
                newBrush.EndPoint = new Point(0.5, 1);
                GradientStop gradStop = new GradientStop(ChangeColorBrightness(color1, 0.3f), 0);
                GradientStop gradStop2 = new GradientStop(ChangeColorBrightness(color2, 0.1f), 0.5);
                GradientStop gradStop3 = new GradientStop(ChangeColorBrightness(color2, 0.4f), 1);
                newBrush.GradientStops.Add(gradStop);
                newBrush.GradientStops.Add(gradStop2);
                newBrush.GradientStops.Add(gradStop3);
                return newBrush;
            }
            Console.WriteLine("relou");
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static System.Windows.Media.Color ChangeColorBrightness(System.Windows.Media.Color color, float correctionFactor)
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

            return System.Windows.Media.Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }
    }
}
