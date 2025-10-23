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
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public override async Task<User> GetByIdAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT UserId, Username, Email, PasswordHash, PasswordSalt, Avatar, Balance, CreatedAt
            FROM Users WHERE UserId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(0), 
                    Username = reader.GetString(1), 
                    Email = reader.GetString(2), 
                    PasswordHash = reader[3] as byte[], 
                    PasswordSalt = reader[4] as byte[], 
                    Avatar = reader[5] as byte[], 
                    Balance = reader.GetDecimal(6), 
                    CreatedAt = reader.GetDateTime(7) 
                };
            }
            return null;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT UserId, Username, Email, PasswordHash, PasswordSalt, Avatar, Balance, CreatedAt
            FROM Users WHERE Username = @Username";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader[3] as byte[],
                    PasswordSalt = reader[4] as byte[],
                    Avatar = reader[5] as byte[],
                    Balance = reader.GetDecimal(6),
                    CreatedAt = reader.GetDateTime(7)
                };
            }
            return null;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT UserId, Username, Email, PasswordHash, PasswordSalt, Avatar, Balance, CreatedAt
            FROM Users WHERE Email = @Email";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader[3] as byte[],
                    PasswordSalt = reader[4] as byte[],
                    Avatar = reader[5] as byte[],
                    Balance = reader.GetDecimal(6),
                    CreatedAt = reader.GetDateTime(7)
                };
            }
            return null;
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT COUNT(*) FROM Users 
            WHERE Username = @Username OR Email = @Email";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Email", email);

            var count = (int)await command.ExecuteScalarAsync();
            return count > 0;
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = new List<User>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT UserId, Username, Email, PasswordHash, PasswordSalt, Avatar, Balance, CreatedAt
            FROM Users";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader[3] as byte[],
                    PasswordSalt = reader[4] as byte[],
                    Avatar = reader[5] as byte[],
                    Balance = reader.GetDecimal(6),
                    CreatedAt = reader.GetDateTime(7)
                });
            }
            return users;
        }

        public override async Task<int> CreateAsync(User user)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, Balance)
            OUTPUT INSERTED.UserId
            VALUES (@Username, @Email, @PasswordHash, @PasswordSalt, @Balance)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PasswordSalt", user.PasswordSalt ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Balance", user.Balance);

            return (int)await command.ExecuteScalarAsync();
        }

        public override async Task<bool> UpdateAsync(User user)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            UPDATE Users 
            SET Username = @Username, Email = @Email, 
                PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt,
                Avatar = @Avatar, Balance = @Balance
            WHERE UserId = @UserId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", user.UserId);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PasswordSalt", user.PasswordSalt ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Avatar", user.Avatar ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Balance", user.Balance);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateBalanceAsync(int userId, decimal newBalance)
        {
            using var connection = await CreateConnectionAsync();
            var query = "UPDATE Users SET Balance = @Balance WHERE UserId = @UserId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Balance", newBalance);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<decimal> GetUserBalanceAsync(int userId)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT Balance FROM Users WHERE UserId = @UserId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            var result = await command.ExecuteScalarAsync();
            return result != DBNull.Value ? (decimal)result : 0;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "DELETE FROM Users WHERE UserId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> ExistsAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT COUNT(*) FROM Users WHERE UserId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return (int)await command.ExecuteScalarAsync() > 0;
        }
    }
}
