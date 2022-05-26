

using Web.DomainCore.Model;

namespace Web.Application.Services.Interface
{
    public interface IBashService
    {
        Response Run(string command, Output output = Output.Internal);
    }
}
