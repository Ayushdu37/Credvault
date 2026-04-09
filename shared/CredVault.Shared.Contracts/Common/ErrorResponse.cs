using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Common
{
    /// <summary>
    /// Returned when something goes seriously wrong (500 errors, unhandled exceptions).
    /// </summary>
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }

        public ErrorResponse(int statusCode, string message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
    }
}
