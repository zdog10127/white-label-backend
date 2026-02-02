using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Entity;

namespace WhiteLabel.Domain.Interfaces
{
    public interface IServiceAppointment
    {
        Task<List<Appointment>> GetAllAsync();
        Task<Appointment> GetByIdAsync(string id);
        Task<List<Appointment>> GetByPatientIdAsync(string patientId);
        Task<List<Appointment>> GetByProfessionalIdAsync(string professionalId);
        Task<List<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<Appointment>> QueryAsync(AppointmentQueryDto query);
        Task<Appointment> CreateAsync(CreateAppointmentRequestDto dto, string userId);
        Task<Appointment> UpdateAsync(string id, UpdateAppointmentDto dto, string userId);
        Task<bool> CancelAsync(string id, CancelAppointmentDto dto, string userId);
        Task<bool> DeleteAsync(string id);
        Task<bool> CheckAvailabilityAsync(string professionalId, DateTime date, string startTime, string endTime, string excludeAppointmentId = null);
    }
}