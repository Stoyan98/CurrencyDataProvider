using System.Threading.Tasks;
using CurrencyDataProvider.Domain;

namespace CurrencyDataProvider.Data
{
    public interface IRequestRepository
    {
        /// <summary>
        /// Gets request by requestId
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        Task<Request> GetRequestByRequestIdAsync(string requestId);

        /// <summary>
        /// Saves unified requests
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task SaveRequestAsync(Request request);
    }
}
