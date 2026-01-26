using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Dto.Response;
using WhiteLabel.Domain.Entity;
using WhiteLabel.Domain.Interfaces;
using WhiteLabel.Infra.Contexts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WhiteLabel.Infra.Repository
{
    public class RepositoryUser : Repository<User>, IRepositoryUser
    {
        public RepositoryUser(IContext context)
            : base(context.Users)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email.ToLower());
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await ExistsAsync(u => u.Email == email.ToLower());
        }
    }
}