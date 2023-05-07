using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public class InventoryRepository : IInventoryRepository
    {
        private List<Inventory> _inventories;
        public InventoryRepository()
        {
            _inventories = new List<Inventory>()
            { 
                new Inventory{InventoryID = 1, InventoryName = "Bike Seat", Quantity = 10, Price=2},
                new Inventory{InventoryID = 2, InventoryName = "Bike Body", Quantity = 10, Price=15},
                new Inventory{InventoryID = 3, InventoryName = "Bike Wheels", Quantity = 20, Price=8},
                new Inventory{InventoryID = 4, InventoryName = "Bike Pedals", Quantity = 20, Price=1},
            };
        }

        public async Task<Task> AddInventoryAsync(Inventory inventory)
        {
            if(_inventories.Any(i=>i.InventoryName.Equals(inventory.InventoryName, StringComparison.OrdinalIgnoreCase))) return Task.CompletedTask;

            //Give the new inventory an id
            var maxId = _inventories.Max(i => i.InventoryID);
            inventory.InventoryID = maxId + 1;

            _inventories.Add(inventory);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Inventory inventory)
        {
            return await Task.FromResult(_inventories.Any(i=>i.InventoryName.Equals(inventory.InventoryName,StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<IEnumerable<Inventory>> GetInventoriesByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return await Task.FromResult(_inventories);

            return _inventories.Where(x=>x.InventoryName.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public Task UpdateInventoryAsync(Inventory inventory)
        {
            //2 inventories can not have the same name
            if(_inventories.Any(i=>i.InventoryID != inventory.InventoryID && i.InventoryName.Equals(inventory.InventoryName, StringComparison.OrdinalIgnoreCase))) return Task.CompletedTask;

            var inv = _inventories.FirstOrDefault(i=>i.InventoryID == inventory.InventoryID);
            if (inv != null)
            {
                inv.InventoryName = inventory.InventoryName;
                inv.Price = inventory.Price;
                inv.Quantity = inventory.Quantity;
            }

            return Task.CompletedTask;
        }

        public async Task<Inventory> GetInventoryByIdAsync(int inventoryId)
        {
            return await Task.FromResult(_inventories.First(i=>i.InventoryID == inventoryId));
        }
    }
}