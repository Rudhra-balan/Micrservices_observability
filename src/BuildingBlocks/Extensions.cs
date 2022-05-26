using BuildingBlocks.Exception;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BuildingBlocks;

public static class Extensions
{
    private const string SectionName = "app";

    public static IServiceCollection AddErrorHandler<T>(this IServiceCollection services)
        where T : class, IExceptionToResponseMapper
    {
        services.AddTransient<ErrorHandlerMiddleware>();
        services.AddSingleton<IExceptionToResponseMapper, T>();

        return services;
    }

    public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlerMiddleware>();
    }

    public static TModel GetOptions<TModel>(this IConfiguration configuration, string sectionName)
        where TModel : new()
    {
        var model = new TModel();
        configuration.GetSection(sectionName).Bind(model);
        return model;
    }

    public static TModel GetOptions<TModel>(this IServiceCollection services, string settingsSectionName)
        where TModel : new()
    {
        using var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetService<IConfiguration>();
        return configuration.GetOptions<TModel>(settingsSectionName);
    }

    public static bool IsNullOrEmpty(this string value)
    {
        return value == null || value.Trim().Length == 0;
    }

    public static string ToJson(this object rawJson)
    {
        return rawJson == null ? string.Empty : JsonConvert.SerializeObject(rawJson, JsonSettings());
    }
    private static JsonSerializerSettings JsonSettings()
    {
        return new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.None,
            Culture = CultureInfo.CurrentUICulture

        };
    }

    public static string ToCamelCase(this string value)
    {
        var pattern = new Regex(@"[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+");
        return new string(
            new CultureInfo("en-US", false)
                .TextInfo
                .ToTitleCase(
                    string.Join(" ", pattern.Matches(value)).ToLower()
                )
                .Replace(@" ", "")
                .Select((x, i) => i == 0 ? char.ToLower(x) : x)
                .ToArray()
        );
    }

    public static string GetExceptionMessage(this System.Exception exception)
    {
        return exception switch
        {
           
            NotImplementedException => "Server does not recognize the request method and is not capable of supporting it for any resource.",
            IndexOutOfRangeException => "An collection index is outside the lower or upper bounds of an array or collection.",
            NullReferenceException => "Access members of null object.",
            AccessViolationException => "An attempt to access protected memory that is, to access memory that is not allocated or that is not owned by a process.",
            ObjectDisposedException => "An operation is performed on an object that has been disposed.",
            UriFormatException => "An invalid uniform resource identifier (URI) is used.",
            PathTooLongException => "A path or file name exceeds the maximum system-defined length.",
            InvalidOperationException => "A method call is invalid in an object's current state.",
            ArgumentNullException => "An argument that is passed to a method is null.",
            ArgumentOutOfRangeException => "An argument is outside the range of valid values.",
            ArgumentException => "A non-null argument that is passed to a method is invalid.",
            UnauthorizedAccessException => "Unable to access file or the file is read-only.",
            DirectoryNotFoundException directoryNotFoundException => "Could not find the specified directory. Please try again.",
            FileNotFoundException => "A physical file does not exist at the specified location.",
            IOException ioException => ((ioException.HResult & 0x0000FFFF) == 32 ? "An attempt to open a file that is being used by another program" : "The file is locked"),
            DivideByZeroException => "Raised when an integer value is divide by zero.",
            FormatException => "A value is not in an appropriate format to be converted from a string by a conversion method such as Parse.",
            KeyNotFoundException => "The specified key for accessing a member in a collection cannot be found.",
            NotSupportedException => "A method or operation is not supported.",
            OverflowException => "An arithmetic, casting, or conversion operation results in an overflow.",
            OutOfMemoryException => "Does not get enough memory to execute the code.",
            StackOverflowException => "A stack in memory overflows.",
            TimeoutException => "The time interval allotted to an operation has expired.",
            SqliteException => "The server encountered an internal error or misconfiguration and was unable to complete your request. Please contact the server administrator.",
            _ => ResponseMessage.UnknownApiError
        };
    }

}