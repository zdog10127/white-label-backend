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

        [Required]
        public AddressRequestDto Address { get; set; }

        [Required]
        public CancerRequestDto Cancer { get; set; }

        public string SUSCard { get; set; }
        public string HospitalCard { get; set; }

        public bool AuthorizeImage { get; set; } = false;
    }
}