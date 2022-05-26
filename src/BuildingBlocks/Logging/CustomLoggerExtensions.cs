

using BuildingBlocks.Logging.Template;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.Logging
{
    public static class CustomLoggerExtensions
    {

        public static string GetMetricsCurrentResourceName(this HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            Endpoint endpoint = httpContext.GetEndpoint();
            return endpoint?.Metadata.GetMetadata<EndpointNameMetadata>()?.EndpointName;
        }
        public static void LogFormatError(this ILogger logger, System.Exception ex, [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0
          )
        {
            if (!logger.IsEnabled(LogLevel.Error)) return;
            var exceptionMessage = new ExceptionMessageTemplate(ex)
            {
                CallerInformation = new LogMessageTemplate
                {
                    MemberName = memberName,
                    Classname = Path.GetFileNameWithoutExtension(sourceFilePath),
                    SourceLineNumber = sourceLineNumber,
                    SourceFilePath = sourceFilePath
                }
            };
            var json = JsonConvert.SerializeObject(exceptionMessage, Formatting.Indented);
            logger.LogError($"{json}", ex);
        }

        public static void LogFormatWarn(this ILogger logger, System.Exception ex,
          [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!logger.IsEnabled(LogLevel.Warning)) return;
            if (ex == null) return;
            var exceptionMessage = new ExceptionMessageTemplate(ex)
            {
                CallerInformation = new LogMessageTemplate
                {
                    MemberName = memberName,
                    Classname = Path.GetFileNameWithoutExtension(sourceFilePath),
                    SourceLineNumber = sourceLineNumber,
                    SourceFilePath = sourceFilePath
                }
            };
            var json = JsonConvert.SerializeObject(exceptionMessage, Formatting.Indented);
            logger.LogWarning($"{json}", ex);
        }

        public static void LogFormatWarn(this ILogger logger, string message = "Warning",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!logger.IsEnabled(LogLevel.Warning)) return;
            var logMessage = new LogMessageTemplate
            {
                MemberName = memberName,
                Classname = Path.GetFileNameWithoutExtension(sourceFilePath),
                SourceLineNumber = sourceLineNumber,
                SourceFilePath = sourceFilePath,
                Message = message
            };
            var json = JsonConvert.SerializeObject(logMessage, Formatting.Indented);
            logger.LogWarning($"{json}");
        }

        public static void LogFormatDebug(this ILogger logger, System.Exception ex,
          [CallerMemberName] string memberName = "",
          [CallerFilePath] string sourceFilePath = "",
          [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!logger.IsEnabled(LogLevel.Debug)) return;
            if (ex == null) return;
            var exceptionMessage = new ExceptionMessageTemplate(ex)
            {
                CallerInformation = new LogMessageTemplate
                {
                    MemberName = memberName,
                    Classname = Path.GetFileNameWithoutExtension(sourceFilePath),
                    SourceLineNumber = sourceLineNumber,
                    SourceFilePath = sourceFilePath
                }
            };
            var json = JsonConvert.SerializeObject(exceptionMessage, Formatting.Indented);
            logger.LogDebug($"{json}", ex);
        }

        public static void LogFormatDebug(this ILogger logger, string message = "Debug",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!logger.IsEnabled(LogLevel.Debug)) return;
            var logMessage = new LogMessageTemplate
            {
                MemberName = memberName,
                Classname = Path.GetFileNameWithoutExtension(sourceFilePath),
                SourceLineNumber = sourceLineNumber,
                SourceFilePath = sourceFilePath,
                Message = message
            };
            var json = JsonConvert.SerializeObject(logMessage, Formatting.Indented);
            logger.LogDebug($"{json}");
        }

        public static void LogFormatInfo(this ILogger logger, System.Exception ex,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!logger.IsEnabled(LogLevel.Information)) return;
            if (ex == null) return;
            var exceptionMessage = new ExceptionMessageTemplate(ex)
            {
                CallerInformation = new LogMessageTemplate
                {
                    MemberName = memberName,
                    Classname = Path.GetFileNameWithoutExtension(sourceFilePath),
                    SourceLineNumber = sourceLineNumber,
                    SourceFilePath = sourceFilePath
                }
            };
            var json = JsonConvert.SerializeObject(exceptionMessage, Formatting.Indented);
            logger.LogInformation($"{json}", ex);
        }

        public static void LogFormatInfo(this ILogger logger, string message = "Information",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!logger.IsEnabled(LogLevel.Information)) return;
            var logMessage = new LogMessageTemplate
            {
                MemberName = memberName,
                Classname = Path.GetFileNameWithoutExtension(sourceFilePath),
                SourceLineNumber = sourceLineNumber,
                SourceFilePath = sourceFilePath,
                Message = message
            };
            var json = JsonConvert.SerializeObject(logMessage, Formatting.Indented);
            logger.LogInformation($"{json}");
        }

    }

}

