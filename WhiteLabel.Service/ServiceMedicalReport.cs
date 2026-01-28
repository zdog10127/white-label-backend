using System;
using System.Threading.Tasks;
using WhiteLabel.Domain.Entities;
using WhiteLabel.Domain.Entity;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Service
{
    public class ServiceMedicalReport : IServiceMedicalReport
    {
        private readonly IRepositoryMedicalReport _repository;

        public ServiceMedicalReport(IRepositoryMedicalReport repository)
        {
            _repository = repository;
        }

        public async Task<MedicalReport> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<MedicalReport> GetByPatientIdAsync(string patientId)
        {
            return await _repository.GetByPatientIdAsync(patientId);
        }

        public async Task<MedicalReport> CreateAsync(MedicalReport report)
        {
            // Verificar se já existe relatório para o paciente
            var existing = await _repository.GetByPatientIdAsync(report.PatientId);
            if (existing != null)
            {
                throw new Exception("Patient already has a medical report");
            }

            report.CreatedAt = DateTime.Now;
            report.UpdatedAt = DateTime.Now;
            
            // AddAsync retorna a entidade criada
            return await _repository.AddAsync(report);
        }

        public async Task<MedicalReport> UpdateAsync(string id, MedicalReport report)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("Medical report not found");
            }

            report.Id = id;
            report.CreatedAt = existing.CreatedAt;
            report.CreatedBy = existing.CreatedBy;
            report.UpdatedAt = DateTime.Now;

            // UpdateAsync retorna bool, então precisamos retornar o report
            var updated = await _repository.UpdateAsync(id, report);
            if (!updated)
            {
                throw new Exception("Failed to update medical report");
            }
            
            return report;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}