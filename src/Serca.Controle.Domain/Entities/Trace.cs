using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Domain.Entities
{
    public class Trace
    {
        public string Id { get; set; }
        public int Code { get; set; }
        public DateTime Date { get; set; }
        public int Group { get; set; }
        public string Content { get; set; }

        public static Trace CreateTrace(string message, int group = 1, int code = 1)
            => new Trace() { Id = Guid.NewGuid().ToString(), Date = DateTime.Now, Group = group, Code = code, Content = message };
    }
}
