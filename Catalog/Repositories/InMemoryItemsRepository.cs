using System;
using System.Collections.Generic;
using System.Linq;
using Catalog.Entities;
using Catalog.Services;

namespace Catalog.Repositories
{
    public class InMemoryItemsRepository :IItemsRepository
    {
        private readonly List<Item> _items = new()
        {
            new Item {Id = Guid.NewGuid(), Name = "Potion", Price = 9, CreatedDate = DateTimeOffset.UtcNow},
            new Item {Id = Guid.NewGuid(), Name = "Sherif", Price = 95, CreatedDate = DateTimeOffset.UtcNow},
            new Item {Id = Guid.NewGuid(), Name = "Mubarak", Price = 55, CreatedDate = DateTimeOffset.UtcNow},
            new Item {Id = Guid.NewGuid(), Name = "Adams", Price = 40, CreatedDate = DateTimeOffset.UtcNow}
        };

        public IEnumerable<Item> GetItems()
        {
            return _items;
        }
        
        public Item GetItems(Guid id)
        {
            return _items.Where(item => item.Id == id).SingleOrDefault();
        }
        
        public void CreateItems(Item item)
        {
            _items.Add(item);
        }
        
        public void UpdateItems(Item item)
        {
            var index = _items.FindIndex(eachItems => eachItems.Id == item.Id);
            _items[index] = item;

        }

        public void DeleteItems(Guid id)
        {
            var index = _items.FindIndex(eachItems => eachItems.Id == id);
            _items.RemoveAt(index);
        }
    }
}