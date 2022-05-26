


using DomainCore.Models;
using DomainCore.Models.Response;
using System.Threading.Tasks;

namespace Application.Services.Interface
{
    public interface ITransaction
    {
     
        Task<WebClientResponse> DepositAsync(TransactionInput transactionDetails);

        Task<WebClientResponse> WithdrawAsync(TransactionInput transactionDetails);
    }
}
