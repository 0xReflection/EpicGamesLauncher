using EpicGamesLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(string username, string email, string password);
        Task<bool> UserExistsAsync(string username);
        Task<decimal> GetUserBalanceAsync(int userId);
    }

}
