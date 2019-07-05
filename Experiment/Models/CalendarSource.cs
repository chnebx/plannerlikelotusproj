using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Experiment.Models
{
    class CalendarSource
    {
        private static ObservableCollection<String> _daysList;
        private static ObservableCollection<String> _daysNames;
        private static ObservableCollection<String> _monthNames;
        public static LinearGradientBrush defaultMonthBrush = new LinearGradientBrush();
        public static LinearGradientBrush hoveredMonthBrush = new LinearGradientBrush();
        static CalendarSource()
        {
            _daysList = new ObservableCollection<string>();
            _daysNames = new ObservableCollection<string> { "L", "M", "M", "J", "V", "S", "D" };
            for (int i = 0; i < 37; i++)
            {
                _daysList.Add(_daysNames[i % 7]);
            }

            _monthNames = new ObservableCollection<string> { "Jan", "Fév", "Mar", "Avr", "Mai", "Jun", "Jui", "Aou", "Sep", "Oct", "Nov", "Dec" };

            defaultMonthBrush.EndPoint = new Point(0.5, 1);
            defaultMonthBrush.StartPoint = new Point(0.5, 0);
            Color col0 = (Color)ColorConverter.ConvertFromString("#FF9A9DB9");
            Color col1 = (Color)ColorConverter.ConvertFromString("#FF9A9EB9");
            defaultMonthBrush.GradientStops = new GradientStopCollection();
            defaultMonthBrush.GradientStops.Add(new GradientStop(col0, 0));
            defaultMonthBrush.GradientStops.Add(new GradientStop(col1, 1));
            defaultMonthBrush.MappingMode = BrushMappingMode.RelativeToBoundingBox;

            hoveredMonthBrush.EndPoint = new Point(0.5, 1);
            hoveredMonthBrush.StartPoint = new Point(0.5, 0);
            Color color0 = (Color)ColorConverter.ConvertFromString("#FF6A6DB7");
            Color color1 = (Color)ColorConverter.ConvertFromString("#FF6A6EB7");
            hoveredMonthBrush.GradientStops = new GradientStopCollection();
            hoveredMonthBrush.GradientStops.Add(new GradientStop(color0, 0));
            hoveredMonthBrush.GradientStops.Add(new GradientStop(color1, 1));
            hoveredMonthBrush.MappingMode = BrushMappingMode.RelativeToBoundingBox;

        }

        public static ObservableCollection<String> getDaysNames()
        {
            return _daysList;
        }

        public static ObservableCollection<String> getMonthNames()
        {
            return _monthNames;
        }
    }
}
