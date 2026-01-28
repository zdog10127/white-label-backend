using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Entity;

namespace WhiteLabel.Domain.Interfaces
{
    public interface IServiceMedicalReport
    {
        Task<MedicalReport> GetByIdAsync(string id);
        Task<MedicalReport> GetByPatientIdAsync(string patientId);
        Task<MedicalReport> CreateAsync(MedicalReport report);
        Task<MedicalReport> UpdateAsync(string id, MedicalReport report);
        Task<bool> DeleteAsync(string id);
    }
}