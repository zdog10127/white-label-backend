using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Entity;
using WhiteLabel.Domain.Interfaces;
using WhiteLabel.Utility;

namespace WhiteLabel.Infra.Contexts
{
    public class Context : IContext
    {
        public IMongoCollection<User> User { get; }

        public IMongoCollection<Profile> Profile { get; }

        public IMongoCollection<AdditionalData> AdditionalData { get; }

        public IMongoCollection<Client> Client { get; }

        public IMongoCollection<Address> Address { get; }

        public Context()
        {
            var client = new MongoClient(ApplicationSettings.GetStringConnectionDB());
            var dataBase = client.GetDatabase("white-label");

            Client = dataBase.GetCollection<Client>("client");
            Address = dataBase.GetCollection<Address>("address");
            AdditionalData = dataBase.GetCollection<AdditionalData>("additionalData");
            User = dataBase.GetCollection<User>("user");
            Profile = dataBase.GetCollection<Profile>("profile");
        }
    }
}