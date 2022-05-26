


using BuildingBlocks.Types;

namespace AccountSummary.Core.Events
{
    public class DepositDomainEvent : IDomainEvent
    {
        public DepositDomainEvent(string name, int accountNumber,decimal balance, string currency)
        {
            AccountNumber = accountNumber;
            Balance = balance;
            Currency = currency; 
            Name = name;
        }

        public string Name { get; set; }
        public int AccountNumber { get; }
        public decimal Balance { get; }
        public string Currency { get; }
      

    }
}
