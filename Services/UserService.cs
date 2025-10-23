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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User> GetUserProfileAsync(int userId)
        {
            try
            {
                return await _userRepository.GetByIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user profile for user {userId}");
                return null;
            }
        }

        public async Task<bool> UpdateUserProfileAsync(User user)
        {
            try
            {
                return await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user profile for user {user.UserId}");
                return false;
            }
        }

        public async Task<bool> UpdateUserBalanceAsync(int userId, decimal newBalance)
        {
            try
            {
                return await _userRepository.UpdateBalanceAsync(userId, newBalance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating balance for user {userId}");
                return false;
            }
        }
    }
}
