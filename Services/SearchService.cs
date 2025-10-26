using EpicGamesLauncher.Logger.Interface;
using EpicGamesLauncher.Models;
using EpicGamesLauncher.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Services
{
    public class SearchService : ISearchService
    {
        private readonly ICatalogService _catalogService;
        private readonly ILibraryService _libraryService;
        private readonly ILogger<SearchService> _logger;

        public SearchService(
            ICatalogService catalogService,
            ILibraryService libraryService,
            ILogger<SearchService> logger)
        {
            _catalogService = catalogService;
            _libraryService = libraryService;
            _logger = logger;
        }

        public async Task<IEnumerable<Game>> SearchGamesAsync(string searchTerm, IEnumerable<Game> games)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return games;

                var searchLower = searchTerm.ToLower();
                return games.Where(game =>
                    game.Title?.ToLower().Contains(searchLower) == true ||
                    game.Description?.ToLower().Contains(searchLower) == true ||
                    game.Developer?.Name?.ToLower().Contains(searchLower) == true ||
                    game.Publisher?.Name?.ToLower().Contains(searchLower) == true ||
                    game.Genres.Any(g => g.Name?.ToLower().Contains(searchLower) == true) ||
                    game.Platforms.Any(p => p.Name?.ToLower().Contains(searchLower) == true)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching in games collection with term: {searchTerm}");
                return games;
            }
        }

        public async Task<IEnumerable<Game>> SearchInCatalogAsync(string searchTerm)
        {
            try
            {
                return await _catalogService.SearchGamesAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching in catalog with term: {searchTerm}");
                return new List<Game>();
            }
        }

        public async Task<IEnumerable<Game>> SearchInLibraryAsync(string searchTerm, int userId)
        {
            try
            {
                var entitlements = await _libraryService.GetUserLibraryAsync(userId);
                var libraryGames = entitlements.Where(e => e.Game != null).Select(e => e.Game);

                return await SearchGamesAsync(searchTerm, libraryGames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching in library for user {userId} with term: {searchTerm}");
                return new List<Game>();
            }
        }
    }
}