using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Entities;

namespace WhiteLabel.Domain.Dto.Response
{
    public class PatientResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CPF { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
        public CancerData Cancer { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Status { get; set; }
        public bool Active { get; set; }
    }
}