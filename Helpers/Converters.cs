
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace SeriesTags.Helpers {
   
    public class MinutesToHoursConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var i = value as int?;
            if (i == null) {
                return "";
            }

            int hours = i.Value / 60;
            int minutes = i.Value % 60;
            return $"{hours} Hours and {minutes} Minutes";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new MissingMethodException();
        }
    }
    public class IntToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value?.ToString() ?? "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            int result = 0;
            int.TryParse(value?.ToString() ?? "0", out result);
            return result;
        }
    }
    
    public class CollectionToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var result = "";
            var seperator = parameter as string ?? ",";
            if (seperator == "NewLine") {
                seperator = "\n";
            }

            if (value is ICollection<object>) {
                return string.Join(seperator, value as ICollection<object>);
            } else if (value is IEnumerable<object>) {
                return string.Join(seperator, value as IEnumerable<object>);

            }



            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class EnumToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
    public class BooleanOppositConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return !((bool)value);
        }
    }
    public class PathExistsVisibility_CollapseConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var str = value?.ToString();
            var bol = File.Exists(str) || Directory.Exists(str);
            var inverse = parameter != null && bool.Parse(parameter.ToString());
            if (inverse) {
                return bol ? Visibility.Visible : Visibility.Collapsed;
            }

            return bol ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }


    public class BooleanScrollVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var inverse = parameter != null && bool.Parse(parameter.ToString());
            if (inverse) {
                return ((bool)value) ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Visible;
            }

            return ((bool)value) ? ScrollBarVisibility.Visible : ScrollBarVisibility.Disabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            var inverse = parameter != null && bool.Parse(parameter.ToString());
            if (inverse) {
                return ((ScrollBarVisibility)value) != ScrollBarVisibility.Visible;
            }

            return ((ScrollBarVisibility)value) == ScrollBarVisibility.Visible;
        }
    }
    public class BooleanVisibility_HiddenConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var inverse = parameter != null && bool.Parse(parameter.ToString());
            if (inverse) {
                return ((bool)value) ? Visibility.Hidden : Visibility.Visible;
            }

            return ((bool)value) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            var inverse = parameter != null && bool.Parse(parameter.ToString());
            if (inverse) {
                return ((Visibility)value) != Visibility.Visible;
            }

            return ((Visibility)value) == Visibility.Visible;
        }
    }
    public class BooleanVisibility_CollapseConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var inverse = parameter != null && bool.Parse(parameter.ToString());
            if (inverse) {
                return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
            }

            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            var inverse = parameter != null && bool.Parse(parameter.ToString());
            if (inverse) {
                return ((Visibility)value) != Visibility.Visible;
            }

            return ((Visibility)value) == Visibility.Visible;
        }
    }
    public class BooleanOpposetConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return !((bool)value);
        }
    }

    public class DoubleDividerConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return parameter == null ? value : (((double)value) / (double.Parse(parameter.ToString())));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return parameter == null ? value : (((double)value) * (double.Parse(parameter.ToString())));
        }
    }
    
    public class IntToBrushConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            string str = new IntToColorConverter().Convert(value, targetType, parameter, culture).ToString();
            return new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString(str)) ?? new SolidColorBrush(Colors.Black);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return (value as System.Windows.Media.Color?)?.ToString();
        }
    }
    public class IntToColorConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var alpha = "";
            if (parameter as double? != null) {
                var d = parameter as double?;
                int b255 = (int)Math.Round(d!.Value * 255);
                alpha = b255.ToString("X2");
            } else if (parameter != null) {
                double d = 0;
                if (double.TryParse(parameter.ToString(), out d)) {
                    int b255 = (int)Math.Round(d * 255);
                    alpha = b255.ToString("X2");
                }

            }
            var i = (int)value;
            switch (i) {
                case 1:
                    return alpha + "#FF4C29";
                case 2:
                    return alpha + "#334756";
                case 3:
                    return alpha + "#2C394B";
                case 4:
                    return alpha + "#082032";
                case 5:
                    return alpha + "#B42B51";
                case 6:
                    return alpha + "#420516";
                case 7:
                    return alpha + "#5C3D2E";
                case 8:
                    return alpha + "#F4ABC4";
                case 9:
                    return alpha + "#2B2B2B";
                case 10:
                    return alpha + "#787A91";
                case 11:
                    return alpha + "#141E61";
                case 12:
                    return alpha + "#0F044C";
                case 13:
                    return alpha + "#8FD6E1";
                case 14:
                    return alpha + "#1597BB";
                case 15:
                    return alpha + "#F05454";
                case 16:
                    return alpha + "#B4A5A5";
                case 17:
                    return alpha + "#726A95";
                case 18:
                    return alpha + "#351F39";
                case 19:
                    return alpha + "#090030";
                default:
                    return alpha + "#171717";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            var i = value.ToString();
            switch (i) {
                case "#c62828":
                    return 1;
                case "#ad1457":
                    return 2;
                case "#6a1b9a":
                    return 3;
                case "#4527a0":
                    return 4;
                case "#283593":
                    return 5;
                case "#1565c0":
                    return 6;
                case "#0277bd":
                    return 7;
                case "#00838f":
                    return 8;
                case "#00695c":
                    return 9;
                case "#2e7d32":
                    return 10;
                case "#33691e":
                    return 11;
                case "#827717":
                    return 12;
                case "#bc5100":
                    return 13;
                case "#c43e00":
                    return 14;
                case "#b53d00":
                    return 15;
                case "#9f0000":
                    return 16;
                case "#4e342e":
                    return 17;
                case "#424242":
                    return 18;
                case "#37474f":
                    return 19;
                default:
                    return 0;
            }
        }
    }
}
