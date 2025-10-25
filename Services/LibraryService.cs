using EpicGamesLauncher.Logger.Interface;
using EpicGamesLauncher.Models;
using EpicGamesLauncher.Repository.Interfaces;
using EpicGamesLauncher.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly IEntitlementRepository _entitlementRepository;
        private readonly ILogger<LibraryService> _logger;

        public LibraryService(IEntitlementRepository entitlementRepository, ILogger<LibraryService> logger)
        {
            _entitlementRepository = entitlementRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Entitlement>> GetUserLibraryAsync(int userId)
        {
            try
            {
                return await _entitlementRepository.GetUserLibraryAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting library for user {userId}");
                return new List<Entitlement>();
            }
        }

        public async Task<bool> HasGameAsync(int userId, int gameId)
        {
            try
            {
                return await _entitlementRepository.HasGameAsync(userId, gameId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if user {userId} has game {gameId}");
                return false;
            }
        }

        public async Task<bool> HasDLCAsync(int userId, int dlcId)
        {
            try
            {
                return await _entitlementRepository.HasDLCAsync(userId, dlcId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if user {userId} has DLC {dlcId}");
                return false;
            }
        }
        public async Task<bool> AddFreeGameToLibraryAsync(int userId, int gameId)
        {
            try
            {
                if (await HasGameAsync(userId, gameId))
                {
                    _logger.LogWarning($"User {userId} already owns game {gameId}");
                    return false;
                }

                var success = await _entitlementRepository.AddGameToLibraryAsync(userId, gameId);
                if (success)
                {
                    _logger.LogInformation($"User {userId} added free game {gameId} to library");
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding free game {gameId} to library for user {userId}");
                return false;
            }
        }
    }
}
