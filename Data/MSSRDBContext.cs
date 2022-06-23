using Microsoft.EntityFrameworkCore;
using SeriesTags.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriesTags.Data {
    public class MSSRDBContext : DbContext {

        public DbSet<EpisodeEntity> Episodes { get; set; }
        public DbSet<TagEntity> Tags { get; set; }
        public DbSet<EpisodeTag> EpisodeTags { get; set; }

        private string dbPath;


        public MSSRDBContext(string dbPath):base() {
            this.dbPath = dbPath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            //#if DEBUG
            //            optionsBuilder.UseSqlite(@"Data Source=.\msdmtest.db", x => x.UseNodaTime());
            //#else
            optionsBuilder.UseSqlite($"Data Source={dbPath};");
            //#endif
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            optionsBuilder.UseSnakeCaseNamingConvention();
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EpisodeTag>(e => {
                e.HasKey(et => new { et.EpisodeNumber, et.TagId });
                e.HasOne(ep => ep.Episode).WithMany(et => et.ETs).HasForeignKey(et => et.EpisodeNumber);
                e.HasOne(ep => ep.Tag).WithMany(et => et.ETs).HasForeignKey(et => et.TagId);
            });
           
        }

    }
}
