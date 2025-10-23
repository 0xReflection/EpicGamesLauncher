using EpicGamesLauncher.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Repository.Interfaces
{
    public interface IPurchaseRepository : IRepositoryBase<Purchase>
    {
        Task<IEnumerable<Purchase>> GetByTransactionIdAsync(int transactionId);
        Task<IEnumerable<Purchase>> GetUserPurchasesAsync(int userId);
    }
}
