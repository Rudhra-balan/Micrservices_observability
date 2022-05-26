

namespace Transaction.Core.Entities
{
    public class AccountTransactionEntity
    {
      
        public DateTime ModifiedDate { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
  
    }
}
