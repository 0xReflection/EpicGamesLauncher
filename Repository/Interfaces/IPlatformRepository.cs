using EpicGamesLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Repository.Interfaces
{
    public interface IPlatformRepository : IRepositoryBase<Platform>
    {
        Task<Platform> GetByNameAsync(string name);
    }
}
