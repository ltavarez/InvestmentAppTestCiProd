﻿using FluentValidation;
using MediatR;

namespace InvestmentApp.Core.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var validationContext = new ValidationContext<TRequest>(request);
                var validationResults = await Task
                    .WhenAll(_validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)));
                var failures = validationResults.SelectMany(e => e.Errors).Where(f => f != null).ToList();

                if (failures.Count > 0)
                {
                    throw new Exceptions.ValidationException(failures);
                }
            }

            return await next(cancellationToken);
        }
    }
}
