using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Application.ViewModels
{
    public class SerigProfileViewModel
    {
        public string? ServerHost { get; set; }
        public string? ServerPort { get; set; }
        public string? ServerProtocol { get; set; }
        public string? ServerPath { get; set; }
        public string? ServerName { get; set; }
        public string? SocCode { get; set; }
        public string? ClientId { get; set; }
        public string? Depo { get; set; }
        public string? ServerURL
        {
            get
            {
                var value = ServerBase;

                if (string.IsNullOrEmpty(value)) return null;

                if (!string.IsNullOrEmpty(ServerPath))
                {
                    value += $"/{ServerPath}";
                }

                if (!string.IsNullOrEmpty(SocCode))
                {
                    value += $"/{SocCode}";
                }

                return Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute) ? value : null;
            }
        }

        public string? ServerBase
        {
            get
            {
                var value = $"{ServerProtocol}://{ServerHost}:{ServerPort}";
                return Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute) ? value : null;
            }
        }
    }
}
