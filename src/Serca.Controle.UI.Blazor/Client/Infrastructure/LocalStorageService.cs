

using Blazored.LocalStorage;
using Serca.Controle.Core.Application.Abstraction.Services;

namespace Serca.Controle.UI.Blazor.Client.Infrastructure
{
    public class LocalStorageService : IStorageService
    {
        protected readonly ILocalStorageService ILocalStorageService;
        protected readonly ISyncLocalStorageService SyncLocalStorageService;

        public LocalStorageService(ILocalStorageService localStorageService, ISyncLocalStorageService syncLocalStorageService)
        {
            ILocalStorageService = localStorageService;
            SyncLocalStorageService = syncLocalStorageService;
        }

        public T? Get<T>(string key)
        {
            try
            {
                return SyncLocalStorageService.GetItem<T>(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : LocalStorageService - Get() - {ex.Message}");
                return default;
            }
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                return await ILocalStorageService.GetItemAsync<T>(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : LocalStorageService - GetAsync() - {ex.Message}");
                return default;
            }
        }

        public async Task<bool> SaveAsync<T>(T resource, string key)
        {
            try
            {
                await ILocalStorageService.SetItemAsync(key, resource);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : LocalStorageService - SaveAsync() - {ex.Message}");
                return false;
            }
        }

        public bool Save<T>(T resource, string key)
        {
            try
            {
                SyncLocalStorageService.SetItem(key, resource);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : LocalStorageService - Save() - {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            try
            {
                await ILocalStorageService.RemoveItemAsync(key);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : LocalStorageService - RemoveAsync() - {ex.Message}");
                return false;
            }
        }

        public bool Remove(string key)
        {
            try
            {
                SyncLocalStorageService.RemoveItem(key);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : LocalStorageService - Remove() - {ex.Message}");
                return false;
            }
        }
    }
}
