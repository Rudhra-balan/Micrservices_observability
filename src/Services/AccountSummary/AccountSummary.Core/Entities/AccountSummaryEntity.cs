

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountSummary.Core.Entities
{
    [Table("AccountSummary", Schema = "dbo")]
    public class AccountSummaryEntity
    {
        [Key]
        public int AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
      
        public ICollection<AccountTransaction> AccountTransactions { get; set; }
    }


    [Table("AccountTransaction")]
    public class AccountTransaction
    {
        [Key] public int TransactionId { get; set; }
        [ForeignKey("AccountNumber")] public int AccountNumber { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Description { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public AccountSummaryEntity AccountSummary { get; set; }
    }
}
