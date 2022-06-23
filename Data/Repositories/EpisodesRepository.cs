using Microsoft.EntityFrameworkCore;
using SeriesTags.Data.Entities;
using SeriesTags.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriesTags.Data.Repositories {
    public class EpisodesRepository : BaseRepository {
        public EpisodesRepository(string dbPath) : base(dbPath) {


        }

        public async Task<bool> AddEpisode(EpisodeEntity episode) {
            try {
                episode.Created = DateTime.Now;
                await context.Episodes.AddAsync(episode);
                await context.SaveChangesAsync();
                return true;
            } catch {
                return false;
            }

        }
        public async Task<bool> Update(EpisodeEntity episode) {
            try {
                episode.Created = DateTime.Now;
                context.Episodes.Update(episode);
                await context.SaveChangesAsync();
                return true;
            } catch {
                return false;
            }
        }
        public async Task<TagEntity?> AddTag(string name) {
            try {
                var t = new TagEntity { Name = name.ToTitleCase() };
                await context.Tags.AddAsync(t);
                await context.SaveChangesAsync();
                return t;
            } catch {
                return null;
            }
        }
        public async Task<List<TagEntity>> AddTags(IEnumerable<string> names) {
            try {
                var tags = names.Select(n => new TagEntity { Name = n.ToTitleCase() });
                await context.Tags.AddRangeAsync(tags);
                await context.SaveChangesAsync();
                return tags.ToList();
            } catch {
                return new List<TagEntity>();
            }
        }
        public async Task<int> AddTagToEpisode(int episodeNr, string tag) {
            var t = tag.ToTitleCase();
            if (t.Length == 0) return -1;
            var dbt = await context.Tags.FirstOrDefaultAsync(tag => tag.Name == t);
            if (dbt == null) {
                dbt = (await context.Tags.AddAsync(new TagEntity { Name = t.ToTitleCase() })).Entity;
                await context.SaveChangesAsync();
            }
            return await AddTagToEpisode(episodeNr, dbt.Id);
        }
        public async Task<int> AddTagToEpisode(int episodeNr, int tagId) {
           
            try {
                await context.EpisodeTags.AddAsync(new EpisodeTag { EpisodeNumber = episodeNr, TagId = tagId });
                await context.SaveChangesAsync();
                return tagId;
            } catch {
                return -1;
            }
        }
        public async Task<bool> AddTagsToEpisode(int episodeNr, IEnumerable<int> tagIds) {

            try {
                await context.EpisodeTags.AddRangeAsync(tagIds.Select(tagId => new EpisodeTag { EpisodeNumber = episodeNr, TagId = tagId }));
                await context.SaveChangesAsync();
                return true;
            } catch {
                return false;
            }
        }
        public async Task<bool> RemoveTagFromEpisode(int episodeNr, int tagId) {
            try {
                context.EpisodeTags.Remove(new EpisodeTag { EpisodeNumber = episodeNr, TagId = tagId });
                await context.SaveChangesAsync();
                return true;
            } catch {
                return false;
            }
        }

        public async Task<List<TagEntity>> GetTags() => await context.Tags.ToListAsync();
        public async Task<List<EpisodeEntity>> GetEpisodes() => await context.Episodes.Include(e => e.ETs).IgnoreAutoIncludes().ToListAsync();

       
    }
}
