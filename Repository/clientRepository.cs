using EpicGamesLauncher.Models;
using EpicGamesLauncher.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Repository
{
    public class clientRepository : RepositoryBase, IClientRepository
    {
        public bool AuthenticateUser(NetworkCredential credential)
        {
            bool validUser;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "select count(*) from [Users] where username=@username and [password]=@password";
                command.Parameters.Add("@username", SqlDbType.NVarChar).Value = credential.UserName;
                command.Parameters.Add("@password", SqlDbType.NVarChar).Value = credential.Password;
                validUser = command.ExecuteScalar() == null ? false : true;
            }
            return validUser;
        }

     
        public ClientModel GetByUsername(string username)
        {
            ClientModel user = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "select * from [Users] where username=@username";
                command.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new ClientModel()
                        {
                            Username = reader["Username"].ToString(),
                            Password = string.Empty,
                            Email = reader["Email"].ToString()
                        };
                    }
                }
            }
            return user;
        }
        public IEnumerable<ClientModel> GetByAll()
        {
            throw new NotImplementedException();
        }
        public ClientModel GetById(int id)
        {
            throw new NotImplementedException();
        }
      
    }
}
