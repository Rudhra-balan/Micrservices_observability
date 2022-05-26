
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Metrics.Services
{
    public class MonitorAttribute : Attribute { }

    public interface IMonitoringService
    {
        bool Monitor(string httpMethod, PathString path);
    }
}
