using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Entities;
using Catalog.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Repositories
{
    public class MongoDbItemsRepositoryAsync:IItemsRepositoryAsync
    {
        private const string DatabaseName ="catalog";
        private const string CollectionName ="items";
        private readonly IMongoCollection<Item> itemsCollection;

        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;
        
        public MongoDbItemsRepositoryAsync(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(DatabaseName);
            itemsCollection = database.GetCollection<Item>(CollectionName);
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await itemsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<Item> GetItemsAsync(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            return await itemsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task CreateItemsAsync(Item item)
        {
            await itemsCollection.InsertOneAsync(item);
        }

        public async  Task UpdateItemsAsync(Item item)
        {
            var filter = filterBuilder.Eq(eachItem => eachItem.Id, item.Id);
            await itemsCollection.ReplaceOneAsync(filter, item);
        }

        public async Task DeleteItemsAsync(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            await itemsCollection.DeleteOneAsync(filter);
        }
    }
}