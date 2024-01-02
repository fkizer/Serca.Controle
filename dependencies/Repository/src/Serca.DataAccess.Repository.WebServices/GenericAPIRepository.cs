using Serca.Tools.Extensions;
using Serca.DataAccess.Repository.Abstractions;
using System.Net.Http.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Serca.DataAccess.Exceptions;
using System.Text.Json;
using Serca.DataAccess.Json.Converters;

namespace Serca.DataAccess.Repository.WebServices
{
    public class GenericAPIRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : IAggregateRoot
    {
        public readonly string Endpoint = string.Empty;
        public IReadOnlyDictionary<string, string> Parameters { get; private set; } = new Dictionary<string, string>();

        private readonly IDefaultRepositoryParams? _defaultRepositoryParams;
        private readonly HttpClient _httpClient;
        private JsonSerializerOptions _jsonSerializerOptions;

        public GenericAPIRepository(HttpClient httpClient, IDefaultRepositoryParams? defaultRepositoryParams)
            : this(httpClient, null, defaultRepositoryParams)
        {
        }
        public GenericAPIRepository(HttpClient httpClient, IOptions<ApiRepositoryOptions> options)
            : this(httpClient, options, null)
        {
        }

        public GenericAPIRepository(HttpClient httpClient, IOptions<ApiRepositoryOptions>? options, IDefaultRepositoryParams? defaultRepositoryParams)
        {
            _httpClient = httpClient;
            _defaultRepositoryParams = defaultRepositoryParams;

            if (options != null)
            {
                var endpointApiRepositoryOptions = options.Value.Endpoints.Where(x => x.Resource?.Equals(typeof(TEntity).FullName) ?? false).First();

                if (string.IsNullOrEmpty(endpointApiRepositoryOptions?.Url))
                {
                    throw new Exception("Endpoint url missing");
                }

                if (endpointApiRepositoryOptions.Url.StartsWith("http"))
                {
                    Endpoint = endpointApiRepositoryOptions.Url.Trim('/');
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(options.Value.Base))
                    {
                        Endpoint = options.Value.Base.Trim('/');
                    }

                    Endpoint += "/" + endpointApiRepositoryOptions.Url.Trim('/');
                }
            }

            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new DateTimeConverterUsingDateTimeParse());
        }

        public void SetParameters(Dictionary<string, string> parameters)
        {
            Parameters = parameters;
        }

        private void ClearParameters()
        {
            Parameters = new Dictionary<string, string>();
        }

        public virtual async Task<TEntity?> GetByIdAsync(TKey? id)
        {
            if (string.IsNullOrWhiteSpace(Endpoint)) throw new Exception("Endpoint not found");

            var endpoint = Endpoint.TrimEnd('/');
            if (!EqualityComparer<TKey>.Default.Equals(id, default(TKey)))
            {
                endpoint = endpoint + $"/{id}";
            }

            var endpointWithParams = AddParameters(endpoint);

            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, endpointWithParams);
                AddHeaders(request);
                using var response = await _httpClient
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);
                await EnsureSuccessStatusCode(response);
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TEntity>(content, _jsonSerializerOptions);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Security
                ClearParameters();
            }
        }

        public async Task<IReadOnlyList<TEntity>?> GetAllAsync()
        {
            if (string.IsNullOrWhiteSpace(Endpoint)) throw new Exception("Endpoint not found");

            var endpoint = Endpoint.TrimEnd('/');
            var endpointWithParams = AddParameters(endpoint);

            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, endpointWithParams);
                AddHeaders(request);
                using var response = await _httpClient
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);
                await EnsureSuccessStatusCode(response);

                return JsonSerializer.Deserialize<IReadOnlyList<TEntity>>(await response.Content.ReadAsStringAsync(), _jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                // Security
                ClearParameters();
            }
        }

        private string AddParameters(string url)
        {
            var parameters = _defaultRepositoryParams != null ? Parameters.Union(_defaultRepositoryParams.ToDictionnary()) : Parameters;

            if (parameters?.Count() == 0)
            {
                return url;
            }

            return (url + "?" + parameters?.ToParamAsString()).Trim('/');
        }

        private void AddHeaders(HttpRequestMessage requestl)
        {
            if (_defaultRepositoryParams?.Headers == null)
            {
                return;
            }

            foreach (var header in _defaultRepositoryParams.Headers)
            {
                requestl.Headers.Add(header.Key, header.Value);
            }
        }

        public async Task AddAsync(TEntity entity, TKey? id = default)
        {
            if (string.IsNullOrWhiteSpace(Endpoint)) throw new Exception("Endpoint not found");

            var endpoint = Endpoint.TrimEnd('/');
            if (!EqualityComparer<TKey>.Default.Equals(id, default(TKey)))
            {
                endpoint = endpoint + $"/{id}";
            }

            try
            {
                var endpointWithParams = AddParameters(endpoint);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, endpointWithParams);
                AddHeaders(request);
                string json = JsonSerializer.Serialize(entity);
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;
                    using var response = await _httpClient
                        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false);
                    await EnsureSuccessStatusCode(response);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Security
                ClearParameters();
            }
        }

        public async Task UpdateAsync(TEntity? entity, TKey? id = default)
        {
            if (string.IsNullOrWhiteSpace(Endpoint)) throw new Exception("Endpoint not found");

            var endpoint = Endpoint.TrimEnd('/');
            if (!EqualityComparer<TKey>.Default.Equals(id, default(TKey)))
            {
                endpoint = endpoint + $"/{id}";
            }

            var endpointWithParams = AddParameters(endpoint);
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, endpointWithParams);
                AddHeaders(request);
                string json = JsonSerializer.Serialize(entity);
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;
                    using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                                                      .ConfigureAwait(false);
                    await EnsureSuccessStatusCode(response);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                // Security
                ClearParameters();
            }
        }

        public async Task DeleteAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        private async Task EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                ProblemDetails? problemDetails = null;

                try
                {
                    problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content ?? string.Empty);
                }
                catch (Exception)
                {
                    //Blank (serialization error)
                }

                if (problemDetails != null)
                {
                    throw new ProblemDetailsException(problemDetails);
                }

                // Raise default error
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
