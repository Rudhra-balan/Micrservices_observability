
using BuildingBlocks.CQRS.Events;

namespace Transaction.Application.Events
{
    public record BalanceCheckEvent : IEvent
    {

        public int AccountNumber { get; }
        public bool IsSuccessful { get; }
        public decimal Balance { get; }
        public string Currency { get; }
        public string Message { get; }

        public BalanceCheckEvent(int accountNumber, bool isSuccessful, decimal balance, string currency, string message)
        {
            AccountNumber = accountNumber;
            IsSuccessful = isSuccessful;
            Balance = balance;
            Currency = currency;
            Message = message;
        }
    }
}
