using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace NHkey.View
{
    public class ProgramNameValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(string))
            {
                string programPath = (string)value;
                if (value != null)
                {
                    int lastIndex = programPath.LastIndexOf(@"\");
                    programPath = programPath.Substring(lastIndex + 1);
                }
                else
                {
                    programPath = App.Instance.FindResource("ProgramFieldEmptyMessage") as string;
                }
                return programPath;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
