using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Authentication.Abstractions
{
    public interface IAuthenticationManager
    {
        Task<ICredentials?> InitializedAsync(IInitializationParameters? initializationRequestParameters);
    }
}
