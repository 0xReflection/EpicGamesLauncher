using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Models
{
    public class Game
    {
        public int GameId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int DeveloperId { get; set; }
        public int PublisherId { get; set; }
        public string CoverImage { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Developer Developer { get; set; }
        public Publisher Publisher { get; set; }
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public List<Platform> Platforms { get; set; } = new List<Platform>();
        public string GenresString => Genres != null ? string.Join(", ", Genres.Select(g => g.Name)) : "";
        public string PlatformsString => Platforms != null ? string.Join(", ", Platforms.Select(p => p.Name)) : "";
    }

}
