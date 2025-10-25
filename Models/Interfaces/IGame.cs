using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Models.Interfaces
{
    public interface IGame : IEntity
    {
        string Title { get; set; }
        string Description { get; set; }
        DateTime? ReleaseDate { get; set; }
        int DeveloperId { get; set; }
        int PublisherId { get; set; }
        string CoverImage { get; set; }
        decimal Price { get; set; }
        DateTime CreatedAt { get; set; }



        IDeveloper Developer { get; set; }
        IPublisher Publisher { get; set; }
        ICollection<IGenre> Genres { get; set; }
        ICollection<IPlatform> Platforms { get; set; }
    }
}
