



using Newtonsoft.Json;

namespace Webhook.DomainCore.Model
{
    public class GrafanaAlert
    {


        public string title { get; set; }
        public long ruleId { get; set; }
        public string ruleName { get; set; }
        public string state { get; set; }
        public List<EvalMatch> evalMatches { get; set; }
        public int orgId { get; set; }
        public int dashboardId { get; set; }
        public int panelId { get; set; }
        public IDictionary<string,string> tags { get; set; }
        public string ruleUrl { get; set; }
        public string message { get; set; }
    }
}
