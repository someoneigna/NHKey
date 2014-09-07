using NHkey.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Resources;

namespace NHkey.View
{
    public class HotkeyNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var hotkey = value as HotkeyAssociation;
            string name = hotkey.Name;

            if (hotkey.Orphaned == true)
            {
                string orphaned = Application.Current.FindResource("OrphanedHotkeyLabel") as string;
                return string.Format(name + " - " + orphaned);
            }
            return name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
