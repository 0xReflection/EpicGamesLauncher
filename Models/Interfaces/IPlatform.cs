using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Models.Interfaces
{
    public interface IPlatform : IEntity
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}
