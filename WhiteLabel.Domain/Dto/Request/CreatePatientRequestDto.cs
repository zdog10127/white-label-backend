using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Dto.Request
{
    public class CreatePatientRequestDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "CPF is required")]
        public string CPF { get; set; }

        public string RG { get; set; }

        [Required(ErrorMessage = "Birth date is required")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        public string MaritalStatus { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }

        public string SecondaryPhone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public AddressRequestDto Address { get; set; }

        public CancerRequestDto Cancer { get; set; }

        public MedicalHistoryRequestDto MedicalHistory { get; set; }

        public List<MedicationRequestDto> Medications { get; set; } = new List<MedicationRequestDto>();

        [Required(ErrorMessage = "SUS Card is required")]
        public string SUSCard { get; set; }

        [Required(ErrorMessage = "Hospital Card is required")]
        public string HospitalCard { get; set; }

        public decimal? FamilyIncome { get; set; }
        public int? NumberOfResidents { get; set; }
        public List<FamilyCompositionRequestDto> FamilyComposition { get; set; } = new List<FamilyCompositionRequestDto>();

        public string Status { get; set; } = "Under Review";

        public bool Active { get; set; } = true;

        // NOVOS CAMPOS AMPARA
        public int? TreatmentYear { get; set; }
        public bool FiveYears { get; set; } = false;
        public DateTime? DeathDate { get; set; }
        public bool AuthorizeImage { get; set; } = false;

        public string Notes { get; set; }

        public DocumentsRequestDto Documents { get; set; } = new DocumentsRequestDto();
    }
}