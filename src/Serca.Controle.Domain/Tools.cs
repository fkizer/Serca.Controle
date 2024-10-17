using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Domain
{
    public static class Tools
    {
        public static readonly Dictionary<string, string> Erps = new() { { "BDM", "bevdemo" }, { "B1P", "b1" }, { "B2P", "b2" } };

        public static string ExtractErpId(this string? code)
        {
            if (!String.IsNullOrEmpty(code))
            {
                foreach (var item in Erps)
                {
                    if (code.ToUpper()[..3].Equals(item.Key))
                    {
                        return item.Value;
                    }
                }
            }

            throw new Exception("Code ERP non reconnu");
        }
    }
}
