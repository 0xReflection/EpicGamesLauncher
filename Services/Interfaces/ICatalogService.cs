using EpicGamesLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<IEnumerable<Game>> GetAllGamesAsync();
        Task<IEnumerable<Game>> GetGamesByGenreAsync(string genreSlug);
        Task<IEnumerable<Game>> SearchGamesAsync(string searchTerm);
        Task<Game> GetGameByIdAsync(int id);
        Task<IEnumerable<Genre>> GetAllGenresAsync();
        Task<IEnumerable<Platform>> GetAllPlatformsAsync();
        Task<IEnumerable<DLC>> GetGameDLCsAsync(int gameId);
    }
}
