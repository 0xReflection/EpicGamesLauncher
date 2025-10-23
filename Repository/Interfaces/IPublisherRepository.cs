using EpicGamesLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Repository.Interfaces
{
    public interface IPublisherRepository : IRepositoryBase<Publisher>
    {
        Task<Publisher> GetByNameAsync(string name);
        Task<IEnumerable<Game>> GetPublisherGamesAsync(int publisherId);
    }
}
