using SeriesTags.Data.Entities;
using SeriesTags.Data.Repositories;
using SeriesTags.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SeriesTags {
    public class MSSeries : BaseViewModel {
        private CancellationTokenSource cancels;
        private string? imagePath;
        private string? dbFile;
        private string filter = "";

        public string BasePath { get; private set; }

        public string Filter { get => filter; set { filter = value; Changed(); _ = doFilter(); } }
        public string Title => Path.GetFileName(BasePath);

        public string? ImagePath { get => imagePath; set => imagePath = value; }
        public ObservableCollection<MSTag> Tags { get; set; } = new ObservableCollection<MSTag>();
        public ObservableCollection<MSEpisode> Episodes { get; set; } = new ObservableCollection<MSEpisode>();
        public List<MSEpisode> allEpisodes { get; set; } = new List<MSEpisode>();

        public int EpisodeCount => Episodes.Count;


        public MSSeries(string path) {
            BasePath = path;
            var files = Directory.GetFiles(path);
            ImagePath = files.FirstOrDefault(f => f.EndsWith(".jpg") || f.EndsWith(".png"));
            var episodeFiles = files.Where(f => f.EndsWith(".ts") || f.EndsWith(".mp4")).Where(f => Regex.IsMatch(f, @" (episode|ep) *(!?\d*)",RegexOptions.IgnoreCase))
                .GroupBy(f => int.Parse(Regex.Match(Regex.Match(f, @" (episode|ep) *(!?\d*)", RegexOptions.IgnoreCase).Value, @"\d.*").Value))
                .ToDictionary(f => f.Key,f => f.First());
            dbFile = files.FirstOrDefault(f => f.EndsWith(".mssrdb"));
            if (dbFile == null) {
                dbFile = Path.Join(path, Path.GetFileName(path) + ".mssrdb");
                //using (var f = File.Create(dbFile));
                var txtFile = files.FirstOrDefault(f => f.EndsWith(".mssr"));
                if (txtFile != null) {
                    Task.Run(async () => {
                        var lines = File.ReadAllLines(txtFile);
                        var episodes = new List<EpisodeEntity>();
                        await new EpisodesRepository(dbFile).AddTags(Regex.Matches(File.ReadAllText(txtFile), @"\[(.*?)\]").Select(m => m.Value.Replace("[", "").Replace("]", "").Trim().ToTitleCase()).ToHashSet());
                        var allTags = await new EpisodesRepository(dbFile).GetTags();
                        System.Windows.Application.Current.Dispatcher.Invoke(() => {
                            foreach (var t in allTags) {
                                Tags.Add(new MSTag(t));
                            }
                        });
                        var max = 0;
                        foreach (var l in lines) {
                            if (l.Trim().Length == 0) continue;
                            var episode = new EpisodeEntity();
                            var split = l.Split(":");
                            var number = int.Parse(split[0]);
                            episode.Episode = number;
                            var text = split[1].Trim();
                            episode.Title = Regex.Match(text, @"^[a-zA-Z0-9 .;,!?\{\}'\-_\" + "\"" + @"]*").Value;
                            if (text.EndsWith("++++")) {
                                episode.Rating = 10;
                            } else if (text.EndsWith("++")) {
                                episode.Rating = 8;
                            } else if (text.EndsWith("+")) {
                                episode.Rating = 6;
                            } else if (text.EndsWith("--")) {
                                episode.Rating = 0;
                            } else if (text.EndsWith("-")) {
                                episode.Rating = 2;
                            } else {
                                episode.Rating = 5;
                            }
                            var tags = Regex.Matches(text, @"\[(.*?)\]").Select(m => Tags.FirstOrDefault(t => t.Name == m.Value.Replace("[", "").Replace("]", "").Trim().ToTitleCase())).ToList();
                            await new EpisodesRepository(dbFile).AddEpisode(episode);
                            await new EpisodesRepository(dbFile).AddTagsToEpisode(episode.Episode, tags.Select(tt => tt!.id));
                            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                                var mse = new MSEpisode(BasePath, dbFile, episode, Tags);
                                if (episodeFiles.ContainsKey(mse.Episode)) mse.EpisodePath = episodeFiles[mse.Episode];
                                foreach (var t in tags) mse.Tags.Add(t!);
                                allEpisodes.Add(mse);
                            });
                            if (episode.Episode > max) {
                                max = episode.Episode;
                            }
                        }
                        for (int i = 1; i < max; i++) {
                            if (!allEpisodes.Any(e => e.Episode == i)) {
                                var episode = new EpisodeEntity { Episode = i };
                                await new EpisodesRepository(dbFile).AddEpisode(episode);
                                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                                    var mse = new MSEpisode(BasePath, dbFile, episode, Tags);
                                    if (episodeFiles.ContainsKey(mse.Episode)) mse.EpisodePath = episodeFiles[mse.Episode];
                                    allEpisodes.Insert(i - 1, mse);
                                });
                            }
                        }
                        System.Windows.Application.Current.Dispatcher.Invoke(() => {
                            foreach (var e in allEpisodes) {
                                Episodes.Add(e);
                            }
                        });
                    });
                }
            } else {
                Task.Run(async () => {
                    var tags = await new EpisodesRepository(dbFile).GetTags();
                    foreach (var t in tags) {
                        System.Windows.Application.Current.Dispatcher.Invoke(() => {
                            Tags.Add(new MSTag(t));
                        });
                    }
                    var episodes = await new EpisodesRepository(dbFile).GetEpisodes();
                    foreach (var e in episodes) {
                        System.Windows.Application.Current.Dispatcher.Invoke(() => {
                            var mse = new MSEpisode(BasePath, dbFile, e, Tags);
                            if (episodeFiles.ContainsKey(mse.Episode)) mse.EpisodePath = episodeFiles[mse.Episode];
                            allEpisodes.Add(mse);
                            Episodes.Add(mse);
                        });
                    }
                });
            }
            Episodes.CollectionChanged += (a, b) => { Changed(nameof(EpisodeCount)); };
            if (!Directory.Exists(Path.Join(path, "Photos"))) {
                Directory.CreateDirectory(Path.Join(path, "Photos"));
            }
        }

        public async Task<MSTag?> CreateTag(string name) {
            if (name == null || name.Trim().Length == 0) return null;
            if (Tags.Any(t => t.Name.ToLower() == name.Trim().ToLower())) return Tags.First(t => t.Name.ToLower() == name.Trim().ToLower());
            var tag = new TagEntity { Name = name.ToTitleCase() };
            tag = await new EpisodesRepository(dbFile!).AddTag(name);
            if (tag == null) return null;
            var res = new MSTag(tag);
            Application.Current.Dispatcher.Invoke(() => Tags.Add(res));
            return res;
        }
        public async Task<MSEpisode?> CreateEpisode() {
            var episode = new EpisodeEntity();
            episode.Episode = Episodes.Count == 0 ? 1 : Episodes.Select(e => e.Episode).Max() + 1;
           
            if (await new EpisodesRepository(dbFile!).AddEpisode(episode)) {
                var res = new MSEpisode(BasePath, dbFile!, episode, Tags);
                var episodeFile = Directory.GetFiles(BasePath).Where(f => f.EndsWith(".ts") || f.EndsWith(".mp4"))
               .FirstOrDefault(f => Regex.IsMatch(f, @$" (episode|ep) *(!?{episode.Episode})", RegexOptions.IgnoreCase));
                if (episodeFile != null) res.EpisodePath = episodeFile;
                    Application.Current.Dispatcher.Invoke(() => Episodes.Add(res));
                return res;
            }
            return null;
        }

        private async Task doFilter() {
            cancels?.Cancel();
            cancels = new CancellationTokenSource();
            var token = cancels.Token;
            await Task.Delay(300);
            if (token.IsCancellationRequested) return;
            System.Windows.Application.Current.Dispatcher.Invoke(() => {
                Episodes.Clear();
                foreach (var e in allEpisodes.Where(ep => ep.Title.ToLower().Contains(Filter))) {
                    Episodes.Add(e);
                }
            });
        }
    }
}
