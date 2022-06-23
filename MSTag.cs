using SeriesTags.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SeriesTags {
    public class MSTag : BaseViewModel {
        private string name = "";
        private static SolidColorBrush[] Colors = {
        new SolidColorBrush(Color.FromRgb(160, 76, 89)),
        new SolidColorBrush(Color.FromRgb(121, 141, 151)),
        new SolidColorBrush(Color.FromRgb(35, 112, 138)),
        new SolidColorBrush(Color.FromRgb(23, 86, 115)),
        new SolidColorBrush(Color.FromRgb(44, 44, 44)),
        new SolidColorBrush(Color.FromRgb(64, 0, 0)),
        new SolidColorBrush(Color.FromRgb(128, 0, 0)),
        };

        public int id { get; set; }

        public string Name { get => name; set { name = value; Changed(); } }

        public SolidColorBrush TagColor => Colors[new Random().Next(Colors.Length)];


        public MSTag() {

        }
        public MSTag(TagEntity tag) {
            id = tag.Id;
            name = tag.Name;
        }

        public TagEntity TagEntity => new TagEntity { Id = id, Name = name };
    }
}
