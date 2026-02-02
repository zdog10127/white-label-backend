using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Entity;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Service
{
    public class ServiceAppointment : IServiceAppointment
    {
        private readonly IRepositoryAppointment _repository;

        public ServiceAppointment(IRepositoryAppointment repository)
        {
            _repository = repository;
        }

        public async Task<List<Appointment>> GetAllAsync()
        {
            var appointments = await _repository.GetAllAsync();
            return appointments.OrderBy(a => a.Date).ThenBy(a => a.StartTime).ToList();
        }

        public async Task<Appointment> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<Appointment>> GetByPatientIdAsync(string patientId)
        {
            var appointments = await _repository.GetByPatientIdAsync(patientId);
            return appointments.OrderByDescending(a => a.Date).ThenByDescending(a => a.StartTime).ToList();
        }

        public async Task<List<Appointment>> GetByProfessionalIdAsync(string professionalId)
        {
            var appointments = await _repository.GetByProfessionalIdAsync(professionalId);
            return appointments.OrderBy(a => a.Date).ThenBy(a => a.StartTime).ToList();
        }

        public async Task<List<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var appointments = await _repository.GetByDateRangeAsync(startDate, endDate);
            return appointments.OrderBy(a => a.Date).ThenBy(a => a.StartTime).ToList();
        }

        public async Task<List<Appointment>> QueryAsync(AppointmentQueryDto query)
        {
            var allAppointments = await _repository.GetAllAsync();
            var filtered = allAppointments.AsQueryable();

            if (!string.IsNullOrEmpty(query.PatientId))
            {
                filtered = filtered.Where(a => a.PatientId == query.PatientId);
            }

            if (!string.IsNullOrEmpty(query.ProfessionalId))
            {
                filtered = filtered.Where(a => a.ProfessionalId == query.ProfessionalId);
            }

            if (query.StartDate.HasValue)
            {
                filtered = filtered.Where(a => a.Date >= query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                filtered = filtered.Where(a => a.Date <= query.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                filtered = filtered.Where(a => a.Status == query.Status);
            }

            if (!string.IsNullOrEmpty(query.Type))
            {
                filtered = filtered.Where(a => a.Type == query.Type);
            }

            return filtered.OrderBy(a => a.Date).ThenBy(a => a.StartTime).ToList();
        }

        public async Task<Appointment> CreateAsync(CreateAppointmentRequestDto dto, string userId)
        {
            // Verificar disponibilidade
            var isAvailable = await CheckAvailabilityAsync(
                dto.ProfessionalId, 
                dto.Date, 
                dto.StartTime, 
                dto.EndTime
            );

            if (!isAvailable)
            {
                throw new Exception("Professional is not available at this time");
            }

            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
                PatientName = dto.PatientName,
                ProfessionalId = dto.ProfessionalId,
                ProfessionalName = dto.ProfessionalName,
                Date = dto.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Duration = dto.Duration,
                Type = dto.Type,
                Specialty = dto.Specialty,
                Notes = dto.Notes,
                Status = "Agendado",
                CreatedBy = userId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            return await _repository.AddAsync(appointment);
        }

        public async Task<Appointment> UpdateAsync(string id, UpdateAppointmentDto dto, string userId)
        {
            var appointment = await _repository.GetByIdAsync(id);
            if (appointment == null)
            {
                throw new Exception("Appointment not found");
            }

            // Se está mudando data/hora, verificar disponibilidade
            if (dto.Date.HasValue || !string.IsNullOrEmpty(dto.StartTime) || !string.IsNullOrEmpty(dto.EndTime))
            {
                var newDate = dto.Date ?? appointment.Date;
                var newStartTime = dto.StartTime ?? appointment.StartTime;
                var newEndTime = dto.EndTime ?? appointment.EndTime;

                var isAvailable = await CheckAvailabilityAsync(
                    appointment.ProfessionalId,
                    newDate,
                    newStartTime,
                    newEndTime,
                    id
                );

                if (!isAvailable)
                {
                    throw new Exception("Professional is not available at this time");
                }

                appointment.Date = newDate;
                appointment.StartTime = newStartTime;
                appointment.EndTime = newEndTime;
            }

            if (dto.Duration.HasValue)
                appointment.Duration = dto.Duration.Value;

            if (!string.IsNullOrEmpty(dto.Type))
                appointment.Type = dto.Type;

            if (!string.IsNullOrEmpty(dto.Specialty))
                appointment.Specialty = dto.Specialty;

            if (!string.IsNullOrEmpty(dto.Status))
                appointment.Status = dto.Status;

            if (dto.Notes != null)
                appointment.Notes = dto.Notes;

            appointment.UpdatedBy = userId;
            appointment.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(id, appointment);
            return appointment;
        }

        public async Task<bool> CancelAsync(string id, CancelAppointmentDto dto, string userId)
        {
            var appointment = await _repository.GetByIdAsync(id);
            if (appointment == null)
            {
                throw new Exception("Appointment not found");
            }

            appointment.Status = "Cancelado";
            appointment.CancellationReason = dto.CancellationReason;
            appointment.UpdatedBy = userId;
            appointment.UpdatedAt = DateTime.Now;

            return await _repository.UpdateAsync(id, appointment);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var appointment = await _repository.GetByIdAsync(id);
            if (appointment == null)
            {
                throw new Exception("Appointment not found");
            }

            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> CheckAvailabilityAsync(
            string professionalId, 
            DateTime date, 
            string startTime, 
            string endTime, 
            string excludeAppointmentId = null)
        {
            // Buscar agendamentos do profissional na mesma data
            var existingAppointments = await _repository.GetByProfessionalIdAsync(professionalId);
            existingAppointments = existingAppointments.Where(a =>
                a.Date.Date == date.Date &&
                a.Status != "Cancelado" &&
                (excludeAppointmentId == null || a.Id != excludeAppointmentId)
            );

            // Verificar conflitos de horário
            foreach (var existing in existingAppointments)
            {
                if (HasTimeConflict(startTime, endTime, existing.StartTime, existing.EndTime))
                {
                    return false;
                }
            }

            return true;
        }

        private bool HasTimeConflict(string start1, string end1, string start2, string end2)
        {
            var startTime1 = TimeSpan.Parse(start1);
            var endTime1 = TimeSpan.Parse(end1);
            var startTime2 = TimeSpan.Parse(start2);
            var endTime2 = TimeSpan.Parse(end2);

            // Verifica se há sobreposição
            return startTime1 < endTime2 && endTime1 > startTime2;
        }
    }
}