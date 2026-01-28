
using System.Threading.Tasks;
using MongoDB.Driver;
using WhiteLabel.Domain.Entity;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Infra.Repository
{
    public class RepositoryMedicalReport : Repository<MedicalReport>, IRepositoryMedicalReport
    {
        public RepositoryMedicalReport(IContext context)
            : base(context.MedicalReport)
        {
        }

        public async Task<MedicalReport> GetByPatientIdAsync(string patientId)
        {
            var filter = Builders<MedicalReport>.Filter.Eq(r => r.PatientId, patientId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
    }
}