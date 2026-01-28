using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Entities;
using WhiteLabel.Domain.Entity;

namespace WhiteLabel.Domain.Interfaces
{
    public interface IContext
    {
        IMongoCollection<User> Users { get; }
        IMongoCollection<Patient> Patients { get; }
        IMongoCollection<Profile> Profile { get; }
        IMongoCollection<AdditionalData> AdditionalData { get; }
        IMongoCollection<Client> Client { get; }
        IMongoCollection<Entities.Address> Address { get; }
        IMongoCollection<MedicalReport> MedicalReport { get; }
        IMongoCollection<Evolution> Evolution { get; }
        bool IsConnected();
        IMongoDatabase GetDatabase();
    }
}