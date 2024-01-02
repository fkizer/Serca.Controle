using Serca.Authentication.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Authentication
{
    public class CredentialsDTO : ICredentials
    {
        public string? Userm { get; set; }

        public string? Password { get; set; }
    }
}
