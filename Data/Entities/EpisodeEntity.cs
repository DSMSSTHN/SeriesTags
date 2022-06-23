using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriesTags.Data.Entities {
    public class EpisodeEntity {
        [Key]
        public int Episode { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Aired { get; set; }
        public virtual List<EpisodeTag> ETs { get; set; }
        public int Rating { get; set; }
        public string Links { get; set; } = "";
    }
}
