using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Dto.Request
{
    public class DocumentsRequestDto
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