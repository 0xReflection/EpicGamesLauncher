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
    public class DLCRepository : RepositoryBase<DLC>, IDLCRepository
    {
        public override async Task<DLC> GetByIdAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT DLCId, GameId, Title, Description, ReleaseDate, Price
            FROM DLCs 
            WHERE DLCId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new DLC
                {
                    DLCId = reader.GetInt32(0),
                    GameId = reader.GetInt32(1),
                    Title = reader.GetString(2),
                    Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                    ReleaseDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                    Price = reader.GetDecimal(5)
                };
            }
            return null;
        }

        public async Task<IEnumerable<DLC>> GetByGameIdAsync(int gameId)
        {
            var dlcs = new List<DLC>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT DLCId, GameId, Title, Description, ReleaseDate, Price
            FROM DLCs 
            WHERE GameId = @GameId
            ORDER BY ReleaseDate DESC";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@GameId", gameId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                dlcs.Add(new DLC
                {
                    DLCId = reader.GetInt32(0),
                    GameId = reader.GetInt32(1),
                    Title = reader.GetString(2),
                    Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                    ReleaseDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                    Price = reader.GetDecimal(5)
                });
            }
            return dlcs;
        }

        public async Task<IEnumerable<DLC>> GetUserDLCAccessAsync(int userId, int gameId)
        {
            var dlcs = new List<DLC>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT d.DLCId, d.GameId, d.Title, d.Description, d.ReleaseDate, d.Price
            FROM DLCs d
            INNER JOIN Entitlements e ON d.DLCId = e.DLCId
            WHERE e.UserId = @UserId AND d.GameId = @GameId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@GameId", gameId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                dlcs.Add(new DLC
                {
                    DLCId = reader.GetInt32(0),
                    GameId = reader.GetInt32(1),
                    Title = reader.GetString(2),
                    Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                    ReleaseDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                    Price = reader.GetDecimal(5)
                });
            }
            return dlcs;
        }

        public async Task<IEnumerable<DLC>> GetUserDLCsAsync(int userId, int gameId)
        {
            return await GetUserDLCAccessAsync(userId, gameId);
        }

        public override async Task<IEnumerable<DLC>> GetAllAsync()
        {
            var dlcs = new List<DLC>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT DLCId, GameId, Title, Description, ReleaseDate, Price
            FROM DLCs 
            ORDER BY Title";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                dlcs.Add(new DLC
                {
                    DLCId = reader.GetInt32(0),
                    GameId = reader.GetInt32(1),
                    Title = reader.GetString(2),
                    Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                    ReleaseDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                    Price = reader.GetDecimal(5)
                });
            }
            return dlcs;
        }

        public override async Task<int> CreateAsync(DLC dlc)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            INSERT INTO DLCs (GameId, Title, Description, ReleaseDate, Price)
            OUTPUT INSERTED.DLCId
            VALUES (@GameId, @Title, @Description, @ReleaseDate, @Price)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@GameId", dlc.GameId);
            command.Parameters.AddWithValue("@Title", dlc.Title);
            command.Parameters.AddWithValue("@Description", dlc.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ReleaseDate", dlc.ReleaseDate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Price", dlc.Price);

            return (int)await command.ExecuteScalarAsync();
        }

        public override async Task<bool> UpdateAsync(DLC dlc)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            UPDATE DLCs 
            SET GameId = @GameId, Title = @Title, Description = @Description, 
                ReleaseDate = @ReleaseDate, Price = @Price
            WHERE DLCId = @DLCId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DLCId", dlc.DLCId);
            command.Parameters.AddWithValue("@GameId", dlc.GameId);
            command.Parameters.AddWithValue("@Title", dlc.Title);
            command.Parameters.AddWithValue("@Description", dlc.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ReleaseDate", dlc.ReleaseDate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Price", dlc.Price);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "DELETE FROM DLCs WHERE DLCId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> ExistsAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT COUNT(*) FROM DLCs WHERE DLCId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return (int)await command.ExecuteScalarAsync() > 0;
        }
    }
}
