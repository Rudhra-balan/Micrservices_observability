using System.Collections.Generic;
using NetEscapades.AspNetCore.SecurityHeaders.Headers;

namespace Web.Common.HeaderPolicy
{
    public class HeaderPolicyCollection : Dictionary<string, IHeaderPolicy>
    {
    }
}