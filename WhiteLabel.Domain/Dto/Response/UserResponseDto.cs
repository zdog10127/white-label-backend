using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Dto.Response
{
    public class UserResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string RoleLabel { get; set; }
        public bool Active { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string[] Permissions { get; set; }
    }

    public class RoleInfoDto
    {
        public string Value { get; set; }
        public string Label { get; set; }
        public string[] Permissions { get; set; }
        public string Description { get; set; }
    }
}