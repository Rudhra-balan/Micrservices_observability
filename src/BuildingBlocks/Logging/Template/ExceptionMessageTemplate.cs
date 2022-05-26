using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace BuildingBlocks.Logging.Template
{
    public class ExceptionMessageTemplate
    {
        public DateTime When { get; } = DateTime.Now;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ApplicationMessage { get; set; }

        [JsonProperty("ExceptionMessage")]
        private string _exceptionMessage;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ExceptionMessageTemplate InnerException { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LogMessageTemplate CallerInformation { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StackTrace { get; set; }

        public ExceptionMessageTemplate(System.Exception ex,string applicationMessage = null)
        {
            _exceptionMessage = ex.Message;
            ApplicationMessage = applicationMessage;
            var stackTrace = new StackTrace(ex, true);
            StackTrace = stackTrace.ToString().IsNullOrEmpty()?null: stackTrace.ToString();
            InnerException = ex.InnerException?.CreateReport();
        }
    }

    public static class ExceptionReportExtensionMethods
    {
        public static ExceptionMessageTemplate CreateReport(this System.Exception ex)
        {
            return new(ex);
        }
    }

}
