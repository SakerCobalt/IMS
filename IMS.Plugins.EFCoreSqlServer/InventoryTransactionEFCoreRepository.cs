using IMS.CoreBusiness;
using IMS.Plugins.EFCoreSqlServer;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Plugins.EFCoreSqlServer
{
    public class InventoryTransactionEFCoreRepository : IInventoryTransactionRepository
    {
        private readonly IDbContextFactory<IMSContext> contextFactory;

        public InventoryTransactionEFCoreRepository(IDbContextFactory<IMSContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetInventoryTransactionsAsync(string inventoryName, DateTime? dateFrom, DateTime? dateTo, InventoryTransactionType? transactionType)
        {
            using var db = this.contextFactory.CreateDbContext();
            //select * from inventoryTransactions it
            //join inventories inv on it.inventoryId = inv.inventory.Id

            var query = from it in db.Transactions
                        join inv in db.Inventories on it.InventoryId equals inv.InventoryID
                        where (string.IsNullOrEmpty(inventoryName) || inv.InventoryName.ToLower().IndexOf(inventoryName.ToLower()) >= 0)
                        && (!dateFrom.HasValue || it.TransactionDate >= dateFrom.Value.Date)
                        && (!dateTo.HasValue || it.TransactionDate <= dateTo.Value.Date)
                        && (!transactionType.HasValue || it.ActivityType == transactionType)
                        select it;

            return await query.Include(i=>i.Inventory).ToListAsync();
        }

        public async Task ProduceAsync(string productionNumber, Inventory inventory, int quantityToConsume, string doneBy, double price)
        {
            using var db = this.contextFactory.CreateDbContext();

            db.Transactions.Add(new InventoryTransaction
            {
                ProductionNumber = productionNumber,
                InventoryId = inventory.InventoryID,
                QuantityBefore = inventory.Quantity,
                ActivityType = InventoryTransactionType.ProduceProduct,
                QuantityAfter = inventory.Quantity - quantityToConsume,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
                UnitPrice = price
            });

            await db.SaveChangesAsync();
        }

        public async Task PurchaseAsync(string poNumber, Inventory inventory, int quantity, string doneBy, double price)
        {
            using var db = this.contextFactory.CreateDbContext();

            db.Transactions.Add(new InventoryTransaction
            {
                PoNumber = poNumber,
                InventoryId = inventory.InventoryID,
                QuantityBefore = inventory.Quantity,
                ActivityType = InventoryTransactionType.PurchaseInventory,
                QuantityAfter = inventory.Quantity + quantity,
                TransactionDate = DateTime.Now, 
                DoneBy = doneBy,
                UnitPrice = price
            });

            await db.SaveChangesAsync();
        }
    }
}
