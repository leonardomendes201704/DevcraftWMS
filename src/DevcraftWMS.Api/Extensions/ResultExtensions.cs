using Microsoft.AspNetCore.Mvc;
using DevcraftWMS.Api.Middleware;
using DevcraftWMS.Application.Common.Models;

namespace DevcraftWMS.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this ControllerBase controller, RequestResult<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }

        if (result.ValidationErrors is not null)
        {
            var modelState = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary();
            foreach (var (key, errors) in result.ValidationErrors)
            {
                foreach (var error in errors)
                {
                    modelState.AddModelError(key, error);
                }
            }

            var validationProblem = new ValidationProblemDetails(modelState);
            AddCorrelationId(controller, validationProblem);
            return new BadRequestObjectResult(validationProblem);
        }

        var problem = result.ErrorCode switch
        {
            "customer.not_found" => new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status404NotFound
            },
            "warehouses.warehouse.not_found" => new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status404NotFound
            },
            "warehouses.warehouse.code_exists" => new ProblemDetails
            {
                Title = "Conflict",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status409Conflict
            },
            "sectors.sector.not_found" => new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status404NotFound
            },
            "sectors.warehouse.not_found" => new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status404NotFound
            },
            "sectors.sector.code_exists" => new ProblemDetails
            {
                Title = "Conflict",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status409Conflict
            },
            "sectors.warehouse.mismatch" => new ProblemDetails
            {
                Title = "Bad Request",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status400BadRequest
            },
            "sections.section.not_found" => new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status404NotFound
            },
            "sections.sector.not_found" => new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status404NotFound
            },
            "sections.section.code_exists" => new ProblemDetails
            {
                Title = "Conflict",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status409Conflict
            },
            "sections.sector.mismatch" => new ProblemDetails
            {
                Title = "Bad Request",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status400BadRequest
            },
            "structures.structure.not_found" => new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status404NotFound
            },
            "structures.section.not_found" => new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status404NotFound
            },
            "structures.structure.code_exists" => new ProblemDetails
            {
                Title = "Conflict",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status409Conflict
            },
            "structures.section.mismatch" => new ProblemDetails
            {
                Title = "Bad Request",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status400BadRequest
            },
            "auth.email_exists" => new ProblemDetails
            {
                Title = "Conflict",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status409Conflict
            },
            "auth.invalid_credentials" => new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status401Unauthorized
            },
            "auth.unauthorized" => new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status401Unauthorized
            },
            "auth.invalid_password" => new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status401Unauthorized
            },
            "auth.user_not_found" => new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status404NotFound
            },
            "logs.error.not_found" => new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status404NotFound
            },
            "logs.transaction.not_found" => new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status404NotFound
            },
            "emails.message.not_found" => new ProblemDetails
            {
                Title = "Not Found",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status404NotFound
            },
            "emails.message.invalid_status" => new ProblemDetails
            {
                Title = "Bad Request",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status400BadRequest
            },
            "emails.inbox.invalid_status" => new ProblemDetails
            {
                Title = "Bad Request",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status400BadRequest
            },
            _ => new ProblemDetails
            {
                Title = "Bad Request",
                Detail = result.ErrorMessage,
                Status = StatusCodes.Status400BadRequest
            }
        };

        AddCorrelationId(controller, problem);
        return new ObjectResult(problem) { StatusCode = problem.Status };
    }

    private static void AddCorrelationId(ControllerBase controller, ProblemDetails details)
    {
        if (controller.HttpContext.Items.TryGetValue(CorrelationIdMiddleware.HeaderName, out var value) && value is string correlationId)
        {
            details.Extensions["correlationId"] = correlationId;
        }
    }
}


