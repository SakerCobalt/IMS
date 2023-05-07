using IMS.CoreBusiness;
using IMS.UseCases.Inventories.Interfaces;
using IMS.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.UseCases.Inventories
{
    public class ViewInventoryByIdUseCase : IViewInventoryByIdUseCase
    {
        private readonly IInventoryRepository inventoryRepository;

        public ViewInventoryByIdUseCase(IInventoryRepository inventoryRepository)
        {
            this.inventoryRepository = inventoryRepository;
        }
        public async Task<Inventory> ExecuteAsync(int inventoryId)
        {
            var inv =  await this.inventoryRepository.GetInventoryByIdAsync(inventoryId);
            var newInv = new Inventory
            {
                InventoryID = inv.InventoryID,
                InventoryName = inv.InventoryName,
                Price = inv.Price,
                Quantity = inv.Quantity
            };

            return await Task.FromResult(newInv);
        }
    }
}
