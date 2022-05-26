
namespace Webhook.Application.Constants
{
    public static class  BashCommands
    {
        public static string GetServiceRunningStatus(string serviceName) => String.Format("systemctl is-active --quiet microservice-balance.service && echo running", serviceName);

        public const string BalanceService = "microservice-balance.service";
        public const string TransactionService = "microservice-Transaction.service";
        
    }
}
