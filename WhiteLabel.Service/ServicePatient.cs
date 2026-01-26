using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Dto.Response;
using WhiteLabel.Domain.Entities;
using WhiteLabel.Domain.Interfaces;

namespace WhiteLabel.Service
{
    public class ServicePatient : IServicePatient
    {
        private readonly IPatientRepository _patientRepository;

        public ServicePatient(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<PatientResponseDto> CreateAsync(CreatePatientRequestDto dto, string userId)
        {
            if (await _patientRepository.CPFExistsAsync(dto.CPF))
            {
                throw new Exception("CPF already registered");
            }

            var patient = new Patient
            {
                Name = dto.Name,
                CPF = dto.CPF,
                RG = dto.RG,
                BirthDate = dto.BirthDate,
                Gender = dto.Gender,
                MaritalStatus = dto.MaritalStatus,
                Phone = dto.Phone,
                SecondaryPhone = dto.SecondaryPhone,
                Email = dto.Email,
                Address = new Address
                {
                    Street = dto.Address.Street,
                    Number = dto.Address.Number,
                    Complement = dto.Address.Complement,
                    Neighborhood = dto.Address.Neighborhood,
                    City = dto.Address.City,
                    State = dto.Address.State,
                    ZipCode = dto.Address.ZipCode
                },
                Cancer = new CancerData
                {
                    Type = dto.Cancer.Type,
                    DetectionDate = dto.Cancer.DetectionDate,
                    Stage = dto.Cancer.Stage,
                    TreatmentLocation = dto.Cancer.TreatmentLocation,
                    TreatmentStartDate = dto.Cancer.TreatmentStartDate,
                    CurrentTreatment = dto.Cancer.CurrentTreatment,
                    HasBiopsyResult = dto.Cancer.HasBiopsyResult
                },
                SUSCard = dto.SUSCard,
                HospitalCard = dto.HospitalCard,
                AuthorizeImage = dto.AuthorizeImage,
                RegisteredById = userId,
                NextReviewDate = DateTime.Now.AddMonths(3)
            };

            await _patientRepository.AddAsync(patient);

            return MapToResponse(patient);
        }

        public async Task<PatientResponseDto> GetByIdAsync(string id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                throw new Exception("Patient not found");

            return MapToResponse(patient);
        }

        public async Task<IEnumerable<PatientResponseDto>> GetAllAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            var response = new List<PatientResponseDto>();

            foreach (var patient in patients)
            {
                response.Add(MapToResponse(patient));
            }

            return response;
        }

        public async Task<PatientResponseDto> UpdateAsync(string id, UpdatePatientRequestDto dto)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                throw new Exception("Patient not found");

            if (!string.IsNullOrEmpty(dto.Name))
                patient.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Phone))
                patient.Phone = dto.Phone;

            if (!string.IsNullOrEmpty(dto.Email))
                patient.Email = dto.Email;

            if (dto.Address != null)
            {
                patient.Address = new Address
                {
                    Street = dto.Address.Street,
                    Number = dto.Address.Number,
                    Complement = dto.Address.Complement,
                    Neighborhood = dto.Address.Neighborhood,
                    City = dto.Address.City,
                    State = dto.Address.State,
                    ZipCode = dto.Address.ZipCode
                };
            }

            if (dto.Cancer != null)
            {
                patient.Cancer.Type = dto.Cancer.Type ?? patient.Cancer.Type;
                patient.Cancer.CurrentTreatment = dto.Cancer.CurrentTreatment;
                patient.Cancer.TreatmentLocation = dto.Cancer.TreatmentLocation;
            }

            if (dto.Status != null)
                patient.Status = dto.Status;

            if (dto.Active.HasValue)
                patient.Active = dto.Active.Value;

            if (!string.IsNullOrEmpty(dto.Notes))
                patient.Notes = dto.Notes;

            await _patientRepository.UpdateAsync(id, patient);

            return MapToResponse(patient);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                throw new Exception("Patient not found");

            return await _patientRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<PatientResponseDto>> GetByStatusAsync(string status)
        {
            var patients = await _patientRepository.GetByStatusAsync(status);
            var response = new List<PatientResponseDto>();

            foreach (var patient in patients)
            {
                response.Add(MapToResponse(patient));
            }

            return response;
        }

        public async Task<IEnumerable<PatientResponseDto>> GetActiveAsync()
        {
            var patients = await _patientRepository.GetActiveAsync();
            var response = new List<PatientResponseDto>();

            foreach (var patient in patients)
            {
                response.Add(MapToResponse(patient));
            }

            return response;
        }

        private PatientResponseDto MapToResponse(Patient patient)
        {
            return new PatientResponseDto
            {
                Id = patient.Id,
                Name = patient.Name,
                CPF = patient.CPF,
                BirthDate = patient.BirthDate,
                Gender = patient.Gender,
                Phone = patient.Phone,
                Email = patient.Email,
                Address = patient.Address,
                Cancer = patient.Cancer,
                RegistrationDate = patient.RegistrationDate,
                Status = patient.Status,
                Active = patient.Active
            };
        }
    }
}