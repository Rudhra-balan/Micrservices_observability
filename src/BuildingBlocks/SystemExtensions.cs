using System.Linq;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks
{
    public static class SystemExtensions
    {
        public static string Underscore(this string value)
            => string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));
        
      
    }
}