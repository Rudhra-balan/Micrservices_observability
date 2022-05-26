


using DomainCore.Models;
using DomainCore.Models.Balance;
using System.Threading.Tasks;

namespace Application.Services.Interface
{
    public interface IBalance
    {
        Task<BalanceModel> BalanceAsync();

      
    }
}
