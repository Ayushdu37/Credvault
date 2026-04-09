using CredVault.Shared.Contracts.Common;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityService.Application.Behaviors
{
    /// <summary>
    /// Runs FluentValidation validators BEFORE the handler executes.
    /// If validation fails, returns a failure response without hitting the handler.
    /// </summary>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next(); // No validators? Just continue.

            var context = new ValidationContext<TRequest>(request);

            // Run all validators
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // Collect all errors
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                var errors = failures.Select(f => f.ErrorMessage).ToList();

                // Try to create a failure response using the ApiResponse pattern
                var responseType = typeof(TResponse);
                if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(ApiResponse<>))
                {
                    var failureMethod = responseType.GetMethod("FailureResponse",
                        new[] { typeof(string), typeof(List<string>) });

                    if (failureMethod != null)
                    {
                        var result = failureMethod.Invoke(null, new object[] { "Validation failed.", errors });
                        return (TResponse)result!;
                    }
                }

                // Fallback — throw if we can't create a proper response
                throw new ValidationException(failures);
            }
            return await next(); // Validation passed, continue to handler
        }
    }
}
