using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace BuildingBlocks.Exception
{
    public class ValidationResult : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            var modelStateEntries = context.ModelState.Where(e => e.Value.Errors.Count > 0).ToArray();
            var errors = new List<ExceptionResponse>();
            var error = new ExceptionResponse();


            if (modelStateEntries.Any())
            {
                if (modelStateEntries.Length == 1 && modelStateEntries[0].Value.Errors.Count == 1 &&
                    modelStateEntries[0].Key == string.Empty)
                    error.Response = modelStateEntries[0].Value.Errors[0].ErrorMessage;
                else
                    foreach (var modelStateEntry in modelStateEntries)
                        errors.AddRange(modelStateEntry.Value.Errors.Select(modelStateError => new ExceptionResponse
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Response = modelStateError.ErrorMessage
                        }));
            }

            if (!error.Response.ToString().IsNullOrEmpty()) errors.Add(error);

            context.HttpContext.Response.Headers.Add(nameof(ValidationResult), "true");
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var errorMessage = new Errorlist { Errors = errors };
            context.HttpContext.Response.ContentType = "application/json; charset=utf-8";
            context.HttpContext.Response.WriteAsync(errorMessage.ToJson());
            return Task.CompletedTask;
        }
    }
}
