using App.Application;
using App.Application.Contracts.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace App.API.Filters;

public class IdCheckFilter<T>(IGenericRepository<T> genericRepository) : Attribute, IAsyncActionFilter where T : class
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ActionArguments.TryGetValue("id", out var idValue) ||
            !int.TryParse(idValue?.ToString(), out var id))
        {
            await next();
            return;
        }

        if (!await genericRepository.AnyAsync(id))
        {
            var entityName = typeof(T).Name;
            var endpointName = context.ActionDescriptor.RouteValues["action"];

            context.Result = new NotFoundObjectResult(
                ServiceResult.Fail($"{entityName} with ID {id} not found ({endpointName})"));
            return;
        }

        await next();
    }
}