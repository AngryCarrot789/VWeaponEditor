using System;
using System.Globalization;
using System.Windows.Data;

namespace VWeaponEditor.Processes {
    public class ProcessNameConverter : IMultiValueConverter {
        public static ProcessNameConverter Instance { get; } = new ProcessNameConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            string processName = values[0] as string;
            bool? isResponding = values[1] as bool?;
            bool? isAlive = values[2] as bool?;

            if (isAlive.HasValue && isAlive.Value) {
                if (isResponding.HasValue && isResponding.Value) {
                    return processName;
                }
                else {
                    return $"{processName} (Not Responding)";
                }
            }
            else {
                return $"{processName} (Dead)";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
