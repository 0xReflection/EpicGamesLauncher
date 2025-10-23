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
    public class OrderService : IOrderService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IEntitlementRepository _entitlementRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IDLCRepository _dlcRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            ITransactionRepository transactionRepository,
            IEntitlementRepository entitlementRepository,
            IUserRepository userRepository,
            IGameRepository gameRepository,
            IDLCRepository dlcRepository,
            ILogger<OrderService> logger)
        {
            _transactionRepository = transactionRepository;
            _entitlementRepository = entitlementRepository;
            _userRepository = userRepository;
            _gameRepository = gameRepository;
            _dlcRepository = dlcRepository;
            _logger = logger;
        }

        public async Task<bool> PurchaseGameAsync(int userId, int gameId)
        {
            try
            {
                if (await _entitlementRepository.HasGameAsync(userId, gameId))
                {
                    _logger.LogWarning($"User {userId} already owns game {gameId}");
                    return false;
                }

                var game = await _gameRepository.GetByIdAsync(gameId);
                if (game == null)
                {
                    _logger.LogError($"Game {gameId} not found");
                    return false;
                }
                var userBalance = await _userRepository.GetUserBalanceAsync(userId);
                if (userBalance < game.Price)
                {
                    _logger.LogWarning($"User {userId} has insufficient balance for game {gameId}");
                    return false;
                }
                var purchase = new Purchase
                {
                    GameId = gameId,
                    Price = game.Price
                };

                var purchases = new List<Purchase> { purchase };
                var transactionId = await _transactionRepository.CreatePurchaseTransactionAsync(userId, game.Price, purchases);

                if (transactionId > 0)
                {
                    await _entitlementRepository.AddGameToLibraryAsync(userId, gameId);
                    var newBalance = userBalance - game.Price;
                    await _userRepository.UpdateBalanceAsync(userId, newBalance);
                    _logger.LogInformation($"User {userId} successfully purchased game {gameId}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error purchasing game {gameId} for user {userId}");
                return false;
            }
        }

        public async Task<bool> PurchaseDLCAsync(int userId, int dlcId)
        {
            try
            {
                if (await _entitlementRepository.HasDLCAsync(userId, dlcId))
                {
                    _logger.LogWarning($"User {userId} already owns DLC {dlcId}");
                    return false;
                }
                var dlc = await _dlcRepository.GetByIdAsync(dlcId);
                if (dlc == null)
                {
                    _logger.LogError($"DLC {dlcId} not found");
                    return false;
                }

                
                if (!await _entitlementRepository.HasGameAsync(userId, dlc.GameId))
                {
                    _logger.LogWarning($"User {userId} doesn't own base game for DLC {dlcId}");
                    return false;
                }
                var userBalance = await _userRepository.GetUserBalanceAsync(userId);
                if (userBalance < dlc.Price)
                {
                    _logger.LogWarning($"User {userId} has insufficient balance for DLC {dlcId}");
                    return false;
                }
                var purchase = new Purchase
                {
                    GameId = dlc.GameId,
                    DLCId = dlcId,
                    Price = dlc.Price
                };

                var purchases = new List<Purchase> { purchase };
                var transactionId = await _transactionRepository.CreatePurchaseTransactionAsync(userId, dlc.Price, purchases);

                if (transactionId > 0)
                {
          
                    await _entitlementRepository.AddDLCToLibraryAsync(userId, dlcId);

       
                    var newBalance = userBalance - dlc.Price;
                    await _userRepository.UpdateBalanceAsync(userId, newBalance);

                    _logger.LogInformation($"User {userId} successfully purchased DLC {dlcId}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error purchasing DLC {dlcId} for user {userId}");
                return false;
            }
        }

        public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId)
        {
            try
            {
                return await _transactionRepository.GetUserTransactionsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting transactions for user {userId}");
                return new List<Transaction>();
            }
        }

        public async Task<decimal> GetUserBalanceAsync(int userId)
        {
            return await _userRepository.GetUserBalanceAsync(userId);
        }
    }
}
