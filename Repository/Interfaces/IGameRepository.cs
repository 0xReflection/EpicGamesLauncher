using EpicGamesLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Repository.Interfaces
{
    public interface IGameRepository : IRepositoryBase<Game>
    {
        Task<IEnumerable<Game>> GetByGenreAsync(string genreSlug);
        Task<IEnumerable<Game>> GetByPlatformAsync(int platformId);
        Task<IEnumerable<Game>> SearchAsync(string searchTerm);
        Task<IEnumerable<Game>> GetFeaturedGamesAsync();
        Task<IEnumerable<Game>> GetGamesOnSaleAsync();
        Task<IEnumerable<Genre>> GetGameGenresAsync(int gameId);
        Task<IEnumerable<Platform>> GetGamePlatformsAsync(int gameId);
    }
}
