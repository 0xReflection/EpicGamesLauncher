using EpicGamesLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Repository.Interfaces
{
    public interface IDeveloperRepository : IRepositoryBase<Developer>
    {
        Task<Developer> GetByNameAsync(string name);
        Task<IEnumerable<Game>> GetDeveloperGamesAsync(int developerId);
    }

}
