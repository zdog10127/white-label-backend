using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Entity;

namespace WhiteLabel.Domain.Interfaces
{
    public interface IContext
    {
        IMongoCollection<User> User { get; }
        IMongoCollection<Profile> Profile { get; }
        IMongoCollection<AdditionalData> AdditionalData { get; }
        IMongoCollection<Client> Client { get; }
        IMongoCollection<Address> Address { get; }
    }
}