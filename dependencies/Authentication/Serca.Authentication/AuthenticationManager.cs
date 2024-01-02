using AutoMapper;
using Serca.Authentication.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Authentication
{
    public class AuthenticationManager : IAuthenticationManager
    {
        protected readonly IInitalizer Initializer;

        public AuthenticationManager(IInitalizer initalizer)
        {
            Initializer = initalizer;
        }

        public async Task<ICredentials?> InitializedAsync(IInitializationParameters? initializationRequestParameters)
        {
            var credentialsEntity = await Initializer.GetCredentialsAsync(initializationRequestParameters);

            if (credentialsEntity == null)
            {
                return null;
            }

            var config = new MapperConfiguration(config => config.CreateMap<CredentialsEntity, CredentialsDTO>());
            var mapper = new Mapper(config);

            return mapper.Map<CredentialsEntity, CredentialsDTO>(credentialsEntity);
        }
    }
}
