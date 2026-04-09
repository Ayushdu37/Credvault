using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingService.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
            => _validators = validators;

        public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            if (!_validators.Any()) return await next();
            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, ct)));
            var errors = results
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .Select(f => f.ErrorMessage)
                .ToList();

            if (errors.Count != 0)
            {
                var responseType = typeof(TResponse);
                if (responseType.IsGenericType)
                {
                    var failMethod = responseType.GetMethod("FailureResponse",
                        new[] { typeof(string), typeof(List<string>) });
                    if (failMethod != null)
                        return (TResponse)failMethod.Invoke(null,
                            new object[] { "Validation failed.", errors })!;
                }
                throw new ValidationException(errors.Select(
                    e => new FluentValidation.Results.ValidationFailure("", e)));
            }
            return await next();
        }
    }
}
