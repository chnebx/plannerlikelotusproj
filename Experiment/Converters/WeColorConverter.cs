using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Experiment.Converters
{
    class WeColorConverter : IValueConverter
    {
        public LinearGradientBrush enabledColor;
        public LinearGradientBrush WEColor;
        public LinearGradientBrush IsToday;
        //#E5F3F3F3
        //#D8E6E6E6

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String val = value.ToString();     
            if (val == "Enabled")
            {
                if (enabledColor != null) { return enabledColor; }
                else
                {
                    LinearGradientBrush standardColor = new LinearGradientBrush();
                    standardColor.EndPoint = new Point(0.5, 1);
                    standardColor.StartPoint = new Point(0.5, 0);
                    Color col0 = (Color)ColorConverter.ConvertFromString("#EFF3F3F3");
                    Color col1 = (Color)ColorConverter.ConvertFromString("#EFE9E9E9");
                    standardColor.GradientStops = new GradientStopCollection();
                    standardColor.GradientStops.Add(new GradientStop(col0, 0));
                    standardColor.GradientStops.Add(new GradientStop(col1, 1));
                    standardColor.MappingMode = BrushMappingMode.RelativeToBoundingBox;
                    //standardColor.SpreadMethod = GradientSpreadMethod.Reflect;
                    //TransformGroup myGroup = new TransformGroup();
                    //ScaleTransform scaleT = new ScaleTransform();
                    //scaleT.CenterX = 0.5;
                    //scaleT.CenterY = 0.5;
                    //SkewTransform skewT = new SkewTransform();
                    //skewT.CenterX = 0.5;
                    //skewT.CenterY = 0.5;
                    //RotateTransform rotateT = new RotateTransform();
                    //rotateT.CenterX = 0.5;
                    //rotateT.CenterY = 0.5;
                    //rotateT.Angle = -15.642;
                    //myGroup.Children.Add(scaleT);
                    //myGroup.Children.Add(skewT);
                    //myGroup.Children.Add(rotateT);
                    //standardColor.RelativeTransform = myGroup;
                    enabledColor = standardColor;
                    return enabledColor;
                }
                
            } else if (val == "IsWE")
            {
                if ( WEColor != null ) { return WEColor;  }
                else
                {
                    LinearGradientBrush color = new LinearGradientBrush();
                    color.StartPoint = new Point(0.5, 0);
                    color.EndPoint = new Point(0.5, 1);
                    Color col0 = (Color)ColorConverter.ConvertFromString("#EDEAD284");
                    Color col1 = (Color)ColorConverter.ConvertFromString("#EDDEC46A");
                    color.GradientStops = new GradientStopCollection();
                    color.GradientStops.Add(new GradientStop(col0, 0));
                    color.GradientStops.Add(new GradientStop(col1, 1));
                    WEColor = color;
                    return WEColor;
                }
                
            } else
            {
                if ( IsToday != null ) { return IsToday;  }
                else
                {
                    LinearGradientBrush color = new LinearGradientBrush();
                    color.StartPoint = new Point(0.5, 0);
                    color.EndPoint = new Point(0.5, 1);
                    Color col0 = (Color)ColorConverter.ConvertFromString("#EDB6C5E4");
                    Color col1 = (Color)ColorConverter.ConvertFromString("#ED899ED4");
                    color.GradientStops = new GradientStopCollection();
                    color.GradientStops.Add(new GradientStop(col0, 0));
                    color.GradientStops.Add(new GradientStop(col1, 1));
                    IsToday = color;
                    return IsToday;
                }
               
            }

            //throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
