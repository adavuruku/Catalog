using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Entities;

namespace Catalog.Services
{
    public interface IItemsRepositoryAsync
    {
        Task<IEnumerable<Item>> GetItemsAsync();
        Task<Item> GetItemsAsync(Guid id);
        Task CreateItemsAsync(Item item);
        
        Task UpdateItemsAsync(Item item);
        
        Task DeleteItemsAsync(Guid id);
    }
}