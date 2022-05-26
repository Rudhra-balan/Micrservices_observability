

using System.Text.Json.Serialization;

namespace Webhook.DomainCore.Model
{
    public class EvalMatch
    {

        public int value { get; set; }
        public string metric { get; set; }
        public object? tags { get; set; }
    }
}
