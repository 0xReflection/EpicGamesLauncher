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
    public class PurchaseRepository : RepositoryBase<Purchase>, IPurchaseRepository
    {
        public override async Task<Purchase> GetByIdAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT p.PurchaseId, p.TransactionId, p.GameId, p.DLCId, p.Price,
                   g.Title as GameTitle, d.Title as DLCTitle
            FROM Purchases p
            LEFT JOIN Games g ON p.GameId = g.GameId
            LEFT JOIN DLCs d ON p.DLCId = d.DLCId
            WHERE p.PurchaseId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Purchase
                {
                    PurchaseId = reader.GetInt32(0),
                    TransactionId = reader.GetInt32(1),
                    GameId = reader.GetInt32(2),
                    DLCId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    Price = reader.GetDecimal(4),
                    Game = reader.IsDBNull(5) ? null : new Game { Title = reader.GetString(5) },
                    DLC = reader.IsDBNull(6) ? null : new DLC { Title = reader.GetString(6) }
                };
            }
            return null;
        }

        public async Task<IEnumerable<Purchase>> GetByTransactionIdAsync(int transactionId)
        {
            var purchases = new List<Purchase>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT p.PurchaseId, p.TransactionId, p.GameId, p.DLCId, p.Price,
                   g.Title as GameTitle, d.Title as DLCTitle
            FROM Purchases p
            LEFT JOIN Games g ON p.GameId = g.GameId
            LEFT JOIN DLCs d ON p.DLCId = d.DLCId
            WHERE p.TransactionId = @TransactionId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TransactionId", transactionId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                purchases.Add(new Purchase
                {
                    PurchaseId = reader.GetInt32(0),
                    TransactionId = reader.GetInt32(1),
                    GameId = reader.GetInt32(2),
                    DLCId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    Price = reader.GetDecimal(4),
                    Game = reader.IsDBNull(5) ? null : new Game { Title = reader.GetString(5) },
                    DLC = reader.IsDBNull(6) ? null : new DLC { Title = reader.GetString(6) }
                });
            }
            return purchases;
        }

        public async Task<IEnumerable<Purchase>> GetUserPurchasesAsync(int userId)
        {
            var purchases = new List<Purchase>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT p.PurchaseId, p.TransactionId, p.GameId, p.DLCId, p.Price,
                   g.Title as GameTitle, d.Title as DLCTitle
            FROM Purchases p
            LEFT JOIN Games g ON p.GameId = g.GameId
            LEFT JOIN DLCs d ON p.DLCId = d.DLCId
            INNER JOIN Transactions t ON p.TransactionId = t.TransactionId
            WHERE t.UserId = @UserId
            ORDER BY t.TransactionDate DESC";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                purchases.Add(new Purchase
                {
                    PurchaseId = reader.GetInt32(0),
                    TransactionId = reader.GetInt32(1),
                    GameId = reader.GetInt32(2),
                    DLCId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    Price = reader.GetDecimal(4),
                    Game = reader.IsDBNull(5) ? null : new Game { Title = reader.GetString(5) },
                    DLC = reader.IsDBNull(6) ? null : new DLC { Title = reader.GetString(6) }
                });
            }
            return purchases;
        }

        public override async Task<IEnumerable<Purchase>> GetAllAsync()
        {
            var purchases = new List<Purchase>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT p.PurchaseId, p.TransactionId, p.GameId, p.DLCId, p.Price,
                   g.Title as GameTitle, d.Title as DLCTitle
            FROM Purchases p
            LEFT JOIN Games g ON p.GameId = g.GameId
            LEFT JOIN DLCs d ON p.DLCId = d.DLCId
            ORDER BY p.PurchaseId";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                purchases.Add(new Purchase
                {
                    PurchaseId = reader.GetInt32(0),
                    TransactionId = reader.GetInt32(1),
                    GameId = reader.GetInt32(2),
                    DLCId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    Price = reader.GetDecimal(4),
                    Game = reader.IsDBNull(5) ? null : new Game { Title = reader.GetString(5) },
                    DLC = reader.IsDBNull(6) ? null : new DLC { Title = reader.GetString(6) }
                });
            }
            return purchases;
        }

        public override async Task<int> CreateAsync(Purchase purchase)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            INSERT INTO Purchases (TransactionId, GameId, DLCId, Price)
            OUTPUT INSERTED.PurchaseId
            VALUES (@TransactionId, @GameId, @DLCId, @Price)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TransactionId", purchase.TransactionId);
            command.Parameters.AddWithValue("@GameId", purchase.GameId);
            command.Parameters.AddWithValue("@DLCId", purchase.DLCId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Price", purchase.Price);

            return (int)await command.ExecuteScalarAsync();
        }

        public override async Task<bool> UpdateAsync(Purchase purchase)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            UPDATE Purchases 
            SET TransactionId = @TransactionId, GameId = @GameId, 
                DLCId = @DLCId, Price = @Price
            WHERE PurchaseId = @PurchaseId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PurchaseId", purchase.PurchaseId);
            command.Parameters.AddWithValue("@TransactionId", purchase.TransactionId);
            command.Parameters.AddWithValue("@GameId", purchase.GameId);
            command.Parameters.AddWithValue("@DLCId", purchase.DLCId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Price", purchase.Price);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "DELETE FROM Purchases WHERE PurchaseId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> ExistsAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT COUNT(*) FROM Purchases WHERE PurchaseId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return (int)await command.ExecuteScalarAsync() > 0;
        }
    }
}
