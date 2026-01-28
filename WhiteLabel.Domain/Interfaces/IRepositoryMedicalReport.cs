using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Entity;

namespace WhiteLabel.Domain.Interfaces
{
    public interface IRepositoryMedicalReport : IRepository<MedicalReport>
    {
        Task<MedicalReport> GetByPatientIdAsync(string patientId);
    }
}