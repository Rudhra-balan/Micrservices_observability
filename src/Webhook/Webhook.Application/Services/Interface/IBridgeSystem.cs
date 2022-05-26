using Webhook.DomainCore.Model;

namespace Webhook.Application.Services.Interface
{
    public interface IBridgeSystem
    {
        string GetFileName();
        string[] CommandConstructor(string command, Output? output = Output.Hidden, string dir = "");
        
    }
}