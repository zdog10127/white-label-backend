using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Dto.Request
{
    public class ProfileResponseDto
    {
        public string ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string IdUser { get; set; }
        public string UserName { get; set; }
    }
}