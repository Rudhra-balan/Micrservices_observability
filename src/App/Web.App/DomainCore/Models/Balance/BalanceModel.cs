
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DomainCore.Models.Balance
{
    public class BalanceModel
    {
        public BalanceModel()
        {
            TransactionHistory = new List<Transaction>();
        }
        public int AccountNumber { get; set; }
        public Money Balance { get; set; }
        public List<Transaction> TransactionHistory { get; set; }
    }

    public class Transaction
    {
        public string TransactionType { get; set; }
        public DateTime ModifiedDate { get; set; }
        public decimal Amount { get; set; }
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
