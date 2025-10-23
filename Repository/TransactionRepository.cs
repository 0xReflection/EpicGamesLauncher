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
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
    {
        public override async Task<Transaction> GetByIdAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT TransactionId, UserId, TransactionDate, TotalAmount
            FROM Transactions 
            WHERE TransactionId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var transaction = new Transaction
                {
                    TransactionId = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    TransactionDate = reader.GetDateTime(2),
                    TotalAmount = reader.GetDecimal(3)
                };
                transaction.Purchases = (await GetTransactionPurchasesAsync(id)).ToList();
                return transaction;
            }
            return null;
        }

        public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId)
        {
            var transactions = new List<Transaction>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT TransactionId, UserId, TransactionDate, TotalAmount
            FROM Transactions 
            WHERE UserId = @UserId
            ORDER BY TransactionDate DESC";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var transaction = new Transaction
                {
                    TransactionId = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    TransactionDate = reader.GetDateTime(2),
                    TotalAmount = reader.GetDecimal(3)
                };
                transactions.Add(transaction);
            }

            foreach (var transaction in transactions)
            {
                transaction.Purchases = (await GetTransactionPurchasesAsync(transaction.TransactionId)).ToList();
            }

            return transactions;
        }

        public async Task<int> CreatePurchaseTransactionAsync(int userId, decimal totalAmount, IEnumerable<Purchase> purchases)
        {
            using var connection = await CreateConnectionAsync();

            using var dbTransaction = connection.BeginTransaction();

            try
            {
                // Новая транзакция
                var transactionQuery = @"
                INSERT INTO Transactions (UserId, TotalAmount)
                OUTPUT INSERTED.TransactionId
                VALUES (@UserId, @TotalAmount)";

                using var transactionCommand = new SqlCommand(transactionQuery, connection, dbTransaction);
                transactionCommand.Parameters.AddWithValue("@UserId", userId);
                transactionCommand.Parameters.AddWithValue("@TotalAmount", totalAmount);

                var transactionId = (int)await transactionCommand.ExecuteScalarAsync();

             
                foreach (var purchase in purchases)
                {
                    var purchaseQuery = @"
                    INSERT INTO Purchases (TransactionId, GameId, DLCId, Price)
                    VALUES (@TransactionId, @GameId, @DLCId, @Price)";

                    using var purchaseCommand = new SqlCommand(purchaseQuery, connection, dbTransaction);
                    purchaseCommand.Parameters.AddWithValue("@TransactionId", transactionId);
                    purchaseCommand.Parameters.AddWithValue("@GameId", purchase.GameId);
                    purchaseCommand.Parameters.AddWithValue("@DLCId", purchase.DLCId ?? (object)DBNull.Value);
                    purchaseCommand.Parameters.AddWithValue("@Price", purchase.Price);

                    await purchaseCommand.ExecuteNonQueryAsync();
                }

                dbTransaction.Commit();
                return transactionId;
            }
            catch
            {
                dbTransaction.Rollback();
                throw;
            }
        }

        private async Task<IEnumerable<Purchase>> GetTransactionPurchasesAsync(int transactionId)
        {
            var purchases = new List<Purchase>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT PurchaseId, TransactionId, GameId, DLCId, Price
            FROM Purchases 
            WHERE TransactionId = @TransactionId";

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
                    Price = reader.GetDecimal(4)
                });
            }
            return purchases;
        }

        public override async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            var transactions = new List<Transaction>();
            using var connection = await CreateConnectionAsync();
            var query = @"
            SELECT TransactionId, UserId, TransactionDate, TotalAmount
            FROM Transactions 
            ORDER BY TransactionDate DESC";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var transaction = new Transaction
                {
                    TransactionId = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    TransactionDate = reader.GetDateTime(2),
                    TotalAmount = reader.GetDecimal(3)
                };
                transactions.Add(transaction);
            }

            foreach (var transaction in transactions)
            {
                transaction.Purchases = (await GetTransactionPurchasesAsync(transaction.TransactionId)).ToList();
            }

            return transactions;
        }

        public override async Task<int> CreateAsync(Transaction transaction)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            INSERT INTO Transactions (UserId, TotalAmount)
            OUTPUT INSERTED.TransactionId
            VALUES (@UserId, @TotalAmount)";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", transaction.UserId);
            command.Parameters.AddWithValue("@TotalAmount", transaction.TotalAmount);

            return (int)await command.ExecuteScalarAsync();
        }

        public override async Task<bool> UpdateAsync(Transaction transaction)
        {
            using var connection = await CreateConnectionAsync();
            var query = @"
            UPDATE Transactions 
            SET UserId = @UserId, TotalAmount = @TotalAmount
            WHERE TransactionId = @TransactionId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TransactionId", transaction.TransactionId);
            command.Parameters.AddWithValue("@UserId", transaction.UserId);
            command.Parameters.AddWithValue("@TotalAmount", transaction.TotalAmount);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "DELETE FROM Transactions WHERE TransactionId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public override async Task<bool> ExistsAsync(int id)
        {
            using var connection = await CreateConnectionAsync();
            var query = "SELECT COUNT(*) FROM Transactions WHERE TransactionId = @Id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            return (int)await command.ExecuteScalarAsync() > 0;
        }
    }
}
