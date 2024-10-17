using MediatR;
using Serca.Controle.Core.Application.Constants;
using Serca.Controle.Core.Application.UseCases.Shared;
using Serca.Controle.Core.Domain.Entities.DiagAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Application.UseCases.RunDiag
{
    public class RunDiagCommand : IRequest<Result<bool>>
    {
        public DiagServiceEntity? Service { get; set; }
        public bool HttpCodeShouldBeValid { get; set; }
    }

    public class RunDiagCommandHandler : IRequestHandler<RunDiagCommand, Result<bool>>
    {
        private readonly HttpClient HttpClient;
        public RunDiagCommandHandler(IHttpClientFactory httpClientFactory)
        {
            HttpClient = httpClientFactory.CreateClient(ApplicationConstants.DiagHttpClientName);
        }

        public async Task<Result<bool>> Handle(RunDiagCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Service?.Uri))
            {
                return new Result<bool>()
                {
                    Success = false,
                    Message = "Url non renseignée"
                };
            }

            HttpResponseMessage? httpResponseMessage = null;
            var req = new HttpRequestMessage()
            {
                RequestUri = new Uri(request.Service.Uri),
            };

            try
            {
                ; httpResponseMessage = await HttpClient.SendAsync(req);

                if (request.HttpCodeShouldBeValid)
                {
                    httpResponseMessage.EnsureSuccessStatusCode();
                }
            }
            catch (Exception)
            {
                return new Result<bool>()
                {
                    Success = false,
                    Message = "Service injoignable"
                };
            }

            return new Result<bool>()
            {
                Success = true
            };
        }
    }
}
