using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Models.Interfaces
{
    public interface IClientRepository 
    {
        bool AuthenticateUser(NetworkCredential credential);
        ClientModel GetById(int id);
        ClientModel GetByUsername(string username);
        IEnumerable<ClientModel> GetByAll();
    }
}
