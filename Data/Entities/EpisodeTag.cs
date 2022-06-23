using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriesTags.Data.Entities {
    public class EpisodeTag {
        public int EpisodeNumber { get; set; }
        public virtual EpisodeEntity? Episode { get; set; }
        public int TagId { get; set; }
        public virtual TagEntity? Tag { get; set; }
    }
}
