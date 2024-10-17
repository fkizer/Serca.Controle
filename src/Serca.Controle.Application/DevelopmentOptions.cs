using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Application
{
    /// <summary>
    /// Class used only in development environment, to set environment preferences from configuration
    /// </summary>
    public class DevelopmentOptions
    {
        public bool UseFakeAuthentication { get; set; }
    }
}
