using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhiteLabel.Domain.Entity;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Service
{
    public class ServiceEvolution : IServiceEvolution
    {
        private readonly IRepositoryEvolution _repository;

        public ServiceEvolution(IRepositoryEvolution repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Evolution>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Evolution> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Evolution>> GetByPatientIdAsync(string patientId)
        {
            return await _repository.GetByPatientIdAsync(patientId);
        }

        public async Task<Evolution> CreateAsync(Evolution evolution)
        {
            evolution.CreatedAt = DateTime.Now;
            evolution.UpdatedAt = DateTime.Now;
            
            return await _repository.AddAsync(evolution);
        }

        public async Task<Evolution> UpdateAsync(string id, Evolution evolution)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("Evolution not found");
            }

            evolution.Id = id;
            evolution.CreatedAt = existing.CreatedAt;
            evolution.AttendedBy = existing.AttendedBy;
            evolution.UpdatedAt = DateTime.Now;

            var updated = await _repository.UpdateAsync(id, evolution);
            if (!updated)
            {
                throw new Exception("Failed to update evolution");
            }
            
            return evolution;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}