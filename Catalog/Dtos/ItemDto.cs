using System;

namespace Catalog.Dtos
{
    public record ItemDto
    {
        public Guid Id { get; init; } //init accessor means it can only be initialise once - it implies after creating the object this property cannot be changed
        public string Name { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}