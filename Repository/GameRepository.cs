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
    public class GameRepository : RepositoryBase<Game>, IGameRepository
    {
        public override async Task<Game> GetByIdAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT g.GameId, g.Title, g.Description, g.ReleaseDate, g.DeveloperId, g.PublisherId, 
                   g.CoverImage, g.Price, g.CreatedAt,
                   d.Name as DeveloperName, p.Name as PublisherName
            FROM Games g
            INNER JOIN Developers d ON g.DeveloperId = d.DeveloperId
            INNER JOIN Publishers p ON g.PublisherId = p.PublisherId
            WHERE g.GameId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var game = new Game
                {
                    GameId = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    ReleaseDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    DeveloperId = reader.GetInt32(4),
                    PublisherId = reader.GetInt32(5),
                    CoverImage = reader.IsDBNull(6) ? null : reader.GetString(6), 
                    Price = reader.GetDecimal(7),
                    CreatedAt = reader.GetDateTime(8),
                    Developer = new Developer { Name = reader.GetString(9) },
                    Publisher = new Publisher { Name = reader.GetString(10) }
                };

                // Load genres and platforms
                game.Genres = (await GetGameGenresAsync(id)).ToList();
                game.Platforms = (await GetGamePlatformsAsync(id)).ToList();

                return game;
            }
            return null;
        }

        public override async Task<IEnumerable<Game>> GetAllAsync()
        {
            var games = new List<Game>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT g.GameId, g.Title, g.Description, g.ReleaseDate, g.DeveloperId, g.PublisherId, 
                   g.CoverImage, g.Price, g.CreatedAt,
                   d.Name as DeveloperName, p.Name as PublisherName
            FROM Games g
            INNER JOIN Developers d ON g.DeveloperId = d.DeveloperId
            INNER JOIN Publishers p ON g.PublisherId = p.PublisherId";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var game = new Game
                {
                    GameId = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    ReleaseDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    DeveloperId = reader.GetInt32(4),
                    PublisherId = reader.GetInt32(5),
                    CoverImage = reader.IsDBNull(6) ? null : reader.GetString(6), // Изменено
                    Price = reader.GetDecimal(7),
                    CreatedAt = reader.GetDateTime(8),
                    Developer = new Developer { Name = reader.GetString(9) },
                    Publisher = new Publisher { Name = reader.GetString(10) }
                };
                games.Add(game);
            }

            foreach (var game in games)
            {
                game.Genres = (await GetGameGenresAsync(game.GameId)).ToList();
                game.Platforms = (await GetGamePlatformsAsync(game.GameId)).ToList();
            }

            return games;
        }

        public async Task<IEnumerable<Game>> GetByGenreAsync(string genreSlug)
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
            INNER JOIN GameGenres gg ON g.GameId = gg.GameId
            INNER JOIN Genres gen ON gg.GenreId = gen.GenreId
            WHERE gen.Slug = @GenreSlug";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@GenreSlug", genreSlug);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var game = new Game
                {
                    GameId = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    ReleaseDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    DeveloperId = reader.GetInt32(4),
                    PublisherId = reader.GetInt32(5),
                    CoverImage = reader.IsDBNull(6) ? null : reader.GetString(6), // Изменено
                    Price = reader.GetDecimal(7),
                    CreatedAt = reader.GetDateTime(8),
                    Developer = new Developer { Name = reader.GetString(9) },
                    Publisher = new Publisher { Name = reader.GetString(10) }
                };
                games.Add(game);
            }

            foreach (var game in games)
            {
                game.Genres = (await GetGameGenresAsync(game.GameId)).ToList();
                game.Platforms = (await GetGamePlatformsAsync(game.GameId)).ToList();
            }

            return games;
        }

        public async Task<IEnumerable<Game>> GetByPlatformAsync(int platformId)
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
            INNER JOIN GamePlatforms gp ON g.GameId = gp.GameId
            WHERE gp.PlatformId = @PlatformId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PlatformId", platformId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var game = new Game
                {
                    GameId = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    ReleaseDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    DeveloperId = reader.GetInt32(4),
                    PublisherId = reader.GetInt32(5),
                    CoverImage = reader.IsDBNull(6) ? null : reader.GetString(6), // Изменено
                    Price = reader.GetDecimal(7),
                    CreatedAt = reader.GetDateTime(8),
                    Developer = new Developer { Name = reader.GetString(9) },
                    Publisher = new Publisher { Name = reader.GetString(10) }
                };
                games.Add(game);
            }

            foreach (var game in games)
            {
                game.Genres = (await GetGameGenresAsync(game.GameId)).ToList();
                game.Platforms = (await GetGamePlatformsAsync(game.GameId)).ToList();
            }

            return games;
        }

        public async Task<IEnumerable<Game>> SearchAsync(string searchTerm)
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
            WHERE g.Title LIKE @SearchTerm OR g.Description LIKE @SearchTerm";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var game = new Game
                {
                    GameId = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    ReleaseDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    DeveloperId = reader.GetInt32(4),
                    PublisherId = reader.GetInt32(5),
                    CoverImage = reader.IsDBNull(6) ? null : reader.GetString(6), // Изменено
                    Price = reader.GetDecimal(7),
                    CreatedAt = reader.GetDateTime(8),
                    Developer = new Developer { Name = reader.GetString(9) },
                    Publisher = new Publisher { Name = reader.GetString(10) }
                };
                games.Add(game);
            }

            foreach (var game in games)
            {
                game.Genres = (await GetGameGenresAsync(game.GameId)).ToList();
                game.Platforms = (await GetGamePlatformsAsync(game.GameId)).ToList();
            }

            return games;
        }

        public async Task<IEnumerable<Game>> GetFeaturedGamesAsync()
        {
            var allGames = await GetAllAsync();
            return allGames.Take(6);
        }

        public async Task<IEnumerable<Game>> GetGamesOnSaleAsync()
        {
            var allGames = await GetAllAsync();
            return allGames.Where(g => g.Price > 0);
        }

        public async Task<IEnumerable<Genre>> GetGameGenresAsync(int gameId)
        {
            var genres = new List<Genre>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT g.GenreId, g.Name, g.Slug, g.Description 
            FROM Genres g
            INNER JOIN GameGenres gg ON g.GenreId = gg.GenreId
            WHERE gg.GameId = @GameId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@GameId", gameId);

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

        public async Task<IEnumerable<Platform>> GetGamePlatformsAsync(int gameId)
        {
            var platforms = new List<Platform>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT p.PlatformId, p.Name, p.Description 
            FROM Platforms p
            INNER JOIN GamePlatforms gp ON p.PlatformId = gp.PlatformId
            WHERE gp.GameId = @GameId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@GameId", gameId);

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

        public override async Task<int> CreateAsync(Game game)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            INSERT INTO Games (Title, Description, ReleaseDate, DeveloperId, PublisherId, CoverImage, Price)
            OUTPUT INSERTED.GameId
            VALUES (@Title, @Description, @ReleaseDate, @DeveloperId, @PublisherId, @CoverImage, @Price)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Title", game.Title);
            command.Parameters.AddWithValue("@Description", game.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ReleaseDate", game.ReleaseDate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@DeveloperId", game.DeveloperId);
            command.Parameters.AddWithValue("@PublisherId", game.PublisherId);
            command.Parameters.AddWithValue("@CoverImage", game.CoverImage ?? (object)DBNull.Value); // Изменено
            command.Parameters.AddWithValue("@Price", game.Price);

            return (int)await command.ExecuteScalarAsync();
        }

        public override async Task<bool> UpdateAsync(Game game)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            UPDATE Games 
            SET Title = @Title, Description = @Description, ReleaseDate = @ReleaseDate,
                DeveloperId = @DeveloperId, PublisherId = @PublisherId, 
                CoverImage = @CoverImage, Price = @Price
            WHERE GameId = @GameId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@GameId", game.GameId);
            command.Parameters.AddWithValue("@Title", game.Title);
            command.Parameters.AddWithValue("@Description", game.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ReleaseDate", game.ReleaseDate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@DeveloperId", game.DeveloperId);
            command.Parameters.AddWithValue("@PublisherId", game.PublisherId);
            command.Parameters.AddWithValue("@CoverImage", game.CoverImage ?? (object)DBNull.Value); // Изменено
            command.Parameters.AddWithValue("@Price", game.Price);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "DELETE FROM Games WHERE GameId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> ExistsAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT COUNT(*) FROM Games WHERE GameId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return (int)await command.ExecuteScalarAsync() > 0;
        }
    }
}
