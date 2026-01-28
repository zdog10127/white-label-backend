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

        public async Task<Patient> CreateAsync(CreatePatientRequestDto dto, string userId)
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
                
                Address = dto.Address != null ? new Address
                {
                    Street = dto.Address.Street,
                    Number = dto.Address.Number,
                    Complement = dto.Address.Complement,
                    Neighborhood = dto.Address.Neighborhood,
                    City = dto.Address.City,
                    State = dto.Address.State,
                    ZipCode = dto.Address.ZipCode
                } : null,
                
                Cancer = dto.Cancer != null ? new CancerData
                {
                    Type = dto.Cancer.Type,
                    DetectionDate = dto.Cancer.DetectionDate,
                    Stage = dto.Cancer.Stage,
                    TreatmentLocation = dto.Cancer.TreatmentLocation,
                    TreatmentStartDate = dto.Cancer.TreatmentStartDate,
                    CurrentTreatment = dto.Cancer.CurrentTreatment,
                    HasBiopsyResult = dto.Cancer.HasBiopsyResult
                } : null,
                
                MedicalHistory = dto.MedicalHistory != null ? new MedicalHistory
                {
                    Diabetes = dto.MedicalHistory.Diabetes,
                    Hypertension = dto.MedicalHistory.Hypertension,
                    Cholesterol = dto.MedicalHistory.Cholesterol,
                    Triglycerides = dto.MedicalHistory.Triglycerides,
                    KidneyProblems = dto.MedicalHistory.KidneyProblems,
                    Anxiety = dto.MedicalHistory.Anxiety,
                    HeartAttack = dto.MedicalHistory.HeartAttack,
                    Others = dto.MedicalHistory.Others
                } : new MedicalHistory(),
                
                Medications = dto.Medications?.Select(m => new Medication
                {
                    Name = m.Name,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency
                }).ToList() ?? new List<Medication>(),
                
                SUSCard = dto.SUSCard,
                HospitalCard = dto.HospitalCard,
                
                FamilyIncome = dto.FamilyIncome,
                NumberOfResidents = dto.NumberOfResidents,
                FamilyComposition = dto.FamilyComposition?.Select(f => new FamilyComposition
                {
                    Name = f.Name,
                    Relationship = f.Relationship,
                    Age = f.Age,
                    Income = f.Income,
                    Profession = f.Profession
                }).ToList() ?? new List<FamilyComposition>(),
                
                Status = dto.Status,
                Active = dto.Active,
                
                TreatmentYear = dto.TreatmentYear,
                FiveYears = dto.FiveYears,
                DeathDate = dto.DeathDate,
                AuthorizeImage = dto.AuthorizeImage,
                
                Notes = dto.Notes,
                
                Documents = dto.Documents != null ? new Documents
                {
                    Identity = dto.Documents.Identity,
                    CPFDoc = dto.Documents.CPFDoc,
                    MarriageCertificate = dto.Documents.MarriageCertificate,
                    MedicalReport = dto.Documents.MedicalReport,
                    RecentExams = dto.Documents.RecentExams,
                    AddressProof = dto.Documents.AddressProof,
                    IncomeProof = dto.Documents.IncomeProof,
                    HospitalCardDoc = dto.Documents.HospitalCardDoc,
                    SUSCardDoc = dto.Documents.SUSCardDoc,
                    BiopsyResultDoc = dto.Documents.BiopsyResultDoc
                } : new Documents(),
                
                RegisteredById = userId,
                RegistrationDate = DateTime.Now,
                NextReviewDate = DateTime.Now.AddMonths(3)
            };

            await _patientRepository.AddAsync(patient);

            return patient;
        }

        public async Task<Patient> GetByIdAsync(string id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                throw new Exception("Patient not found");

            return patient;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _patientRepository.GetAllAsync();
        }

        public async Task<Patient> UpdateAsync(string id, UpdatePatientRequestDto dto)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                throw new Exception("Patient not found");

            if (!string.IsNullOrEmpty(dto.Name))
                patient.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Phone))
                patient.Phone = dto.Phone;

            if (!string.IsNullOrEmpty(dto.SecondaryPhone))
                patient.SecondaryPhone = dto.SecondaryPhone;

            if (!string.IsNullOrEmpty(dto.Email))
                patient.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.MaritalStatus))
                patient.MaritalStatus = dto.MaritalStatus;

            if (dto.Address != null)
            {
                patient.Address = new Address
                {
                    Street = dto.Address.Street,
                    Number = dto.Address.Number,
                    Complement = dto.Address.Complement,
                    Neighborhood = dto.Address.Neighborhood,
                    City = dto.Address.City ?? "Araxá",
                    State = dto.Address.State ?? "MG",
                    ZipCode = dto.Address.ZipCode
                };
            }

            if (dto.Cancer != null)
            {
                if (patient.Cancer == null)
                    patient.Cancer = new CancerData();
                    
                patient.Cancer.Type = dto.Cancer.Type ?? patient.Cancer.Type;
                patient.Cancer.Stage = dto.Cancer.Stage ?? patient.Cancer.Stage;
                patient.Cancer.CurrentTreatment = dto.Cancer.CurrentTreatment;
                patient.Cancer.TreatmentLocation = dto.Cancer.TreatmentLocation ?? "Hospital de Araxá";
                patient.Cancer.DetectionDate = dto.Cancer.DetectionDate ?? patient.Cancer.DetectionDate;
                patient.Cancer.TreatmentStartDate = dto.Cancer.TreatmentStartDate ?? patient.Cancer.TreatmentStartDate;
            }

            if (dto.MedicalHistory != null)
            {
                patient.MedicalHistory = new MedicalHistory
                {
                    Diabetes = dto.MedicalHistory.Diabetes,
                    Hypertension = dto.MedicalHistory.Hypertension,
                    Cholesterol = dto.MedicalHistory.Cholesterol,
                    Triglycerides = dto.MedicalHistory.Triglycerides,
                    KidneyProblems = dto.MedicalHistory.KidneyProblems,
                    Anxiety = dto.MedicalHistory.Anxiety,
                    HeartAttack = dto.MedicalHistory.HeartAttack,
                    Others = dto.MedicalHistory.Others
                };
            }

            if (dto.Medications != null)
            {
                patient.Medications = dto.Medications.Select(m => new Medication
                {
                    Name = m.Name,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency
                }).ToList();
            }

            if (!string.IsNullOrEmpty(dto.SUSCard))
                patient.SUSCard = dto.SUSCard;

            if (!string.IsNullOrEmpty(dto.HospitalCard))
                patient.HospitalCard = dto.HospitalCard;

            if (dto.FamilyIncome.HasValue)
                patient.FamilyIncome = dto.FamilyIncome;

            if (dto.NumberOfResidents.HasValue)
                patient.NumberOfResidents = dto.NumberOfResidents;

            if (dto.FamilyComposition != null)
            {
                patient.FamilyComposition = dto.FamilyComposition.Select(f => new FamilyComposition
                {
                    Name = f.Name,
                    Relationship = f.Relationship,
                    Age = f.Age,
                    Income = f.Income,
                    Profession = f.Profession
                }).ToList();
            }

            if (dto.Status != null)
                patient.Status = dto.Status;

            if (dto.Active.HasValue)
                patient.Active = dto.Active.Value;

            // NOVOS CAMPOS AMPARA
            if (dto.TreatmentYear.HasValue)
                patient.TreatmentYear = dto.TreatmentYear;

            if (dto.FiveYears.HasValue)
                patient.FiveYears = dto.FiveYears.Value;

            if (dto.DeathDate.HasValue)
                patient.DeathDate = dto.DeathDate;

            if (dto.AuthorizeImage.HasValue)
                patient.AuthorizeImage = dto.AuthorizeImage.Value;

            if (!string.IsNullOrEmpty(dto.Notes))
                patient.Notes = dto.Notes;

            if (dto.Documents != null)
            {
                patient.Documents = new Documents
                {
                    Identity = dto.Documents.Identity,
                    CPFDoc = dto.Documents.CPFDoc,
                    MarriageCertificate = dto.Documents.MarriageCertificate,
                    MedicalReport = dto.Documents.MedicalReport,
                    RecentExams = dto.Documents.RecentExams,
                    AddressProof = dto.Documents.AddressProof,
                    IncomeProof = dto.Documents.IncomeProof,
                    HospitalCardDoc = dto.Documents.HospitalCardDoc,
                    SUSCardDoc = dto.Documents.SUSCardDoc,
                    BiopsyResultDoc = dto.Documents.BiopsyResultDoc
                };
            }

            patient.LastReviewDate = DateTime.Now;

            await _patientRepository.UpdateAsync(id, patient);

            return patient;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                throw new Exception("Patient not found");

            return await _patientRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Patient>> GetByStatusAsync(string status)
        {
            return await _patientRepository.GetByStatusAsync(status);
        }

        public async Task<IEnumerable<Patient>> GetActiveAsync()
        {
            return await _patientRepository.GetActiveAsync();
        }
    }
}