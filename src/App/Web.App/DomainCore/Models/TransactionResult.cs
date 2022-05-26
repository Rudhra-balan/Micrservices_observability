
using System.ComponentModel;

namespace DomainCore.Models
{
    public class TransactionResult
    {
        public int AccountNumber { get; set; }
        public Money Balance { get; set; }
    }
    public struct Money
    {
        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }

    public enum Currency
    {
        Unknown = 0,

        [Description("United States dollar")]
        USD = 840,

        [Description("Indian rupee")]
        INR = 356
    }
}
