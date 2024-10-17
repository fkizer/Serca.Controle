using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Application.Abstraction.Services
{
    public interface IStorageService
    {
        public T? Get<T>(string key);
        public Task<T?> GetAsync<T>(string key);
        public bool Save<T>(T resource, string key);
        public Task<bool> SaveAsync<T>(T resource, string key);
        public bool Remove(string key);
        public Task<bool> RemoveAsync(string key);
    }
}
