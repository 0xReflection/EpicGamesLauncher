using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Models.Interfaces
{
    public interface IEntitlement : IEntity
    {
        int UserId { get; set; }
        int GameId { get; set; }
        int? DLCId { get; set; }
        DateTime AcquiredAt { get; set; }
        IUser User { get; set; }
        IGame Game { get; set; }
        IDLC DLC { get; set; }
    }
}
