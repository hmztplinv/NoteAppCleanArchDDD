using FluentValidation;
using MediatR;
using NoteApp.Application.Wrappers;

namespace NoteApp.Application.Behaviors;

// Pipeline behavior to validate all requests
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
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
            {
                var errorMessages = failures.Select(f => f.ErrorMessage).ToList();
                var responseType = typeof(TResponse);

                if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(ApiResponse<>))
                {
                    var genericArg = responseType.GetGenericArguments()[0];
                    var errorResponse = Activator.CreateInstance(typeof(ApiResponse<>).MakeGenericType(genericArg), new object[] { string.Join(" | ", errorMessages) })!;
                    return (TResponse)errorResponse;
                }
                else
                {
                    throw new ValidationException(failures);
                }
            }
        }

        return await next();
    }
}
