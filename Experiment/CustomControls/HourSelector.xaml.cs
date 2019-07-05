using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Experiment.CustomControls
{
    /// <summary>
    /// Logique d'interaction pour HourSelector.xaml
    /// </summary>
    public partial class HourSelector : UserControl
    {
        public HourSelector()
        {
            InitializeComponent();
        }

        private void TxtHour_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }



        public string HourValue
        {
            get { return (string)GetValue(HourValueProperty); }
            set { SetValue(HourValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HourValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HourValueProperty =
            DependencyProperty.Register("HourValue", typeof(string), typeof(HourSelector), new PropertyMetadata(null));



        public string MinutesValue
        {
            get { return (string)GetValue(MinutesValueProperty); }
            set { SetValue(MinutesValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinutesValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinutesValueProperty =
            DependencyProperty.Register("MinutesValue", typeof(string), typeof(HourSelector), new PropertyMetadata(null));




        private void TxtHour_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            using (tb.DeclareChangeBlock())
            {
                foreach (var c in e.Changes)
                {
                    if (c.AddedLength == 0) continue;
                    tb.Select(c.Offset, c.AddedLength);
                    if (tb.SelectedText.Contains(' '))
                    {
                        tb.SelectedText = tb.SelectedText.Replace(' ', '0');
                    }
                    tb.Select(c.Offset + c.AddedLength, 0);
                }
            }
            /*
            if (tb.Text.Count() > 0)
            {
                int val = int.Parse(tb.Text);
                
                if (int.Parse(tb.Text) > 23)
                {
                    tb.Text = (val % 24).ToString("00");
                }
                
            }
            */
        }


        private void TxtMinutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            using (tb.DeclareChangeBlock())
            {
                foreach (var c in e.Changes)
                {
                    if (c.AddedLength == 0) continue;
                    tb.Select(c.Offset, c.AddedLength);
                    if (tb.SelectedText.Contains(' '))
                    {
                        tb.SelectedText = tb.SelectedText.Replace(' ', '0');
                    }
                    tb.Select(c.Offset + c.AddedLength, 0);
                }
            }

            /*
            if (tb.Text.Count() > 0 && int.Parse(tb.Text) > 59)
            {
                int val = int.Parse(tb.Text);
                tb.Text = (val % 60).ToString("00");
            }
            */
        }

        private void DigitBtns_Click(object sender, RoutedEventArgs e)
        {
            Button actual = (Button)sender;
            int hour;
            bool hourSuccess = int.TryParse(txtHour.Text, out hour);
            int minutes;
            bool minutesSuccess = int.TryParse(txtMinutes.Text, out minutes);
            switch (actual.Name)
            {
                case "HourFirstDigitIncreaseBtn":
                    if (hourSuccess)
                    {  
                        HourValue = (hour + 10).ToString("00");
                    }
                    break;
                case "MinutesFirstDigitIncreaseBtn":
                    if (minutesSuccess)
                    {
                        MinutesValue = (minutes + 10).ToString("00");
                    }
                    break;
                case "HourSecondDigitIncreaseBtn":
                    if (hourSuccess)
                    {
                        HourValue = (hour + 1).ToString("00");
                    }
                    break;
                case "MinutesSecondDigitIncreaseBtn":
                    if (minutesSuccess)
                    {
                        MinutesValue = (minutes + 1).ToString();
                    }
                    break;
                case "HourFirstDigitDecreaseBtn":
                    if (hourSuccess)
                    {
                        HourValue = (hour - 10).ToString();
                    }
                    break;
                case "MinutesFirstDigitDecreaseBtn":
                    if (minutesSuccess)
                    {
                       MinutesValue = (minutes - 10).ToString();
                    }
                    break;
                case "HourSecondDigitDecreaseBtn":
                    if (hourSuccess)
                    {
                        HourValue = (hour - 1).ToString();
                    }
                    break;
                case "MinutesSecondDigitDecreaseBtn":
                    if (minutesSuccess)
                    {
                        MinutesValue = (minutes - 1).ToString();
                    }
                    break;
                default:
                    return;
            }
        }
    }
}
