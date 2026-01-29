using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Dto.Response;
using WhiteLabel.Domain.Entity;

namespace WhiteLabel.Domain.Interfaces
{
    public interface IServiceUser
    {
        Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto dto);
        Task<UserResponseDto> UpdateUserAsync(string id, UpdateUserRequestDto dto);
        Task<List<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> GetUserByIdAsync(string id);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> ChangePasswordAsync(string id, ChangePasswordRequestDto dto);
        Task<List<RoleInfoDto>> GetAvailableRolesAsync();
    }
}