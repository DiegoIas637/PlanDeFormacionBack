using Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PlanDeFormacionBack.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var result = context.Exception switch
            {
                ValidationException ex => Build(StatusCodes.Status400BadRequest, "Error de validación", ex.Message),
                UnauthorizedException ex => Build(StatusCodes.Status401Unauthorized, "No autorizado", ex.Message),
                NotFoundException ex => Build(StatusCodes.Status404NotFound, "No encontrado", ex.Message),
                ConflictException ex => Build(StatusCodes.Status409Conflict, "Conflicto", ex.Message),
                _ => (ObjectResult?)null
            };

            if (result is null)
            {
                return;
            }

            context.Result = result;
            context.ExceptionHandled = true;
        }

        private static ObjectResult Build(int statusCode, string title, string detail)
        {
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            };

            return new ObjectResult(problem) { StatusCode = statusCode };
        }
    }
}
