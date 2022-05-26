
using DomainCore.Models.Balance;


namespace DomainCore.Models.Transaction
{
    public class TransactionModel
    {
        public int AccountNumber { get; set; }
        public Money Balance { get; set; }

        public TransactionType TransactionType { get; set; }
    }

    public enum TransactionType
    {
        Unknown = 0,

        
        Deposit = 1,

   
        Withdraw = 2
    }
}
