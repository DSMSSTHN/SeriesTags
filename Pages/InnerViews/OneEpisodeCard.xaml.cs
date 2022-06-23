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

namespace SeriesTags.Pages.InnerViews {
    /// <summary>
    /// Interaction logic for OneEpisodeCard.xaml
    /// </summary>
    public partial class OneEpisodeCard : UserControl {
        internal static readonly DependencyProperty EpisodeProp = DependencyProperty.Register("Episode", typeof(MSEpisode),typeof(OneEpisodeCard),new PropertyMetadata(null));

        public MSEpisode? Episode { get => GetValue(EpisodeProp) as MSEpisode; set => SetValue(EpisodeProp, value); }

        public OneEpisodeCard() {
            InitializeComponent();

            this.Loaded += (a, b) => {
                SeriesGrid.DataContext = Episode;
            };
        }

        private void RemoveTag(object sender, RoutedEventArgs e) {
            var tag = (sender as Button)?.DataContext as MSTag;
            if (tag == null) return;
            _ = Episode?.RemoveTag(tag);
        }

        private void PreviousPhoto(object sender, RoutedEventArgs e) {
            Episode?.PreviousPhoto();
        }

        private void NextPhoto(object sender, RoutedEventArgs e) {
            Episode?.NextPhoto();
        }
    }
}
