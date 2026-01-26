using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Dto.Response;

namespace WhiteLabel.Domain.Interfaces
{
    public interface IServicePatient
    {
        Task<PatientResponseDto> CreateAsync(CreatePatientRequestDto dto, string userId);
        Task<PatientResponseDto> GetByIdAsync(string id);
        Task<IEnumerable<PatientResponseDto>> GetAllAsync();
        Task<PatientResponseDto> UpdateAsync(string id, UpdatePatientRequestDto dto);
        Task<bool> DeleteAsync(string id);
        Task<IEnumerable<PatientResponseDto>> GetByStatusAsync(string status);
        Task<IEnumerable<PatientResponseDto>> GetActiveAsync();
    }
}