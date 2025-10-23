using EpicGamesLauncher.Models;
using EpicGamesLauncher.Repository.Interfaces;
using EpicGamesLauncher.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpicGamesLauncher.Logger.Interface;

namespace EpicGamesLauncher.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                if (user == null)
                {
                    _logger.LogWarning($"Login failed: User {username} not found");
                    return null;
                }

                // Генерируем SHA-256 хеш для проверки
                using var sha256 = System.Security.Cryptography.SHA256.Create();
                var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                var computedHash = sha256.ComputeHash(passwordBytes);

                // Сравниваем хеши 
                if (!computedHash.SequenceEqual(user.PasswordHash))
                {
                    _logger.LogWarning($"Login failed: Invalid password for user {username}");
                    return null;
                }

                _logger.LogInformation($"User {username} successfully logged in");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during login for user {username}");
                return null;
            }
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            try
            {
                if (await _userRepository.UserExistsAsync(username, email))
                {
                    _logger.LogWarning($"Registration failed: User {username} or email {email} already exists");
                    return false;
                }

                // ген SHA-256 хеш 
                using var sha256 = System.Security.Cryptography.SHA256.Create();
                var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                var passwordHash = sha256.ComputeHash(passwordBytes);
                var passwordSalt = new byte[32]; // Пустой массив

                var user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Balance = 0.00m,
                    CreatedAt = DateTime.Now
                };

                await _userRepository.CreateAsync(user);
                _logger.LogInformation($"User {username} successfully registered");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during registration for user {username}");
                return false;
            }
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username) != null;
        }

        public async Task<decimal> GetUserBalanceAsync(int userId)
        {
            return await _userRepository.GetUserBalanceAsync(userId);
        }
    }
}
