using System.Threading.Tasks;
using CurrencyDataProvider.Domain;

namespace CurrencyDataProvider.Data
{
    public interface IRequestRepository
    {
        /// <summary>
        /// Get request by requestId
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        Task<Request> GetRequestByRequestIdAsync(string requestId);

        /// <summary>
        /// Save unified requests
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task SaveRequestAsync(Request request);
    }
}
