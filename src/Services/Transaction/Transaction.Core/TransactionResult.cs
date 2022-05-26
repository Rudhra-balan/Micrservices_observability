
using System.ComponentModel;
using System.Globalization;
using Transaction.Core.Entities;

namespace Transaction.Core
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

    public static class TransactionResultExtensions
    {
        public static TransactionResult ToResult(this AccountSummaryEntity p)
        {
            return new TransactionResult()
            {
                AccountNumber = p.AccountNumber,
                Balance = new Money(p.Balance, ((Currency)Enum.Parse(typeof(Currency), p.Currency, true)).GetDescription())
            };
        }

        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            string description = null;

            if (e is not Enum) return null;
            var type = e.GetType();
            var values = Enum.GetValues(type);

            foreach (int val in values)
            {
                if (val != e.ToInt32(CultureInfo.InvariantCulture)) continue;
                var memInfo = type.GetMember(type.GetEnumName(val) ?? string.Empty);
                var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (descriptionAttributes.Length > 0)
                    // we're only getting the first description we find
                    // others will be ignored
                    description = ((DescriptionAttribute)descriptionAttributes[0]).Description;

                break;
            }

            return description;
        }
    }
}
