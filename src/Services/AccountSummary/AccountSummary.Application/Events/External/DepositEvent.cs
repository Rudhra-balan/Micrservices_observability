
using BuildingBlocks.CQRS.Events;

namespace AccountSummary.Application.Events
{
    public record DepositEvent : IEvent
    {

        public int AccountNumber { get; }
        public bool IsSuccessful { get; }
        public decimal Balance { get; }
        public string Currency { get; }
        public string Message { get; }

        public DepositEvent(int accountNumber, bool isSuccessful, decimal balance, string currency, string message)
        {
            AccountNumber = accountNumber;
            IsSuccessful = isSuccessful;
            Balance = balance;
            Currency = currency;
            Message = message;
        }
    }
}
