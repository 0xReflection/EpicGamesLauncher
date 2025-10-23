using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Models.Interfaces
{
    public interface IDLC : IEntity
    {
        int GameId { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        DateTime? ReleaseDate { get; set; }
        decimal Price { get; set; }
        IGame Game { get; set; }
    }

}
