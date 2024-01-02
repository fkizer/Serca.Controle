using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.DataAccess.Repository.Abstractions
{
    public interface IDefaultRepositoryParams
    {
        public Dictionary<string, string> ToDictionnary();
        public Dictionary<string, string>? Headers { get; }
    }
}
