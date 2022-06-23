using System;
using System.Collections.Generic;
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

namespace SeriesTags.Pages {
    /// <summary>
    /// Interaction logic for SeriesView.xaml
    /// </summary>
    public partial class SeriesView : Page {
        public MSSeries Series { get; set; }
        public SeriesView(MSSeries series) {
            this.Series = series;
            InitializeComponent();
           
        }

        private void GoBack(object sender, RoutedEventArgs e) {
            NavigationService.GoBack();
        }

        private void RemoveTag(object sender, RoutedEventArgs e) {

        }

        private void SeriesKeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Escape: e.Handled = true; NavigationService.GoBack(); break;
               
            }
        }

        private void EpisodeSelected(object sender, SelectionChangedEventArgs e) {
            var episode = EpisodeListView.SelectedItem as MSEpisode;
            if (episode == null) return;
            EpisodeListView.SelectedIndex = -1;
            NavigationService.Navigate(new EpisodeView(Series,episode));
        }

        private async void AddEpisode(object sender, RoutedEventArgs e) {
            var ep = await Series.CreateEpisode();
            if (ep != null) NavigationService.Navigate(new EpisodeView(Series, ep));
        }
    }
}
