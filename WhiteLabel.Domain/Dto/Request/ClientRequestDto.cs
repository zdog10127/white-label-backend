using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Entity;

namespace WhiteLabel.Domain.Dto.Request
{
    public class ClientRequestDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? SocialName { get; set; }
        public string Naturalness { get; set; }
        public DateTime DtBirth { get; set; }
        public int Age { get; set; }
        public bool Gender { get; set; }
        public string Email { get; set; }
        public string CellPhone { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }
        public string? Comments { get; set; }
        public Address Addres { get; set; }
        public AdditionalData AdditionalData { get; set; }
    }
}