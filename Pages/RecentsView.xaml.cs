using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
using Microsoft.WindowsAPICodePack.Dialogs;

namespace SeriesTags.Pages {
    /// <summary>
    /// Interaction logic for RecentsView.xaml
    /// </summary>
    public partial class RecentsView : Page {
        public ObservableCollection<MSSeries> Series => STOptions.RecentProjects;
        public RecentsView() {
            InitializeComponent();
        }

        private void SeriesClicked(object sender, RoutedEventArgs e) {
            var series = (sender as Button)?.DataContext as MSSeries;
            if (series == null) return;
            openSeries(series);
        }

        private void AddClicked(object sender, RoutedEventArgs e) {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                var ser = STOptions.AddRecent(dialog.FileName);
                if(ser != null) {
                    openSeries(ser);
                }
            }
        }
        private void openSeries(MSSeries ser) {
            STOptions.PutFirst(ser);
            NavigationService.Navigate(new SeriesView(ser));
        }
    }
}
