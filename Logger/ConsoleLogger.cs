using EpicGamesLauncher.Logger.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.Logger
{
    namespace EpicGamesLauncher.Logger
    {
        public class ConsoleLogger<T> : ILogger<T> where T : class
        {
            public void LogInformation(string message)
            {
                Console.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {typeof(T).Name}: {message}");
            }

            public void LogWarning(string message)
            {
                Console.WriteLine($"[WARN] {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {typeof(T).Name}: {message}");
            }

            public void LogError(string message)
            {
                Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {typeof(T).Name}: {message}");
            }

            public void LogError(Exception ex, string message)
            {
                Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {typeof(T).Name}: {message} - {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }
}
