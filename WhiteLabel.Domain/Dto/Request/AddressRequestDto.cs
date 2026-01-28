using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Dto.Request
{
    public class AddressRequestDto
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string Complement { get; set; }

        [Required]
        public string Neighborhood { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
}