using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Models.Interfaces
{
    public interface IRegisterRequest
    {
        string Username { get; set; }
        string Email { get; set; }
        string Password { get; set; }
    }
}
