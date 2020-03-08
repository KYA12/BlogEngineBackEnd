using System.Threading.Tasks;

namespace BackendTestTask.Authentification
{
    public interface IGetAllApiKeyQuery
    {
        Task<ApiKey> CheckApiKey(string providedApiKey);
    }
}
