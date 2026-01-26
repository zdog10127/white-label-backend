using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Dto.Request
{
    public class UpdatePatientRequestDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string SecondaryPhone { get; set; }
        public string Email { get; set; }
        public AddressRequestDto Address { get; set; }
        public CancerRequestDto Cancer { get; set; }
        public MedicalHistoryRequestDto MedicalHistory { get; set; }
        public decimal? FamilyIncome { get; set; }
        public int? NumberOfResidents { get; set; }
        public string Status { get; set; }
        public bool? Active { get; set; }
        public string Notes { get; set; }
    }
}