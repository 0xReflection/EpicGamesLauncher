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
    public class PublisherRepository : RepositoryBase<Publisher>, IPublisherRepository
    {
        public override async Task<Publisher> GetByIdAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT PublisherId, Name, Description FROM Publishers WHERE PublisherId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Publisher
                {
                    PublisherId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                };
            }
            return null;
        }

        public async Task<Publisher> GetByNameAsync(string name)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT PublisherId, Name, Description FROM Publishers WHERE Name = @Name";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", name);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Publisher
                {
                    PublisherId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                };
            }
            return null;
        }

        public override async Task<IEnumerable<Publisher>> GetAllAsync()
        {
            var publishers = new List<Publisher>();
            using var connection = await CreateConnectionAsync();
            var query = "SELECT PublisherId, Name, Description FROM Publishers ORDER BY Name";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                publishers.Add(new Publisher
                {
                    PublisherId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                });
            }
            return publishers;
        }

        public async Task<IEnumerable<Game>> GetPublisherGamesAsync(int publisherId)
        {
            var games = new List<Game>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT g.GameId, g.Title, g.Description, g.ReleaseDate, g.DeveloperId, g.PublisherId, 
                   g.CoverImage, g.Price, g.CreatedAt,
                   d.Name as DeveloperName, p.Name as PublisherName
            FROM Games g
            INNER JOIN Developers d ON g.DeveloperId = d.DeveloperId
            INNER JOIN Publishers p ON g.PublisherId = p.PublisherId
            WHERE g.PublisherId = @PublisherId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PublisherId", publisherId);

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
                    CoverImage = reader[6] as byte[],
                    Price = reader.GetDecimal(7),
                    CreatedAt = reader.GetDateTime(8),
                    Developer = new Developer { Name = reader.GetString(9) },
                    Publisher = new Publisher { Name = reader.GetString(10) }
                });
            }
            return games;
        }

        public override async Task<int> CreateAsync(Publisher publisher)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            INSERT INTO Publishers (Name, Description)
            OUTPUT INSERTED.PublisherId
            VALUES (@Name, @Description)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", publisher.Name);
            command.Parameters.AddWithValue("@Description", publisher.Description ?? (object)DBNull.Value);

            return (int)await command.ExecuteScalarAsync();
        }

        public override async Task<bool> UpdateAsync(Publisher publisher)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            UPDATE Publishers 
            SET Name = @Name, Description = @Description
            WHERE PublisherId = @PublisherId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PublisherId", publisher.PublisherId);
            command.Parameters.AddWithValue("@Name", publisher.Name);
            command.Parameters.AddWithValue("@Description", publisher.Description ?? (object)DBNull.Value);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "DELETE FROM Publishers WHERE PublisherId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> ExistsAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT COUNT(*) FROM Publishers WHERE PublisherId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return (int)await command.ExecuteScalarAsync() > 0;
        }
    }
}
