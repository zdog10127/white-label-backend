using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Domain.Dto.Request;
using WhiteLabel.Domain.Dto.Response;

namespace WhiteLabel.Domain.Interfaces
{
    public interface IServiceReport
    {
        Task<PatientReportResponseDto> GeneratePatientReportAsync(PatientReportRequestDto request);
        Task<ConsolidatedReportResponseDto> GenerateConsolidatedReportAsync(ConsolidatedReportRequestDto request);
    }
}