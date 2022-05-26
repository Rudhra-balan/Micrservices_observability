

using Webhook.DomainCore.Model;

namespace Webhook.Application.Services.Interface
{
    public interface IBashService
    {
        Response Run(string command, Output output = Output.Internal);
    }
}
