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
    public class EntitlementRepository : RepositoryBase<Entitlement>, IEntitlementRepository
    {
        public override async Task<Entitlement> GetByIdAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT EntitlementId, UserId, GameId, DLCId, AcquiredAt
            FROM Entitlements 
            WHERE EntitlementId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Entitlement
                {
                    EntitlementId = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    GameId = reader.GetInt32(2),
                    DLCId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    AcquiredAt = reader.GetDateTime(4)
                };
            }
            return null;
        }

        public async Task<IEnumerable<Entitlement>> GetUserLibraryAsync(int userId)
        {
            var entitlements = new List<Entitlement>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT e.EntitlementId, e.UserId, e.GameId, e.DLCId, e.AcquiredAt,
                   g.Title as GameTitle
            FROM Entitlements e
            LEFT JOIN Games g ON e.GameId = g.GameId
            WHERE e.UserId = @UserId
            ORDER BY e.AcquiredAt DESC";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                entitlements.Add(new Entitlement
                {
                    EntitlementId = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    GameId = reader.GetInt32(2),
                    DLCId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    AcquiredAt = reader.GetDateTime(4),
                    Game = new Game { Title = reader.GetString(5) }
                });
            }
            return entitlements;
        }

        public async Task<bool> HasGameAsync(int userId, int gameId)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT COUNT(*) FROM Entitlements 
            WHERE UserId = @UserId AND GameId = @GameId AND DLCId IS NULL";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@GameId", gameId);

            return (int)await command.ExecuteScalarAsync() > 0;
        }

        public async Task<bool> HasDLCAsync(int userId, int dlcId)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT COUNT(*) FROM Entitlements WHERE UserId = @UserId AND DLCId = @DLCId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@DLCId", dlcId);

            return (int)await command.ExecuteScalarAsync() > 0;
        }

        public async Task<bool> AddGameToLibraryAsync(int userId, int gameId)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            INSERT INTO Entitlements (UserId, GameId, AcquiredAt)
            VALUES (@UserId, @GameId, GETDATE())";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@GameId", gameId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> AddDLCToLibraryAsync(int userId, int dlcId)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            INSERT INTO Entitlements (UserId, DLCId, AcquiredAt)
            VALUES (@UserId, @DLCId, GETDATE())";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@DLCId", dlcId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<IEnumerable<Entitlement>> GetAllAsync()
        {
            var entitlements = new List<Entitlement>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT EntitlementId, UserId, GameId, DLCId, AcquiredAt
            FROM Entitlements 
            ORDER BY AcquiredAt DESC";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                entitlements.Add(new Entitlement
                {
                    EntitlementId = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    GameId = reader.GetInt32(2),
                    DLCId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    AcquiredAt = reader.GetDateTime(4)
                });
            }
            return entitlements;
        }

        public override async Task<int> CreateAsync(Entitlement entitlement)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            INSERT INTO Entitlements (UserId, GameId, DLCId, AcquiredAt)
            OUTPUT INSERTED.EntitlementId
            VALUES (@UserId, @GameId, @DLCId, @AcquiredAt)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", entitlement.UserId);
            command.Parameters.AddWithValue("@GameId", entitlement.GameId);
            command.Parameters.AddWithValue("@DLCId", entitlement.DLCId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@AcquiredAt", entitlement.AcquiredAt);

            return (int)await command.ExecuteScalarAsync();
        }

        public override async Task<bool> UpdateAsync(Entitlement entitlement)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            UPDATE Entitlements 
            SET UserId = @UserId, GameId = @GameId, DLCId = @DLCId, AcquiredAt = @AcquiredAt
            WHERE EntitlementId = @EntitlementId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EntitlementId", entitlement.EntitlementId);
            command.Parameters.AddWithValue("@UserId", entitlement.UserId);
            command.Parameters.AddWithValue("@GameId", entitlement.GameId);
            command.Parameters.AddWithValue("@DLCId", entitlement.DLCId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@AcquiredAt", entitlement.AcquiredAt);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "DELETE FROM Entitlements WHERE EntitlementId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> ExistsAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT COUNT(*) FROM Entitlements WHERE EntitlementId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return (int)await command.ExecuteScalarAsync() > 0;
        }
    }
}
