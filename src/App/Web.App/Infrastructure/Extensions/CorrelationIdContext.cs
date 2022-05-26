using System;
using System.Threading;

namespace Infrastructure.Extensions
{
    public class CorrelationIdContext
    {
        private static readonly AsyncLocal<string> _correlationId = new AsyncLocal<string>();

        public static void SetCorrelationId(string correlationId)
        {
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                throw new ArgumentException("Correlation Id cannot be null or empty", nameof(correlationId));
            }

            if (!string.IsNullOrWhiteSpace(_correlationId.Value))
            {
                throw new InvalidOperationException("Correlation Id is already set for the context");
            }

            _correlationId.Value = correlationId;
        }

        public static string GetCorrelationId()
        {
            return _correlationId.Value;
        }
    }
}
