using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Dto.Request
{
    public class MedicalHistoryRequestDto
    {
        public bool Diabetes { get; set; }
        public bool Hypertension { get; set; }
        public bool Cholesterol { get; set; }
        public bool Triglycerides { get; set; }
        public bool KidneyProblems { get; set; }
        public bool Anxiety { get; set; }
        public bool HeartAttack { get; set; }
        public string Others { get; set; }
    }
}