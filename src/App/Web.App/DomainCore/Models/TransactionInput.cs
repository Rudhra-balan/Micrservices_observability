


using DomainCore.Models.Transaction;

namespace DomainCore.Models
{
    public class TransactionInput
    {
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
    }

}
