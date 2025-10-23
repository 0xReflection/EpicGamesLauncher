using EpicGamesLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Repository.Interfaces
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<bool> UserExistsAsync(string username, string email);
        Task<bool> UpdateBalanceAsync(int userId, decimal newBalance);
        Task<decimal> GetUserBalanceAsync(int userId);
    }
}
