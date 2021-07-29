using System;
using System.Collections.Generic;
using System.Linq;
using Catalog.Dtos;
using Catalog.Entities;
using Catalog.Repositories;
using Catalog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
{
    [ApiController]
    /*[Route("[controller]")]*/
    [Route("items")]
    public class ItemsController:ControllerBase
    {
        private readonly IItemsRepository _repository;

        public ItemsController(IItemsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        //public IEnumerable<Item> GetItems()
        public IEnumerable<ItemDto> GetItems()
        {
            //var items = _repository.GetItems();
            //return items;
            
            //return await Task.FromResult(items); for async
            
            //using dtos
           /* var items = _repository.GetItems().Select((item => new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                CreatedDate = item.CreatedDate
            }));*/
            var items = _repository.GetItems().Select((item => item.AsDto()));
            return items;
        }

        //get /items/{id}
        [HttpGet("{id}")]
        //rapping the result time with actionresult you can rreturn any type of response
        //public ActionResult<Item> GetItem(Guid id)
        public ActionResult<ItemDto> GetItem(Guid id)
        {
            var item = _repository.GetItems(id);
            if (item is null)
            {
                return NotFound();
            }
            return item.AsDto();
        }
        [HttpPost]
        public ActionResult<ItemDto> CreateItems(CreateItemDto itemDto)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            _repository.CreateItems(item);
            return CreatedAtAction(nameof(GetItem), new {id = item.Id}, item.AsDto());
        }
        [HttpPut("{id}")]
        public ActionResult UpdateItem(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = _repository.GetItems(id);
            if (existingItem is null)
            {
                return NotFound();
            }

            Item updatedItem = existingItem with
            {
                Name = itemDto.Name,
                Price = itemDto.Price
            };
            
            _repository.UpdateItems(updatedItem);
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public ActionResult DeleteItem(Guid id)
        {
            var existingItem = _repository.GetItems(id);
            if (existingItem is null)
            {
                return NotFound();
            }
            _repository.DeleteItems(id);
            return NoContent();
        }
    }
}