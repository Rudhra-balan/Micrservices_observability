using System;

namespace Web.DomainCore.Model
{
    public class Response
    {
        public int Code { get; set; }
        public string Stdout { get; set; }
        public string Stderr { get; set; }

        public Exception? Exception { get; set; } = null;
    }
}