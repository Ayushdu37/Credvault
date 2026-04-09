using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Common
{
    /// <summary>
    /// Standard API response wrapper. Every endpoint returns this shape.
    /// T = the type of data you're returning (e.g., UserProfile, AuthTokens, etc.)
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = [];

        // --- Factory methods (shortcuts to create responses) ---
        /// <summary>Use this when things go well</summary>
        public static ApiResponse<T> SuccessResponse(T data, string message = "Request successful")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>Use this when things go wrong</summary>
        public static ApiResponse<T> FailureResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? []
            };
        }

    }
}
