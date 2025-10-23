using EpicGamesLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Repository.Interfaces
{
    public interface IEntitlementRepository : IRepositoryBase<Entitlement>
    {
        Task<IEnumerable<Entitlement>> GetUserLibraryAsync(int userId);
        Task<bool> HasGameAsync(int userId, int gameId);
        Task<bool> HasDLCAsync(int userId, int dlcId);
        Task<bool> AddGameToLibraryAsync(int userId, int gameId);
        Task<bool> AddDLCToLibraryAsync(int userId, int dlcId);
    }
}
