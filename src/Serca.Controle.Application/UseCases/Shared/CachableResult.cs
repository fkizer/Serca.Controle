using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Application.UseCases.Shared
{
    public class CachableResult<T> : Result<T>
    {
        public bool FromCache { get; set; }
    }
}
