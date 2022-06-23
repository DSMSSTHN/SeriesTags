using SeriesTags.Data.Entities;
using SeriesTags.Data.Repositories;
using SeriesTags.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace SeriesTags.Pages {
    /// <summary>
    /// Interaction logic for EpisodeView.xaml
    /// </summary>
    public partial class EpisodeView : Page, INotifyPropertyChanged {
        private MSEpisode episode;
        private string tagFilter;
        private string tagSuggestion;
        private int tagSuggestionIndex;
        private string linkText;

        public MSEpisode Episode { get => episode; set { episode = value; Changed(); } }
        public MSSeries Series { get; set; }


        public string LinkText { get => linkText; set { linkText = value; Changed(); } }
        public string TagFilter { get => tagFilter; set { tagFilter = value; filterTags(value); Changed(); } }
        public int TagSuggestionIndex { get => tagSuggestionIndex; set { tagSuggestionIndex = value; TagSuggestion = Suggestions.Count() == 0 || value < 0 ? "" : Suggestions[value].Name; } }
        public string TagSuggestion { get => tagSuggestion; set { tagSuggestion = value; Changed(); } }
        public List<MSTag> Suggestions { get; set; } = new List<MSTag>();

        public EpisodeView(MSSeries series, MSEpisode episode) {
            MainWindow.CurrentEpisode = episode;
            this.episode = episode;
            Series = series;
            InitializeComponent();
            Loaded += (a, b) => {
                Window.GetWindow(this).PreviewKeyDown += (a, b) => {
                    if (b.Key == Key.LeftAlt || b.Key == Key.RightAlt) b.Handled = true;
                };
                Dispatcher.BeginInvoke(async () => {
                    await Task.Delay(20);
                    TitleTextBox.Focus();
                });
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void Changed([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        private void GoBack(object sender, RoutedEventArgs e) {
            goBack();
        }
        private void NextEpisode(object sender, RoutedEventArgs e) {
            nextEpisode();
        }
        private void PreviousEpisode(object sender, RoutedEventArgs e) {
            previousEpisode();
        }



        private void EpisodeKeyDown(object sender, KeyEventArgs e) {

            switch (e.Key) {
                case Key.Escape: e.Handled = true; goBack(); break;
                case Key.Q:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                        e.Handled = true;
                        previousEpisode();
                    }
                    break;
                case Key.E:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                        e.Handled = true;
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) copyToNextEpisode();
                        else nextEpisode();
                    }
                    break;
                case Key.N:
                    if(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                        e.Handled = true;
                        Task.Run(async () => {
                            var ep = await Series.CreateEpisode();
                            if (ep != null) Episode = ep;

                        });
                    }
                    break;

            }
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt) {
                switch (e.SystemKey) {
                    case Key.Left:
                        e.Handled = true;
                        previousEpisode();
                        break;
                    case Key.Right:
                        e.Handled = true;
                        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) copyToNextEpisode();
                        else nextEpisode();
                        break;
                    case Key.S:
                        e.Handled = true;
                        TagTextBox.Focus();
                        break;
                    case Key.A:
                        e.Handled = true;
                        TitleTextBox.Focus();
                        break;
                    case Key.D:
                        e.Handled = true;
                        LinkTextBox.Focus();
                        break;
                }
            }
        }

        private void filterTags(string filter) {
            TagSuggestionIndex = -1;
            if (filter == null || filter.Trim().Length == 0 || Series.Tags.Count == 0) {
                Suggestions.Clear();
                return;
            }
            Suggestions = Series.Tags.Where(t => t.Name.ToLower().Contains(filter.Trim().ToLower())).Except(Episode.Tags).ToList();
            if (Suggestions.Count > 0) TagSuggestionIndex = 0;

        }
        private void goBack() {
            NavigationService.GoBack();
            MainWindow.CurrentEpisode = null;
        }
        private void nextEpisode() {
            var index = Series.Episodes.IndexOf(Episode);
            if (index == Series.Episodes.Count - 1) index = -1;
            Episode = Series.Episodes[index + 1];
            MainWindow.CurrentEpisode = episode;
        }
        private void copyToNextEpisode() {
            var index = Series.Episodes.IndexOf(Episode);
            if (index == Series.Episodes.Count - 1) return;
            var newEp = Series.Episodes[index + 1];
            newEp.Title = Episode.Title;
            newEp.Description = Episode.Description;
            foreach (var t in Episode.Tags) _ = newEp.AddTag(t);
            _ = newEp.Update();
            Episode = newEp;
            MainWindow.CurrentEpisode = episode;
        }
        private void previousEpisode() {
            var index = Series.Episodes.IndexOf(Episode);
            if (index == 0) index = Series.Episodes.Count;
            Episode = Series.Episodes[index - 1];
            MainWindow.CurrentEpisode = episode;
        }

        private void RemoveTag(object sender, RoutedEventArgs e) {
            var tag = (sender as Button)?.DataContext as MSTag;
            if (tag == null) return;
            _ = Episode.RemoveTag(tag);
        }

        private async void TagKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Tab) {
                e.Handled = true;
                if (Suggestions.Count > 1) {
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
                        var i = TagSuggestionIndex - 1;
                        if (i <= 0) i = Suggestions.Count - 1;
                        TagSuggestionIndex = i;
                    } else {
                        TagSuggestionIndex = (TagSuggestionIndex + 1) % Suggestions.Count;
                    }
                }
            } else if (e.Key == Key.Enter) {
                e.Handled = true;
                if (TagFilter.Trim().Length == 0) return;

                if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift) && (Suggestions.Count > 0 && tagSuggestionIndex >= 0)) {
                    _ = Episode.AddTag(Suggestions[TagSuggestionIndex]);
                    TagFilter = "";
                } else {
                    if (Episode.Tags.Any(t => t.Name.ToLower() == tagFilter.Trim().ToLower())) return;
                    var t = Series.Tags.FirstOrDefault(t => t.Name.ToLower() == TagFilter.Trim().ToLower());
                    if (t == null) {
                        t = await Series.CreateTag(TagFilter);
                    }
                    if (t != null) await Episode.AddTag(t);
                    Dispatcher.Invoke(() => TagFilter = "");
                }
            }
            // else if (e.Key == Key.Escape) {
            //    e.Handled = true;
            //    if (Suggestions.Count == 0 || tagSuggestionIndex < 0) {
            //        TagFilter = "";
            //    } else {
            //        TagSuggestionIndex = -1;
            //    }
            //}
        }

        private void RemoveLink(object sender, RoutedEventArgs e) {
            var str = (sender as Button)?.DataContext as string;
            if (str == null) return;
            Episode.Links.Remove(str);
        }

        private void LinkKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                e.Handled = true;
                if (LinkText.Trim().Length == 0 || Episode.Links.Any(l => l.ToLower() == LinkText.Trim().ToLower())) {
                    LinkText = "";
                    return;
                }
                Episode.Links.Add(LinkText.Trim().ToLower());
                _ = Episode.Update();
                LinkText = "";
            }
        }

        private void SelectAllFocus(object sender, RoutedEventArgs e) {
            Dispatcher.BeginInvoke(async () => {
                await Task.Delay(30);
                (sender as TextBox)?.SelectAll();
            });
        }

        private void OpenEpisodeFile(object sender, RoutedEventArgs e) {
            if (Episode.EpisodeExists && File.Exists(Episode.EpisodePath)) {
                //System.Diagnostics.Process.Start(@Episode.EpisodePath);
                var process = new Process();
                var startInfo = new ProcessStartInfo {
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(Episode.EpisodePath),
                    FileName = Path.GetFileName(Episode.EpisodePath),
                };
                process.StartInfo = startInfo;
                process.Start();
            }
        }

        private void OpenLink(object sender, RoutedEventArgs e) {
            var link = (sender as Button)?.DataContext as string;
            if (link == null) return;
            try {
                link.OpenInChrome();
            } catch { }
        }
    }
}
