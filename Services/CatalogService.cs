using EpicGamesLauncher.Logger.Interface;
using EpicGamesLauncher.Models;
using EpicGamesLauncher.Repository.Interfaces;
using EpicGamesLauncher.Services.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IPlatformRepository _platformRepository;
        private readonly IDLCRepository _dlcRepository;
        private readonly ILogger<CatalogService> _logger;

        public CatalogService(
            IGameRepository gameRepository,
            IGenreRepository genreRepository,
            IPlatformRepository platformRepository,
            IDLCRepository dlcRepository,
            ILogger<CatalogService> logger)
        {
            _gameRepository = gameRepository;
            _genreRepository = genreRepository;
            _platformRepository = platformRepository;
            _dlcRepository = dlcRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Game>> GetAllGamesAsync()
        {
            try
            {
                return await _gameRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all games");
                return new List<Game>();
            }
        }

        public async Task<IEnumerable<Game>> GetGamesByGenreAsync(string genreSlug)
        {
            try
            {
                return await _gameRepository.GetByGenreAsync(genreSlug);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting games by genre {genreSlug}");
                return new List<Game>();
            }
        }

        public async Task<IEnumerable<Game>> SearchGamesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetAllGamesAsync();

                return await _gameRepository.SearchAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching games with term {searchTerm}");
                return new List<Game>();
            }
        }

        public async Task<Game> GetGameByIdAsync(int id)
        {
            try
            {
                return await _gameRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting game by id {id}");
                return null;
            }
        }

        public async Task<IEnumerable<Genre>> GetAllGenresAsync()
        {
            try
            {
                return await _genreRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all genres");
                return new List<Genre>();
            }
        }

        public async Task<IEnumerable<Platform>> GetAllPlatformsAsync()
        {
            try
            {
                return await _platformRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all platforms");
                return new List<Platform>();
            }
        }

        public async Task<IEnumerable<DLC>> GetGameDLCsAsync(int gameId)
        {
            try
            {
                return await _dlcRepository.GetByGameIdAsync(gameId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting DLCs for game {gameId}");
                return new List<DLC>();
            }
        }
    }
}
