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
    public class PlatformRepository : RepositoryBase<Platform>, IPlatformRepository
    {
        public override async Task<Platform> GetByIdAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT PlatformId, Name, Description FROM Platforms WHERE PlatformId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Platform
                {
                    PlatformId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                };
            }
            return null;
        }

        public async Task<Platform> GetByNameAsync(string name)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT PlatformId, Name, Description FROM Platforms WHERE Name = @Name";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", name);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Platform
                {
                    PlatformId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                };
            }
            return null;
        }

        public override async Task<IEnumerable<Platform>> GetAllAsync()
        {
            var platforms = new List<Platform>();
            using var connection = await CreateConnectionAsync();
            var query = "SELECT PlatformId, Name, Description FROM Platforms ORDER BY Name";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                platforms.Add(new Platform
                {
                    PlatformId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                });
            }
            return platforms;
        }

        public override async Task<int> CreateAsync(Platform platform)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            INSERT INTO Platforms (Name, Description)
            OUTPUT INSERTED.PlatformId
            VALUES (@Name, @Description)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", platform.Name);
            command.Parameters.AddWithValue("@Description", platform.Description ?? (object)DBNull.Value);

            return (int)await command.ExecuteScalarAsync();
        }

        public override async Task<bool> UpdateAsync(Platform platform)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            UPDATE Platforms 
            SET Name = @Name, Description = @Description
            WHERE PlatformId = @PlatformId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PlatformId", platform.PlatformId);
            command.Parameters.AddWithValue("@Name", platform.Name);
            command.Parameters.AddWithValue("@Description", platform.Description ?? (object)DBNull.Value);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "DELETE FROM Platforms WHERE PlatformId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> ExistsAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT COUNT(*) FROM Platforms WHERE PlatformId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return (int)await command.ExecuteScalarAsync() > 0;
        }
    }
}
