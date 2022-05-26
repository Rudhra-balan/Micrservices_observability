
namespace Transaction.Application.Exceptions
{
    public class InvalidCurrencyException : AppException
    {
        public InvalidCurrencyException(string currency)
            : base($"This operation can not be performed with {currency} currency.",System.Net.HttpStatusCode.NotAcceptable)
        { }

     
    }

}
