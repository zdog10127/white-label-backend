using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhiteLabel.Domain.Entities
{
    public class Patient
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "CPF is required")]
        public string CPF { get; set; }

        public string RG { get; set; }

        [Required(ErrorMessage = "Birth date is required")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; } 

        public string MaritalStatus { get; set; }

        // Contact
        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }

        public string SecondaryPhone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public Address Address { get; set; }

        public CancerData Cancer { get; set; }

        public MedicalHistory MedicalHistory { get; set; }

        public List<Medication> Medications { get; set; } = new List<Medication>();

        public string SUSCard { get; set; }
        public string HospitalCard { get; set; }

        public decimal? FamilyIncome { get; set; }
        public int? NumberOfResidents { get; set; }
        public List<FamilyComposition> FamilyComposition { get; set; } = new List<FamilyComposition>();

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? LastReviewDate { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? NextReviewDate { get; set; }

        public string Status { get; set; } = "Under Review"; 

        public bool Active { get; set; } = true;

        public int? TreatmentYear { get; set; }
        public bool FiveYears { get; set; } = false;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? DeathDate { get; set; }

        public bool AuthorizeImage { get; set; } = false;

        public string Notes { get; set; }

        public Documents Documents { get; set; } = new Documents();

        [BsonRepresentation(BsonType.ObjectId)]
        public string RegisteredById { get; set; }
    }

    public class Address
    {
        [Required]
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }
        
        [Required]
        public string Neighborhood { get; set; }
        
        public string City { get; set; } = "Araxá";
        public string State { get; set; } = "MG";
        public string ZipCode { get; set; }
    }

    public class CancerData
    {
        [Required]
        public string Type { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? DetectionDate { get; set; }

        public string Stage { get; set; }
        public string TreatmentLocation { get; set; } = "Hospital de Araxá";

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? TreatmentStartDate { get; set; }

        public string CurrentTreatment { get; set; }
        public bool HasBiopsyResult { get; set; } = false;
    }

    public class MedicalHistory
    {
        public bool Diabetes { get; set; } = false;
        public bool Hypertension { get; set; } = false;
        public bool Cholesterol { get; set; } = false;
        public bool Triglycerides { get; set; } = false;
        public bool KidneyProblems { get; set; } = false;
        public bool Anxiety { get; set; } = false;
        public bool HeartAttack { get; set; } = false;
        public string Others { get; set; }
    }

    public class Medication
    {
        public string Name { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
    }

    public class FamilyComposition
    {
        public string Name { get; set; }
        public string Relationship { get; set; }
        public int? Age { get; set; }
        public decimal? Income { get; set; }
        public string Profession { get; set; }
    }

    public class Documents
    {
        public bool Identity { get; set; } = false;
        public bool CPFDoc { get; set; } = false;
        public bool MarriageCertificate { get; set; } = false;
        public bool MedicalReport { get; set; } = false;
        public bool RecentExams { get; set; } = false;
        public bool AddressProof { get; set; } = false;
        public bool IncomeProof { get; set; } = false;
        public bool HospitalCardDoc { get; set; } = false;
        public bool SUSCardDoc { get; set; } = false;
        public bool BiopsyResultDoc { get; set; } = false;
    }
}