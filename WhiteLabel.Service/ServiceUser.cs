using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Dto.Response;
using WhiteLabel.Domain.Entity;
using WhiteLabel.Domain.Entity.Enum;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Service
{
    public class ServiceUser : IServiceUser
    {
        private readonly IRepositoryUser _repository;

        public ServiceUser(IRepositoryUser repository)
        {
            _repository = repository;
        }

        // ==========================================
        // MÉTODOS PÚBLICOS
        // ==========================================

        public async Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto dto)
        {
            // Validar role
            if (!UserRoles.IsValidRole(dto.Role))
            {
                throw new Exception($"Invalid role: {dto.Role}");
            }

            // Verificar se email já existe
            var existingUsers = await _repository.FindAsync(u => u.Email == dto.Email);
            if (existingUsers.Any())
            {
                throw new Exception("Email already exists");
            }

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = HashPassword(dto.Password),
                Role = dto.Role,
                Phone = dto.Phone,
                Active = dto.Active,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _repository.AddAsync(user);

            return MapToResponseDto(user);
        }

        public async Task<UserResponseDto> UpdateUserAsync(string id, UpdateUserRequestDto dto)
        {
            var existingUser = await _repository.GetByIdAsync(id);
            if (existingUser == null)
            {
                throw new Exception("User not found");
            }

            // Validar role
            if (!UserRoles.IsValidRole(dto.Role))
            {
                throw new Exception($"Invalid role: {dto.Role}");
            }

            // Verificar se email já existe (exceto o próprio usuário)
            var usersWithEmail = await _repository.FindAsync(u => u.Email == dto.Email && u.Id != id);
            if (usersWithEmail.Any())
            {
                throw new Exception("Email already exists");
            }

            existingUser.Name = dto.Name;
            existingUser.Email = dto.Email;
            existingUser.Role = dto.Role;
            existingUser.Phone = dto.Phone;
            existingUser.Active = dto.Active;
            existingUser.UpdatedAt = DateTime.Now;

            // Atualizar senha apenas se fornecida
            if (!string.IsNullOrEmpty(dto.Password))
            {
                existingUser.Password = HashPassword(dto.Password);
            }

            await _repository.UpdateAsync(id, existingUser);

            return MapToResponseDto(existingUser);
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _repository.GetAllAsync();
            return users.Select(MapToResponseDto).ToList();
        }

        public async Task<UserResponseDto> GetUserByIdAsync(string id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            return MapToResponseDto(user);
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ChangePasswordAsync(string id, ChangePasswordRequestDto dto)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Verificar senha atual
            var currentPasswordHash = HashPassword(dto.CurrentPassword);
            if (user.Password != currentPasswordHash)
            {
                return false;
            }

            // Atualizar senha
            user.Password = HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(id, user);

            return true;
        }

        public async Task<List<RoleInfoDto>> GetAvailableRolesAsync()
        {
            var roles = UserRoles.GetAllRoles();
            return roles.Select(role => new RoleInfoDto
            {
                Value = role,
                Label = UserRoles.GetRoleLabel(role),
                Permissions = Permissions.GetRolePermissions(role),
                Description = GetRoleDescription(role)
            }).ToList();
        }

        // ==========================================
        // MÉTODOS AUXILIARES
        // ==========================================

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private UserResponseDto MapToResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                RoleLabel = UserRoles.GetRoleLabel(user.Role),
                Phone = user.Phone,
                Active = user.Active,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Permissions = Permissions.GetRolePermissions(user.Role)
            };
        }

        private string GetRoleDescription(string role)
        {
            return role switch
            {
                UserRoles.Administrator => "Acesso total ao sistema. Gerencia usuários, configurações e todas as funcionalidades.",
                UserRoles.SocialWorker => "Gerencia cadastro de pacientes, dados sociais e pode visualizar prontuários para acompanhamento.",
                UserRoles.Nutritionist => "Gerencia evoluções nutricionais, agenda de atendimentos e visualiza dados dos pacientes.",
                UserRoles.Psychologist => "Gerencia evoluções psicológicas, agenda de atendimentos e visualiza dados dos pacientes.",
                UserRoles.Physiotherapist => "Gerencia evoluções fisioterapêuticas, agenda de atendimentos e visualiza dados dos pacientes.",
                UserRoles.Secretary => "Visualiza pacientes e gerencia agenda de atendimentos. Sem acesso a prontuários.",
                _ => ""
            };
        }
    }
}