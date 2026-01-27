using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Dto.Request
{
    public class MedicationRequestDto
    {
        public string Name { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
    }
}