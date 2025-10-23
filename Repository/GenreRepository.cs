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
    public class GenreRepository : RepositoryBase<Genre>, IGenreRepository
    {
        public override async Task<Genre> GetByIdAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT GenreId, Name, Slug, Description FROM Genres WHERE GenreId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Genre
                {
                    GenreId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Slug = reader.GetString(2),
                    Description = reader.IsDBNull(3) ? null : reader.GetString(3)
                };
            }
            return null;
        }

        public async Task<Genre> GetBySlugAsync(string slug)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT GenreId, Name, Slug, Description FROM Genres WHERE Slug = @Slug";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Slug", slug);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Genre
                {
                    GenreId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Slug = reader.GetString(2),
                    Description = reader.IsDBNull(3) ? null : reader.GetString(3)
                };
            }
            return null;
        }

        public override async Task<IEnumerable<Genre>> GetAllAsync()
        {
            var genres = new List<Genre>();
            using var connection = await CreateConnectionAsync();
            var query = "SELECT GenreId, Name, Slug, Description FROM Genres ORDER BY Name";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                genres.Add(new Genre
                {
                    GenreId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Slug = reader.GetString(2),
                    Description = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }
            return genres;
        }

        public async Task<IEnumerable<Genre>> GetPopularGenresAsync(int count = 10)
        {
            var genres = new List<Genre>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT TOP (@Count) g.GenreId, g.Name, g.Slug, g.Description, 
                   COUNT(gg.GameId) as GameCount
            FROM Genres g
            LEFT JOIN GameGenres gg ON g.GenreId = gg.GenreId
            GROUP BY g.GenreId, g.Name, g.Slug, g.Description
            ORDER BY GameCount DESC, g.Name";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Count", count);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                genres.Add(new Genre
                {
                    GenreId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Slug = reader.GetString(2),
                    Description = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }
            return genres;
        }

        public override async Task<int> CreateAsync(Genre genre)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            INSERT INTO Genres (Name, Slug, Description)
            OUTPUT INSERTED.GenreId
            VALUES (@Name, @Slug, @Description)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", genre.Name);
            command.Parameters.AddWithValue("@Slug", genre.Slug);
            command.Parameters.AddWithValue("@Description", genre.Description ?? (object)DBNull.Value);

            return (int)await command.ExecuteScalarAsync();
        }

        public override async Task<bool> UpdateAsync(Genre genre)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            UPDATE Genres 
            SET Name = @Name, Slug = @Slug, Description = @Description
            WHERE GenreId = @GenreId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@GenreId", genre.GenreId);
            command.Parameters.AddWithValue("@Name", genre.Name);
            command.Parameters.AddWithValue("@Slug", genre.Slug);
            command.Parameters.AddWithValue("@Description", genre.Description ?? (object)DBNull.Value);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "DELETE FROM Genres WHERE GenreId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> ExistsAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT COUNT(*) FROM Genres WHERE GenreId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return (int)await command.ExecuteScalarAsync() > 0;
        }
    }
}
