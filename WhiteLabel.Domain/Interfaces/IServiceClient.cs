using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Dto.Response;

namespace WhiteLabel.Domain.Interfaces
{
    public interface IServiceClient
    {
        Task<List<ClientResponseDto>> GetAllClients();
        Task<ClientResponseDto> GetById(string id);
        Task<ResponseDto> CreateClient(ClientRequestDto client);
        Task<ResponseDto> UpdateClient(string id, ClientRequestDto client);
        Task<ResponseDto> DeleteClient(string id);
    }
}