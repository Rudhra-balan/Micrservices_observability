using Web.DomainCore.Model;

namespace Web.Application.Services.Interface
{
    public interface IBridgeSystem
    {
        string GetFileName();
        string[] CommandConstructor(string command, Output? output = Output.Hidden, string dir = "");
        
    }
}