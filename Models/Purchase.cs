using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Models
{
    public class Purchase
    {
        public int PurchaseId { get; set; }
        public int TransactionId { get; set; }
        public int GameId { get; set; }
        public int? DLCId { get; set; }
        public decimal Price { get; set; }
        public Game Game { get; set; }
        public DLC DLC { get; set; }
    }
   
}
