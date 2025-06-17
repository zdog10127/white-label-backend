using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto;
using WhiteLabel.Domain.Entity;

namespace WhiteLabel.Domain.Interfaces
{
    public interface IServiceUser
    {
        Task<List<User>> GetAllUser();
        Task<User> GetByIdUser(string id);
        Task<ResponseDto> PostUser(User user);
        Task<ResponseDto> PutUser(string id, User user);
        Task<ResponseDto> DeleteUser(string id);

        Task<List<Profile>> GetAllProfiles();
        Task<Profile> GetProfileById(string id);
        Task<ResponseDto> LoginUser(string email, string password);
    }
}