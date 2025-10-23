using EpicGamesLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserProfileAsync(int userId);
        Task<bool> UpdateUserProfileAsync(User user);
        Task<bool> UpdateUserBalanceAsync(int userId, decimal newBalance);
    }
}
