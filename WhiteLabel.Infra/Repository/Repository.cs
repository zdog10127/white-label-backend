using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Infra.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> _collection;

        public Repository(IMongoCollection<T> collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        public virtual async Task<T> GetByIdAsync(string id)
        {
            var objectId = new MongoDB.Bson.ObjectId(id);
            var filter = Builders<T>.Filter.Eq("_id", objectId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public virtual async Task<bool> UpdateAsync(string id, T entity)
        {
            var objectId = new MongoDB.Bson.ObjectId(id);
            var filter = Builders<T>.Filter.Eq("_id", objectId);
            var result = await _collection.ReplaceOneAsync(filter, entity);
            return result.ModifiedCount > 0;
        }

        public virtual async Task<bool> DeleteAsync(string id)
        {
            var objectId = new MongoDB.Bson.ObjectId(id);
            var filter = Builders<T>.Filter.Eq("_id", objectId);
            var result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public virtual async Task<long> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
                return await _collection.CountDocumentsAsync(_ => true);

            return await _collection.CountDocumentsAsync(predicate);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            var count = await _collection.CountDocumentsAsync(predicate);
            return count > 0;
        }
    }
}