using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace AdminGUI.Resourses
{
    [ValueConversion(typeof(int), typeof(String))]
    class StringToDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int tmp, time, minutes, sec;
            time = (int) value;
            int hours = Math.DivRem(time, 60 * 60, out minutes);
            minutes = Math.DivRem(minutes, 60, out sec);
            int summ = Math.DivRem(hours, 24, out tmp);
            if (summ == 0)
                return string.Format("{0}:{1}:{2}", hours, minutes, sec);
            else
                return string.Format("{3} {0}:{1}:{2}", tmp, minutes, sec, summ/6);
            //return 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "back";
        }
    }
}
