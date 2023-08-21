using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Data;
using System.Threading.Tasks;

namespace CurrencyDataProvider.Core.Request
{
    public class RequestQueryHandler : IQueryHandler<RequestQuery, RequestQueryResult>
    {
        public RequestQueryHandler(IRequestRepository requestRepository)
        {
            RequestRepository = requestRepository;
        }

        public IRequestRepository RequestRepository { get; set; }

        public async ValueTask<RequestQueryResult> HandleAsync(RequestQuery query)
        {
            var request = await RequestRepository.GetRequestByRequestIdAsync(query.RequestId);

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
