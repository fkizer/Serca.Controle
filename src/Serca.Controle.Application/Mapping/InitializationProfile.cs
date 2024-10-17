using AutoMapper;
using Serca.Controle.Core.Application.ViewModels;
using Serca.Controle.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Serca.Controle.Core.Application.Mapping
{
    public class InitializationProfile : Profile
    {
        public InitializationProfile()
        {
            CreateMap<DeviceParametersEntity, DeviceParameters>();
            CreateMap<UtilisateurEntity, UtilisateurViewModel>();
        }
    }
}
