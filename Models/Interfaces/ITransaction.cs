using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Models.Interfaces
{
    public interface ITransaction : IEntity
    {
        int UserId { get; set; }
        DateTime TransactionDate { get; set; }
        decimal TotalAmount { get; set; }
        ICollection<IPurchase> Purchases { get; set; }
        IUser User { get; set; }
    }

}
