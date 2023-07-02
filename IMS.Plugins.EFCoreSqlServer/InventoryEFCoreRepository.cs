using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IMS.Plugins.EFCoreSqlServer
{
    public class InventoryEFCoreRepository : IInventoryRepository
    {
        private List<Inventory> _inventories;
        private readonly IMSContext db;

        public InventoryEFCoreRepository(IMSContext db)
        {
            this.db = db;
        }

        public async Task AddInventoryAsync(Inventory inventory)
        {
            await this.db.Inventories.AddAsync(inventory);

            await db.SaveChangesAsync();

            //if(_inventories.Any(i=>i.InventoryName.Equals(inventory.InventoryName, StringComparison.OrdinalIgnoreCase))) return Task.CompletedTask;

            ////Give the new inventory an id
            //var maxId = _inventories.Max(i => i.InventoryID);
            //inventory.InventoryID = maxId + 1;

            //_inventories.Add(inventory);
        }

        public async Task<bool> ExistsAsync(Inventory inventory)
        {
            return await db.Inventories.AsNoTracking().AnyAsync(i=>i.InventoryName.Equals(inventory.InventoryName,StringComparison.OrdinalIgnoreCase));
            //return await Task.FromResult(_inventories.Any(i=>i.InventoryName.Equals(inventory.InventoryName,StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<IEnumerable<Inventory>> GetInventoriesByNameAsync(string name)
        {
            return await this.db.Inventories.Where(x => x.InventoryName.ToLower().IndexOf(name.ToLower()) >= 0).ToListAsync();

            //if (string.IsNullOrWhiteSpace(name)) return await Task.FromResult(_inventories);

            //return _inventories.Where(x=>x.InventoryName.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task UpdateInventoryAsync(Inventory inventory)
        {
            //2 inventories can not have the same name
            var inv = await this.db.Inventories.FindAsync(inventory.InventoryID);
            if (inv != null)
            {
                inv.InventoryName = inventory.InventoryName;
                inv.Price = inventory.Price;
                inventory.Quantity = inventory.Quantity;


                this.db.Inventories.Update(inv);
                await this.db.SaveChangesAsync();
            }

            //if(_inventories.Any(i=>i.InventoryID != inventory.InventoryID && i.InventoryName.Equals(inventory.InventoryName, StringComparison.OrdinalIgnoreCase))) return Task.CompletedTask;

            //var inv = _inventories.FirstOrDefault(i=>i.InventoryID == inventory.InventoryID);
            //if (inv != null)
            //{
            //    inv.InventoryName = inventory.InventoryName;
            //    inv.Price = inventory.Price;
            //    inv.Quantity = inventory.Quantity;
            //}

            //return Task.CompletedTask;
        }

        public async Task<Inventory> GetInventoryByIdAsync(int inventoryId)
        {
            var inv = await this.db.Inventories.FindAsync(inventoryId);
            if (inv == null) return new Inventory();
            
            return inv;
            //return await Task.FromResult(_inventories.First(i=>i.InventoryID == inventoryId));
        }
    }
}