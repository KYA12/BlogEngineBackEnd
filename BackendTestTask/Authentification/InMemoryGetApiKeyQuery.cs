using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendTestTask.Authorization;

namespace BackendTestTask.Authentification
{
    public class InMemoryGetApiKeyQuery : IGetAllApiKeyQuery
    {
        // Create Dictionary object to contain API key in memory
        private readonly IDictionary<string, ApiKey> _apiKeys;
        public InMemoryGetApiKeyQuery()
        {
            var existingApiKeys = new List<ApiKey>
            {
                new ApiKey(1, "Admin", "209e75ab-674b-41a3-92ea-eae383aba37d", new DateTime(2020, 01, 29),
                new List<string>
                {
                    Roles.Admin
                }),
            };

            _apiKeys = existingApiKeys.ToDictionary(x => x.Key, x => x); 
        }

        // Check if Api key exists in db
        public Task<ApiKey> CheckApiKey(string providedApiKey)
        {
            _apiKeys.TryGetValue(providedApiKey, out var key);
            return Task.FromResult(key);
        }
    }
}
