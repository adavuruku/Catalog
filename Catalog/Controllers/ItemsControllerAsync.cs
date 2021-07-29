using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Dtos;
using Catalog.Entities;
using Catalog.Repositories;
using Catalog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
{
    [ApiController]
    /*[Route("[controller]")]*/
    [Route("items/async")]
    public class ItemsControllerAsync:ControllerBase
    {
        private readonly IItemsRepositoryAsync _repository;

        public ItemsControllerAsync(IItemsRepositoryAsync repository)
        {
            _repository = repository;
        }

        [HttpGet]
        //public IEnumerable<Item> GetItems()
        public async Task<IEnumerable<ItemDto>> GetItems()
        {
            var items = (await _repository.GetItemsAsync()).Select(item => item.AsDto());
            return items;
        }

        //get /items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItem(Guid id)
        {
            
            var item = await _repository.GetItemsAsync(id);
            if (item is null)
            {
                return NotFound();
            }
            return item.AsDto();
        }
        
        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItems(CreateItemDto itemDto)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _repository.CreateItemsAsync(item);
            //return Created(new Uri($"{Request.Path}/{productToCreate.Id}", UriKind.Relative), productToCreate);
            //public virtual Microsoft.AspNetCore.Mvc.CreatedAtActionResult CreatedAtAction (string actionName, object routeValues, object value);
            //public virtual Microsoft.AspNetCore.Mvc.CreatedAtActionResult CreatedAtAction (string actionName, object value);
           // public virtual Microsoft.AspNetCore.Mvc.CreatedAtActionResult CreatedAtAction (string actionName, string controllerName, object routeValues, object value);
           return CreatedAtAction(nameof(GetItems), new {id = item.Id}, item.AsDto());
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItem(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = await _repository.GetItemsAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            Item updatedItem = existingItem with
            {
                Name = itemDto.Name,
                Price = itemDto.Price
            };
            
           await _repository.UpdateItemsAsync(updatedItem);
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItem(Guid id)
        {
            var existingItem =await  _repository.GetItemsAsync(id);
            if (existingItem is null)
            {
                return NotFound();
            }
            await _repository.DeleteItemsAsync(id);
            return NoContent();
        }
    }
}