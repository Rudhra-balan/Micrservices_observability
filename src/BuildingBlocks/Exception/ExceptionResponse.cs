using System.Net;

namespace BuildingBlocks.Exception;

public class ExceptionResponse
{

    public string Response { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}