using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Models.Interfaces
{
    public interface IUser : IEntity
    {
        string Username { get; set; }
        string Email { get; set; }
        byte[] PasswordHash { get; set; }
        byte[] PasswordSalt { get; set; }
        byte[] Avatar { get; set; }
        decimal Balance { get; set; }
        DateTime CreatedAt { get; set; }
    }
}
