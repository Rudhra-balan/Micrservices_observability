
using System.Net;

namespace AccountSummary.Core.Exceptions;

public  class CurrencyMismatchException : AccountSummaryExceptions
{
    public CurrencyMismatchException(Currency c1, Currency c2)
           : base($"This operation cannot be performed between {c1} and {c2}.", HttpStatusCode.BadRequest)
    { }

   
}