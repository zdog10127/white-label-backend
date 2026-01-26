using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using WhiteLabel.Domain.Entities;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Infra.Repository
{
    public class RepositoryPatient : Repository<Patient>, IPatientRepository
    {
        public RepositoryPatient(IContext context)
            : base(context.Patients)
        {
        }

        public async Task<Patient> GetByCPFAsync(string cpf)
        {
            var filter = Builders<Patient>.Filter.Eq(p => p.CPF, cpf);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> CPFExistsAsync(string cpf)
        {
            return await ExistsAsync(p => p.CPF == cpf);
        }

        public async Task<IEnumerable<Patient>> GetByStatusAsync(string status)
        {
            return await FindAsync(p => p.Status == status);
        }

        public async Task<IEnumerable<Patient>> GetActiveAsync()
        {
            return await FindAsync(p => p.Active == true);
        }

        public async Task<IEnumerable<Patient>> GetByNeighborhoodAsync(string neighborhood)
        {
            return await FindAsync(p => p.Address.Neighborhood == neighborhood);
        }
    }
}