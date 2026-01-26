using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto;
using WhiteLabel.Domain.Entities;
using WhiteLabel.Domain.Entity;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Infra.Contexts
{
    public class Context : IContext
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<Context> _logger;

        public IMongoCollection<User> Users { get; }
        public IMongoCollection<Patient> Patients { get; }
        public IMongoCollection<Profile> Profile { get; }
        public IMongoCollection<AdditionalData> AdditionalData { get; }
        public IMongoCollection<Client> Client { get; }
        public IMongoCollection<Domain.Entities.Address> Address { get; }

        public Context(IOptions<MongoDBSettings> settings, ILogger<Context> logger)
        {
            _logger = logger;

            try
            {
                _logger.LogInformation("Initializing MongoDB Context");
                _logger.LogInformation("Connection String: {ConnectionString}", settings.Value.ConnectionString);
                _logger.LogInformation("Database Name: {DatabaseName}", settings.Value.DatabaseName);

                var client = new MongoClient(settings.Value.ConnectionString);
                _database = client.GetDatabase(settings.Value.DatabaseName);

                Users = _database.GetCollection<User>("users");
                Patients = _database.GetCollection<Patient>("patients");
                Profile = _database.GetCollection<Profile>("profile");
                AdditionalData = _database.GetCollection<AdditionalData>("additionalData");
                Client = _database.GetCollection<Client>("client");
                Address = _database.GetCollection<Domain.Entities.Address>("address");

                _logger.LogInformation("MongoDB Context initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing MongoDB Context: {Message}", ex.Message);
                throw;
            }
        }

        public bool IsConnected()
        {
            try
            {
                _database.RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}").Wait();
                _logger.LogInformation("MongoDB connection is healthy");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MongoDB connection failed: {Message}", ex.Message);
                return false;
            }
        }

        public IMongoDatabase GetDatabase() => _database;
    }
}