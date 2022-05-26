
namespace BuildingBlocks
{
    public static class ResponseMessage
    {
        public const string UnAuthorized = "The user is not authorized to make the request. Please try again.";
        public const string TokenExpired = "Token Expired. Please obtain a new token.";
        public const string AccessViolationException = "An attempt to access protected memory that is, to access memory that is not allocated or that is not owned by a process.";
        public const string UnknownApiError = "Unknown API Services error has occurred. please contact system administrator.";
    }
    public static class DiagnosticsConstant
    {
        public static long Requests = 0;
    }
}
