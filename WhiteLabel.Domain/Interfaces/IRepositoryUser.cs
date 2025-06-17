using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Dto.Response;
using WhiteLabel.Domain.Entity;

namespace WhiteLabel.Domain.Interfaces
{
    public interface IRepositoryUser
    {
        Task<List<User>> GetAllUsers();
        Task<User> GetById(string id);
        Task CreateUser(User user);
        Task UpdateUser(string id, User user);
        Task DeleteUser(string id);

        Task<List<Profile>> GetAllProfiles();
        Task<Profile> GetProfileById(string id);
        Task<ProfileResponseDto> ProfileUser(string id, string profile);

        Task<LoginResponseDto> LoginUser(string email, string password);
        Task<User> GetUserEmailAndPassword(string email, string password);
    }
}
