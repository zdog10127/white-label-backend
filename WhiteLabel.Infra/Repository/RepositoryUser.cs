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
    public class RepositoryUser : IRepositoryUser
    {
        private readonly IContext _context;

        public RepositoryUser()
        {
            _context = new Context();
        }

        public async Task<List<User>> GetAllUsers()
        {
            var lst = await _context.User.FindAsync(_ => true);
            return lst.ToList().OrderBy(x => x.Name).ToList();
        }

        public async Task<User> GetById(string id)
        {
            var user = await _context.User.Find(x => x.Id == id).FirstOrDefaultAsync();
            return user;
        }

        public async Task CreateUser(User user)
        {
            var profile = await _context.Profile.Find(x => x.Name == user.Profile).FirstOrDefaultAsync();
            if (profile != null)
            {
                await _context.User.InsertOneAsync(user);
            }
        }

        public async Task UpdateUser(string id, User user)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);
            var update = Builders<User>.Update.Set(x => x.Name, user.Name)
                                              .Set(x => x.Email, user.Email)
                                              .Set(x => x.CPF, user.CPF)
                                              .Set(x => x.Password, user.Password);

            await _context.User.UpdateOneAsync(filter, update);
        }

        public async Task DeleteUser(string id)
        {
            await _context.User.DeleteOneAsync(id);
        }

        public Task<List<Profile>> GetAllProfiles()
        {
            throw new NotImplementedException();
        }

        public Task<Profile> GetProfileById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserEmailAndPassword(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponseDto> LoginUser(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<ProfileResponseDto> ProfileUser(string id, string profile)
        {
            throw new NotImplementedException();
        }
    }
}