using EpicGamesLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Repository.Interfaces
{
    public interface IGenreRepository : IRepositoryBase<Genre>
    {
        Task<Genre> GetBySlugAsync(string slug);
        Task<IEnumerable<Genre>> GetPopularGenresAsync(int count = 10);
    }
}
