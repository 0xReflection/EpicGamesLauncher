using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Models.Interfaces
{
    public interface IPurchase : IEntity
    {
        int TransactionId { get; set; }
        int GameId { get; set; }
        int? DLCId { get; set; }
        decimal Price { get; set; }
        ITransaction Transaction { get; set; }
        IGame Game { get; set; }
        IDLC DLC { get; set; }
    }
}
