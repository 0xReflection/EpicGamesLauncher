using EpicGamesLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Services.Interfaces
{
    public interface ISearchService
    {
        Task<IEnumerable<Game>> SearchGamesAsync(string searchTerm, IEnumerable<Game> games);
        Task<IEnumerable<Game>> SearchInCatalogAsync(string searchTerm);
        Task<IEnumerable<Game>> SearchInLibraryAsync(string searchTerm, int userId);
    }
}
