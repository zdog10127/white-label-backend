using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Entity;

namespace WhiteLabel.Domain.Interfaces
{
    public interface IServiceEvolution
    {
        Task<IEnumerable<Evolution>> GetAllAsync();
        Task<Evolution> GetByIdAsync(string id);
        Task<IEnumerable<Evolution>> GetByPatientIdAsync(string patientId);
        Task<Evolution> CreateAsync(Evolution evolution);
        Task<Evolution> UpdateAsync(string id, Evolution evolution);
        Task<bool> DeleteAsync(string id);
    }
}