using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Dto.Response;
using WhiteLabel.Domain.Entity;

namespace WhiteLabel.Domain.Interfaces
{
    public interface IRepositoryClient
    {
        Task<List<ClientResponseDto>> GetAllClients();
        Task<ClientResponseDto> GetById(string id);
        Task CreateClient(ClientRequestDto client);
        Task UpdateClient(string id, ClientRequestDto client);
        Task DeleteClient(string id);
    }
}