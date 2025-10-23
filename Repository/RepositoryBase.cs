using EpicGamesLauncher.Models;
using EpicGamesLauncher.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Repository
{ 
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly string _connectionString;

        public RepositoryBase()
        {
            _connectionString = "Server=HOME-PC\\SQLEXPRESS; Database=GameStoreDB; Integrated Security=true";
        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        protected async Task<SqlConnection> CreateConnectionAsync()
        {
            var connection = GetConnection();
            await connection.OpenAsync();
            return connection;
        }

        public abstract Task<T> GetByIdAsync(int id);
        public abstract Task<IEnumerable<T>> GetAllAsync();
        public abstract Task<int> CreateAsync(T entity);
        public abstract Task<bool> UpdateAsync(T entity);
        public abstract Task<bool> DeleteAsync(int id);
        public abstract Task<bool> ExistsAsync(int id);
    }
}
