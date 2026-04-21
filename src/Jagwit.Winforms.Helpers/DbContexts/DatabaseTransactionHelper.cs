using Jagwit.Winforms.Helpers.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;

namespace Jagwit.Winforms.Helpers.DbContexts
{
    /// <summary>
    /// Extension methods for <see cref="DbContext"/> to simplify transactional operations.
    /// </summary>
    public static class DatabaseTransactionHelper
    {
        /// <summary>
        /// Executes <paramref name="transactionalAction"/> inside a <see cref="TransactionScope"/>.
        /// Handles common exceptions and surfaces them to the user via <see cref="MessageHandler"/>.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> instance (used as extension target).</param>
        /// <param name="transactionalAction">The async work to run inside the transaction.</param>
        public static async Task ExecuteInTransactionAsync(this DbContext context, Func<Task> transactionalAction)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await transactionalAction();
                    transactionScope.Complete();
                }
                catch (NullReferenceException nullEx)
                {
                    MessageHandler.ShowError($"Null data error:\n{nullEx}");
                }
                catch (DBConcurrencyException concurrencyEx)
                {
                    MessageHandler.ShowError($"Dirty data error:\n{concurrencyEx}");
                }
                catch (InvalidOperationException opEx)
                {
                    if (opEx.Message.StartsWith("INSUFFICIENT STOCK") ||
                        opEx.Message.StartsWith("PRODUCT NOT FOUND") ||
                        opEx.Message.StartsWith("INVALID OPERATION"))
                    {
                        MessageHandler.ShowWarning(opEx.Message);
                    }
                    else
                    {
                        MessageHandler.ShowError($"Invalid operation error: \n{opEx}");
                    }
                }
                catch (Exception ex)
                {
                    MessageHandler.ShowError($"Error occurred on executing transaction async:\n{ex}");
                }
            }
        }
    }
}
