using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.DataAccess.Repository.Abstractions
{
    public class ApiRepositoryOptions
    {
        public string? Base{ get; set; }
        public List<EndpointApiRepositoryOptions> Endpoints { get; set; } = new();
    }

    public class EndpointApiRepositoryOptions
    {
        public string? Resource { get; set; }
        public string? Url { get; set; }
    }
}
