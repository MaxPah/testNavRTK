using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApplication1.Converters
{
    public class ColorConverter : IValueConverter
    {
        public enum Status
        {
            StatusOK,
            StatusKO
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string state = (string)value;

            switch (state)
            {
                case "StatusOK":
                    return new SolidColorBrush(Colors.Green);
                case "StatusKO":
                        return new SolidColorBrush(Colors.Red);
                default: return new SolidColorBrush(Colors.Blue);
            }
        }

       public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
