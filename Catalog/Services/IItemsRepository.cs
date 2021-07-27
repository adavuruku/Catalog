using System;
using System.Collections.Generic;
using Catalog.Entities;

namespace Catalog.Services
{
    public interface IItemsRepository
    {
        IEnumerable<Item> GetItems();
        Item GetItems(Guid id);
        void CreateItems(Item item);
        
        void UpdateItems(Item item);
        
        void DeleteItems(Guid id);
    }
}