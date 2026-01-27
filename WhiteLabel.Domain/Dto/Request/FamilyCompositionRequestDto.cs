using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Dto.Request
{
    public class FamilyCompositionRequestDto
    {
        public string Name { get; set; }
        public string Relationship { get; set; }
        public int? Age { get; set; }
        public decimal? Income { get; set; }
        public string Profession { get; set; }
    }
}