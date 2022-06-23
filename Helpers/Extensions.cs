using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriesTags.Helpers {
    public static class Extensions {
        public static string ToTitleCase(this string str) {
            if (str == null || str.Length == 0) return "";
            var s = str.Trim();
            if (s.Length == 0) return s;
            try {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                var res = textInfo.ToTitleCase(s.ToLower());
                return res;
            } catch (Exception) {
                return str;
            }
        }
        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> items) {
            var res = new ObservableCollection<T>();
            foreach(var i in items) res.Add(i);
            return res;
        }
        public static void OpenInChrome(this string url) {

            String chrome = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
            //var incognito = (Application.Current.MainWindow as MainWindow).IsIncognito;
            Process.Start(chrome, url + " " + (false ? "--incognito --new-window" : "--new-window "));
        }
    }
}
