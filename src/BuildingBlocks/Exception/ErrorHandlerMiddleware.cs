
using System.Net;
using System.Text.Json;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Prometheus;
using Prometheus.DotNetRuntime;

namespace BuildingBlocks.Exception;

public class ErrorHandlerMiddleware : IMiddleware
{
    private readonly ICapPublisher _capPublisher;
    private readonly IExceptionToMessageMapper _exceptionToMessageMapper;
    private readonly IExceptionToResponseMapper _exceptionToResponseMapper;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;
    private static readonly Counter ExceptionCounter = Prometheus.Metrics
    .CreateCounter("request_exception_total", "Operations that failed.", labelNames: new[] { "Path", "Exception" });
    public ErrorHandlerMiddleware(IExceptionToResponseMapper exceptionToResponseMapper,
        IExceptionToMessageMapper exceptionToMessageMapper,
        ICapPublisher capPublisher,
        ILogger<ErrorHandlerMiddleware> logger)
    {
        _exceptionToResponseMapper = exceptionToResponseMapper;
        _exceptionToMessageMapper = exceptionToMessageMapper;
        _capPublisher = capPublisher;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var dotNetRuntimeStatsBuilder = DotNetRuntimeStatsBuilder.Default().StartCollecting();
        var diagnosticSourceAdapter = DiagnosticSourceAdapter.StartListening();
        var meterAdapter = MeterAdapter.StartListening();
        try
        {
        
            await next(context);
        }
        catch (System.Exception exception)
        {
            ExceptionCounter.WithLabels(context.Request.Path, exception.GetBaseException().GetType().Name).Inc();
            _logger.LogError(exception, exception.Message);
            await HandleErrorAsync(context, exception);
        }
        finally
        {
            dotNetRuntimeStatsBuilder.Dispose();
            diagnosticSourceAdapter.Dispose();
            meterAdapter.Dispose();

        }
    }

    private async Task HandleErrorAsync(HttpContext context, System.Exception exception)
    {
        var response = context.Response;
        context.Response.ContentType = "application/json";

        var rejectedEvent = _exceptionToMessageMapper.Map(exception, exception.Message);
        if (rejectedEvent != null)
            await _capPublisher.PublishAsync(rejectedEvent.GetType().Name, rejectedEvent);

        var exceptionResponse = _exceptionToResponseMapper.Map(exception);
        context.Response.StatusCode = (int) (exceptionResponse?.StatusCode ?? HttpStatusCode.BadRequest);
        if (exceptionResponse is null)
        {
            await context.Response.WriteAsync(string.Empty);
            return;
        }

        var result = JsonSerializer.Serialize(exceptionResponse);
        await response.WriteAsync(result);
    }
}