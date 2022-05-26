

using DomainCore.Models;
using System.Threading.Tasks;

namespace Application.Services.Interface
{
    public interface IAuthentication
    {
        Task<TokenModel> Login(string userName, string password);
    }
}
