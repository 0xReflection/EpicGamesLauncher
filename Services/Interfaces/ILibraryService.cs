using EpicGamesLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Services.Interfaces
{
    public interface ILibraryService
    {
        Task<IEnumerable<Entitlement>> GetUserLibraryAsync(int userId);
        Task<bool> HasGameAsync(int userId, int gameId);
        Task<bool> HasDLCAsync(int userId, int dlcId);
    }
}
