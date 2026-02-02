using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using WhiteLabel.Domain.Entity;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Infra.Repository
{
    public class RepositoryAppointment : Repository<Appointment>, IRepositoryAppointment
    {
        public RepositoryAppointment(IContext context)
            : base(context.Appointment)
        {
        }

        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(string patientId)
        {
            return await FindAsync(a => a.PatientId == patientId);
        }

        public async Task<IEnumerable<Appointment>> GetByProfessionalIdAsync(string professionalId)
        {
            return await FindAsync(a => a.ProfessionalId == professionalId);
        }

        public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await FindAsync(a => a.Date >= startDate && a.Date <= endDate);
        }

        public async Task<IEnumerable<Appointment>> GetByStatusAsync(string status)
        {
            return await FindAsync(a => a.Status == status);
        }
    }
}