using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Entities;

namespace WhiteLabel.Domain.Interfaces
{
    public interface IServicePatient
    {
        Task<Patient> CreateAsync(CreatePatientRequestDto dto, string userId);
        Task<Patient> GetByIdAsync(string id);
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient> UpdateAsync(string id, UpdatePatientRequestDto dto);
        Task<bool> DeleteAsync(string id);
        Task<IEnumerable<Patient>> GetByStatusAsync(string status);
        Task<IEnumerable<Patient>> GetActiveAsync();
    }
}