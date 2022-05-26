using DomainCore.Extensions;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Extensions
{
    public enum ErrorEnum
    {

        #region Exception Errors

        [Description("The user is not authorized to make the request. Please try again.")]
        UnAuthorized = HttpStatusCode.Unauthorized,
        [Description("The specified URI does not exist. Please verify and try again.")]
        NotFound = HttpStatusCode.NotFound,
        [Description("The specified URI does not contain any content.")]
        NoContent = HttpStatusCode.NoContent,
        [Description("Invalid request or improperly formed.Please verify and try again.")]
        BadRequest = HttpStatusCode.BadRequest,
        [Description("There was a conflict between the supplied data and the existing resource.")]
        Conflict = HttpStatusCode.Conflict,
        [Description("The URI requested by the client is longer than the server is willing to interpret.")]
        RequestUriTooLong = HttpStatusCode.RequestUriTooLong,
        [Description("The media format of the requested data is not supported by the server, so the server is rejecting the request.")]
        UnsupportedMediaType = HttpStatusCode.UnsupportedMediaType,
        [Description("Server does not recognize the request method and is not capable of supporting it for any resource")]
        NotImplemented = HttpStatusCode.NotImplemented,
        [Description("The requested resource has not been modified")]
        NotModified = HttpStatusCode.NotModified,
        [Description("Access Denied You don't have permission to access. please contact system administrator.")]
        Forbidden = HttpStatusCode.Forbidden,
        [Description("Unknown services error has occurred. please contact system administrator.")]
        UnknownApiError = HttpStatusCode.InternalServerError,
        [Description("The file is locked")]
        IoException,
        [Description("An Collection index is outside the lower or upper bounds of an array or collection.")]
        IndexOutOfRangeException,
        [Description("Access members of null object.")]
        NullReferenceException,
        AccessViolationException,
        [Description("A method call is invalid in an object's current state.")]
        InvalidOperationException,
        [Description("A non-null argument that is passed to a method is invalid.")]
        ArgumentException,
        [Description("An argument that is passed to a method is null.")]
        ArgumentNullException,
        [Description("An argument is outside the range of valid values.")]
        ArgumentOutOfRangeException,
        [Description("Raised when an integer value is divide by zero.")]
        DivideByZeroException,
        [Description("A physical file does not exist at the specified location.")]
        FileNotFoundException,
        [Description("A value is not in an appropriate format to be converted from a string by a conversion method such as Parse.")]
        FormatException,
        [Description("The specified key for accessing a member in a collection cannot be found.")]
        KeyNotFoundException,
        [Description("A method or operation is not supported.")]
        NotSupportedException,
        [Description("An arithmetic, casting, or conversion operation results in an overflow.")]
        OverflowException,
        [Description("Does not get enough memory to execute the code.")]
        OutOfMemoryException,
        [Description("A stack in memory overflows.")]
        StackOverflowException,
        [Description("The time interval allotted to an operation has expired.")]
        TimeoutException,
        [Description("An operation is performed on an object that has been disposed.")]
        ObjectDisposedException,
        [Description("A path or file name exceeds the maximum system-defined length.")]
        PathTooLongException,
        [Description("An invalid Uniform Resource Identifier (URI) is used.")]
        UriFormatException,
        [Description("An instance of one type to another type is not supported or Invalid in an object's current state")]
        InvalidCastException



        #endregion
    }
    public static class ExpectionFilterExtenstion
    {
        public static Int32 ToInt<T>(this T e) where T : IConvertible
        {
            int value = 0;

            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        value = val;

                        break;
                    }
                }
            }
            return value;
        }
        /// <summary>
        /// Returns a list of all the exception messages from the top-level
        /// exception down through all the inner exceptions. Useful for making
        /// logs and error pages easier to read when dealing with exceptions.
        /// Usage: Exception.Messages()
        /// </summary>
        public static IEnumerable<string> GetExceptionMessages(this Exception ex)
        {
            // return an empty sequence if the provided exception is null
            if (ex == null) { yield break; }
            // first return THIS exception's message at the beginning of the list
            yield return ex.Message;
            // then get all the lower-level exception messages recursively (if any)
            IEnumerable<Exception> innerExceptions = Enumerable.Empty<Exception>();

            if (ex is AggregateException aggregateException && aggregateException.InnerExceptions.Any())
            {
                innerExceptions = aggregateException.InnerExceptions;
            }
            else if (ex.InnerException != null)
            {
                innerExceptions = new[] { ex.InnerException };
            }

            foreach (var innerEx in innerExceptions)
            {
                foreach (string msg in GetExceptionMessages(innerEx))
                {
                    yield return msg;
                }
            }
        }
        public static string GetExceptionMessage(this Exception exception)
        {

            switch (exception)
            {
                case NotImplementedException _:
                    return ErrorEnum.NotImplemented.GetDescription();
                case IndexOutOfRangeException _:
                    return ErrorEnum.IndexOutOfRangeException.GetDescription();
                case NullReferenceException _:
                    return ErrorEnum.NullReferenceException.GetDescription();
                case AccessViolationException _:
                    return ErrorEnum.AccessViolationException.GetDescription();
                case ObjectDisposedException _:
                    return ErrorEnum.ObjectDisposedException.GetDescription();
                case UriFormatException _:
                    return ErrorEnum.UriFormatException.GetDescription();
                case PathTooLongException _:
                    return ErrorEnum.PathTooLongException.GetDescription();
                case InvalidOperationException _:
                    return ErrorEnum.InvalidOperationException.GetDescription();
                case ArgumentNullException _:
                    return ErrorEnum.ArgumentNullException.GetDescription();
                case ArgumentOutOfRangeException _:
                    return ErrorEnum.ArgumentOutOfRangeException.GetDescription();
                case ArgumentException _:
                    return ErrorEnum.ArgumentException.GetDescription();
                case UnauthorizedAccessException _:
                    return
                        "UnAuthorizedAccessException : Unable to access file or The file is read-only or . Unauthorized Access";
                case DirectoryNotFoundException directoryNotFoundException:
                    return $"Directory not found: {directoryNotFoundException.Message}";
                case FileNotFoundException _:
                    return ErrorEnum.FileNotFoundException.GetDescription();
                case IOException ioException:
                    return (ioException.HResult & 0x0000FFFF) == 32
                        ? "File sharing violation.Please try again later"
                        : ErrorEnum.IoException.GetDescription();
                case DivideByZeroException _:
                    return ErrorEnum.DivideByZeroException.GetDescription();
                case FormatException _:
                    return ErrorEnum.FormatException.GetDescription();
                case KeyNotFoundException _:
                    return ErrorEnum.KeyNotFoundException.GetDescription();
                case NotSupportedException _:
                    return ErrorEnum.NotSupportedException.GetDescription();
                case OverflowException _:
                    return ErrorEnum.OverflowException.GetDescription();
                case OutOfMemoryException _:
                    return ErrorEnum.OutOfMemoryException.GetDescription();
                case StackOverflowException _:
                    return ErrorEnum.StackOverflowException.GetDescription();
                case TimeoutException _:
                    return ErrorEnum.TimeoutException.GetDescription();
              
                
                case JsonException _:
                    return ErrorEnum.FormatException.GetDescription();
                case DataException _:
                    return ErrorEnum.InvalidCastException.GetDescription();
              
    
                default:
                    return exception.GetBaseException().GetType().Name;

            }
        }

    }
}
