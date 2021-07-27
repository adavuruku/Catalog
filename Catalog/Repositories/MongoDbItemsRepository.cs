using System;
using System.Collections.Generic;
using Catalog.Entities;
using Catalog.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Repositories
{
    public class MongoDbItemsRepository:IItemsRepository
    {
        private const string DatabaseName ="catalog";
        private const string CollectionName ="items";
        private readonly IMongoCollection<Item> itemsCollection;

        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;
        
        public MongoDbItemsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(DatabaseName);
            itemsCollection = database.GetCollection<Item>(CollectionName);
        }

        public IEnumerable<Item> GetItems()
        {
            return itemsCollection.Find(new BsonDocument()).ToList();
        }

        public Item GetItems(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            return itemsCollection.Find(filter).SingleOrDefault();
        }

        public void CreateItems(Item item)
        {
            itemsCollection.InsertOne(item);
        }

        public void UpdateItems(Item item)
        {
            var filter = filterBuilder.Eq(eachItem => eachItem.Id, item.Id);
            itemsCollection.ReplaceOne(filter, item);
        }

        public void DeleteItems(Guid id)
        {
            var filter = filterBuilder.Eq(item => item.Id, id);
            itemsCollection.DeleteOne(filter);
        }
    }
}