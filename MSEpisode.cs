using SeriesTags.Data.Entities;
using SeriesTags.Data.Repositories;
using SeriesTags.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SeriesTags {
    public class MSEpisode : BaseViewModel {
        private CancellationTokenSource cancels;
        private string basePath;
        private string dbPath;
        private string title = "";
        private string? description;
        private DateTime created;
        private DateTime? aired;
        private bool episodeExists;
        private int rating;
        private string episodePath;
        private int episode;
        private string? currentPhoto = "/Icons/ArrowRight.png";

        public string PhotosPath => Path.Join(basePath, "Photos", Episode.ToString());

        public bool EpisodeExists { get => episodeExists; set { episodeExists = value; Changed(); } }

        public string EpisodePath { get => episodePath; set { episodePath = value; Changed(); EpisodeExists = value != null && File.Exists(value); } }

        public int Episode {
            get => episode; set {
                episode = value;
                Photos.Clear();
                if (PhotosPath != null && Directory.Exists(PhotosPath)) {
                    var photos = Directory.GetFiles(PhotosPath).Where(f => f.EndsWith(".jpg") || f.EndsWith(".png"));
                    foreach (var p in photos) Photos.Add(p);
                }
                CurrentPhoto = Photos.Count > 0 ? Photos[0] : "/Icons/ArrowRight.png";
                Changed(nameof(HasPhotos));
            }
        }
        public bool HasPhotos => Photos.Count > 0;

        public string? CurrentPhoto { get => currentPhoto; set { currentPhoto = value; Changed(); } }
        public string Title { get => title; set { title = value; _ = Update(); Changed(); } }
        public string? Description { get => description; set { description = value; _ = Update(); Changed(); } }
        public DateTime Created { get => created; set { created = value; _ = Update(); Changed(); } }
        public DateTime? Aired { get => aired; set { aired = value; _ = Update(); Changed(); } }
        public ObservableCollection<MSTag> Tags { get; set; } = new ObservableCollection<MSTag>();
        public int Rating { get => rating; set { rating = value; _ = Update(); Changed(); } }
        public ObservableCollection<string> Links { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Photos { get; set; } = new ObservableCollection<string>();
        public async Task<bool> AddTag(MSTag tag) {
            var res = await new EpisodesRepository(dbPath).AddTagToEpisode(Episode, tag.id);
            if (res >= 0) {
                Application.Current.Dispatcher.Invoke(() => Tags.Add(tag));
                return true;
            }
            return false;
        }
        public async Task<bool> RemoveTag(MSTag tag) {
            if (!Tags.Contains(tag)) return false;
            if (await new EpisodesRepository(dbPath).RemoveTagFromEpisode(Episode, tag.id)) {
                Application.Current.Dispatcher.Invoke(() => Tags.Remove(tag));
                return true;
            }
            return false;
        }

        public async Task<bool> Update() {
            cancels?.Cancel();
            cancels = new CancellationTokenSource();
            var token = cancels.Token;
            await Task.Delay(1000);
            if (token.IsCancellationRequested) return false;
            return await new EpisodesRepository(dbPath).Update(EpisodeEntity);
        }


        public MSEpisode(string basePath, string dbPath) {
            this.basePath = basePath;
            this.dbPath = dbPath;

        }
        public void AddPhoto(string imagePath) {
            if (!Directory.Exists(PhotosPath)) Directory.CreateDirectory(PhotosPath);
            Photos.Add(imagePath);
            Changed(nameof(HasPhotos));
        }
        public MSEpisode(string basePath, string dbPath, EpisodeEntity ee, IEnumerable<MSTag> tags) : this(basePath, dbPath) {

            this.Episode = ee.Episode;
            this.title = ee.Title;
            this.Description = ee.Description;
            this.created = ee.Created;
            this.aired = ee.Aired;
            Rating = ee.Rating;
            Links = ee.Links.Split(';').Where(l => l.Trim().Length > 0).ToObservable();

            if (ee.ETs != null) this.Tags = tags.Where(t => ee.ETs.Any(et => et.TagId == t.id)).ToObservable();

        }

        public EpisodeEntity EpisodeEntity => new EpisodeEntity {
            Episode = Episode,
            Aired = Aired,
            Created = Created,
            Description = Description,
            Title = Title,
            Rating = Rating,
            Links = String.Join(';', Links.Where(l => l.Trim().Length > 0).ToHashSet().Select(l => l.ToLower().Trim())),
        };
        public void NextPhoto() {
            if (Photos.Count < 2 || CurrentPhoto == null) return;
            var index = Photos.IndexOf(CurrentPhoto);
            if (index == -1) return;
            CurrentPhoto = Photos[(index + 1) % Photos.Count];
        }
        public void PreviousPhoto() {
            if (Photos.Count < 2 || CurrentPhoto == null) return;
            var index = Photos.IndexOf(CurrentPhoto);
            if (index == -1) return;
            index--;
            if(index < 0)index = Photos.Count - 1;
            CurrentPhoto = Photos[index];
        }
    }
}
