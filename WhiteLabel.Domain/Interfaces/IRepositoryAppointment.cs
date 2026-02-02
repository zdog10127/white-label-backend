using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Entity;

namespace WhiteLabel.Domain.Interfaces
{
    public interface IRepositoryAppointment : IRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(string patientId);
        Task<IEnumerable<Appointment>> GetByProfessionalIdAsync(string professionalId);
        Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Appointment>> GetByStatusAsync(string status);
    }
}