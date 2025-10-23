using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Models
{
    public class Entitlement
    {
        public int EntitlementId { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public int? DLCId { get; set; }
        public DateTime AcquiredAt { get; set; }
        public Game Game { get; set; }
        public DLC DLC { get; set; }
    }

}
