using EpicGamesLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Services.Interfaces
{
    public interface IOrderService
    {
        Task<bool> PurchaseGameAsync(int userId, int gameId);
        Task<bool> PurchaseDLCAsync(int userId, int dlcId);
        Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId);
        Task<decimal> GetUserBalanceAsync(int userId);
    }
}
