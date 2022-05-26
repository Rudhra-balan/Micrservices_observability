using System;
using System.Net;


namespace DomainCore.Exceptions
{
    [Serializable]
    public class NotFoundException : WebException
    {
       
        public NotFoundException(string message,HttpStatusCode statusCode)
             : base(message, statusCode)
        { }
    }
}
