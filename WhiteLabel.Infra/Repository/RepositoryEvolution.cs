using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using WhiteLabel.Domain.Entity;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Infra.Repository
{
    public class RepositoryEvolution : Repository<Evolution>, IRepositoryEvolution
    {
        public RepositoryEvolution(IContext context)
            : base(context.Evolution)
        {
        }

        public async Task<IEnumerable<Evolution>> GetByPatientIdAsync(string patientId)
        {
            var filter = Builders<Evolution>.Filter.Eq(e => e.PatientId, patientId);
            return await _collection.Find(filter)
                .SortByDescending(e => e.Date)
                .ThenByDescending(e => e.Time)
                .ToListAsync();
        }
    }
}