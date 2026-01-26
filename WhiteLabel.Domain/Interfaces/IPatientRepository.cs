using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Entities;

namespace WhiteLabel.Domain.Interfaces
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient> GetByCPFAsync(string cpf);
        Task<bool> CPFExistsAsync(string cpf);
        Task<IEnumerable<Patient>> GetByStatusAsync(string status);
        Task<IEnumerable<Patient>> GetActiveAsync();
        Task<IEnumerable<Patient>> GetByNeighborhoodAsync(string neighborhood);
    }
}