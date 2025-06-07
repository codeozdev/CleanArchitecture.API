using App.Application;
using App.Application.Contracts.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace App.API.Filters;

public class NameCheckFilter<T>(IGenericRepository<T> repository) : Attribute, IAsyncActionFilter
    where T : class
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ActionArguments.TryGetValue("request", out var requestValue))
        {
            await next();
            return;
        }

        var requestType = requestValue?.GetType();
        var nameProperty = requestType?.GetProperty("Name");
        if (nameProperty == null)
        {
            await next();
            return;
        }

        var nameValue = nameProperty.GetValue(requestValue)?.ToString();
        if (string.IsNullOrEmpty(nameValue))
        {
            await next();
            return;
        }

        var nameExists = await repository.AnyAsync(x =>
            EF.Property<string>(x, "Name") == nameValue);

        if (nameExists)
        {
            var entityName = typeof(T).Name;
            var endpointName = context.ActionDescriptor.RouteValues["action"];

            context.Result = new ConflictObjectResult(
                ServiceResult.Fail($"{entityName} with name '{nameValue}' already exists ({endpointName})",
                    HttpStatusCode.Conflict));
            return;
        }

        await next();
    }
}