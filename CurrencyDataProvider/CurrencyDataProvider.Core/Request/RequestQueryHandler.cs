using System.Threading.Tasks;

using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Data;

namespace CurrencyDataProvider.Core.Request
{
    public class RequestQueryHandler : IQueryHandler<RequestQuery, RequestQueryResult>
    {
        private readonly IRequestRepository _requestRepository;

        public RequestQueryHandler(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async ValueTask<RequestQueryResult> HandleAsync(RequestQuery query)
        {
            var request = await _requestRepository.GetRequestByRequestIdAsync(query.RequestId);

            var isExisting = true;

            if (request is null) 
            { 
                isExisting = false;
            }

            return new RequestQueryResult(
                isExisting, 
                request?.ServiceName, 
                request?.RequestId, 
                request?.RequestDateUtc, 
                request?.ClientId);
        }
    }
}
