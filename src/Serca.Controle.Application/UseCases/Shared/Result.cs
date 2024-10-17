using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Application.UseCases.Shared
{
    public class Result<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
