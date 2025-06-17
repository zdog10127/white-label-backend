using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Dto
{
    public class ResponseDto
    {
        public ResponseDto()
        { }
        public ResponseDto(bool hasError = false,
                          string? errorTitle = "",
                          string? errorMessage = "",
                          string? errorCode = "",
                          object? returnObject = null
                        )
        {
            HasError = hasError;
            ErrorTitle = errorTitle;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            ReturnObject = returnObject;
        }
        public bool HasError { get; set; }
        public string? ErrorTitle { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorCode { get; set; }
        public object? ReturnObject { get; set; }

        public CreatedResult ReturnResult(string requestRoute)
        {
            ProblemDetails problemDetails = new();
            problemDetails.Status = int.Parse(ErrorCode);
            problemDetails.Type = "";
            problemDetails.Detail = ErrorMessage;
            problemDetails.Title = ErrorTitle;
            problemDetails.Instance = requestRoute;

            CreatedResult createdResult = new("", null);
            createdResult.StatusCode = int.Parse(ErrorCode);
            createdResult.Value = problemDetails;

            return createdResult;
        }
    }
}