using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Dto.Request
{
    public class CancerRequestDto
    {
        [Required]
        public string Type { get; set; }
        public DateTime? DetectionDate { get; set; }
        public string Stage { get; set; }
        public string TreatmentLocation { get; set; }
        public DateTime? TreatmentStartDate { get; set; }
        public string CurrentTreatment { get; set; }
        public bool HasBiopsyResult { get; set; } = false;
    }
}