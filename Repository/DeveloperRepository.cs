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
    public class DeveloperRepository : RepositoryBase<Developer>, IDeveloperRepository
    {
        public override async Task<Developer> GetByIdAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT DeveloperId, Name, Description FROM Developers WHERE DeveloperId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Developer
                {
                    DeveloperId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                };
            }
            return null;
        }

        public async Task<Developer> GetByNameAsync(string name)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT DeveloperId, Name, Description FROM Developers WHERE Name = @Name";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", name);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Developer
                {
                    DeveloperId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                };
            }
            return null;
        }

        public override async Task<IEnumerable<Developer>> GetAllAsync()
        {
            var developers = new List<Developer>();
            using var connection = await CreateConnectionAsync();
            var query = "SELECT DeveloperId, Name, Description FROM Developers ORDER BY Name";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                developers.Add(new Developer
                {
                    DeveloperId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                });
            }
            return developers;
        }

        public async Task<IEnumerable<Game>> GetDeveloperGamesAsync(int developerId)
        {
            var games = new List<Game>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT g.GameId, g.Title, g.Description, g.ReleaseDate, g.DeveloperId, g.PublisherId, 
                   g.CoverImage, g.Price, g.CreatedAt,
                   d.Name as DeveloperName, p.Name as PublisherName
            FROM Games g
            INNER JOIN Developers d ON g.DeveloperId = d.DeveloperId
            INNER JOIN Publishers pub ON g.PublisherId = pub.PublisherId
            WHERE g.DeveloperId = @DeveloperId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DeveloperId", developerId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                games.Add(new Game
                {
                    GameId = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    ReleaseDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    DeveloperId = reader.GetInt32(4),
                    PublisherId = reader.GetInt32(5),
                   // CoverImage = reader[6] as byte[],
                    Price = reader.GetDecimal(7),
                    CreatedAt = reader.GetDateTime(8),
                    Developer = new Developer { Name = reader.GetString(9) },
                    Publisher = new Publisher { Name = reader.GetString(10) }
                });
            }
            return games;
        }

        public override async Task<int> CreateAsync(Developer developer)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            INSERT INTO Developers (Name, Description)
            OUTPUT INSERTED.DeveloperId
            VALUES (@Name, @Description)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", developer.Name);
            command.Parameters.AddWithValue("@Description", developer.Description ?? (object)DBNull.Value);

            return (int)await command.ExecuteScalarAsync();
        }

        public override async Task<bool> UpdateAsync(Developer developer)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            UPDATE Developers 
            SET Name = @Name, Description = @Description
            WHERE DeveloperId = @DeveloperId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DeveloperId", developer.DeveloperId);
            command.Parameters.AddWithValue("@Name", developer.Name);
            command.Parameters.AddWithValue("@Description", developer.Description ?? (object)DBNull.Value);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "DELETE FROM Developers WHERE DeveloperId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> ExistsAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT COUNT(*) FROM Developers WHERE DeveloperId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return (int)await command.ExecuteScalarAsync() > 0;
        }
    }
}
