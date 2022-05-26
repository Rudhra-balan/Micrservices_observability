
namespace Transaction.Core.Entities
{
 
    public class AccountSummaryEntity
    {
        public AccountSummaryEntity()
        {
            AccountTransactions = new List<AccountTransactionEntity>();
        }
        public int AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }

        public ICollection<AccountTransactionEntity> AccountTransactions { get; set; }
    }
}
