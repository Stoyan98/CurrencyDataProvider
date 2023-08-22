using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Core.Currency;
using CurrencyDataProvider.Core.Request;
using CurrencyDataProvider.XmlApi.DataContracts;
using CurrencyDataProvider.XmlApi.Helpers;

namespace CurrencyDataProvider.XmlApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/xml")]
    [Consumes("application/xml")]
    public class CurrencyRateController : Controller
    {
        private readonly string DuplicateMessateString = "Duplicare requestId: {0}";
        private readonly string ServiceName = "XML_Service";
        private readonly string RequestKey = "requestKey_{0}";

        private readonly IQueryHandler<CurrentCurrencyQuery, CurrentCurrencyQueryResult> _currentCurrencyQueryHandler;
        private readonly IQueryHandler<HistoryCurrencyQuery, List<HistoryCurrencyQueryResult>> _historyCurrencyQueryHandler;
        private readonly IQueryHandler<RequestQuery, RequestQueryResult> _requestQueryHandler;
        private readonly ICommandHandler<AddRequestCommand> _addRequestCommandHandler;
        private readonly IDistributedCache _cache;

        public CurrencyRateController(
            IQueryHandler<CurrentCurrencyQuery, CurrentCurrencyQueryResult> currentCurrencyQueryHandler,
            IQueryHandler<HistoryCurrencyQuery, List<HistoryCurrencyQueryResult>> historyCurrencyQueryHandler,
            IQueryHandler<RequestQuery, RequestQueryResult> requestQueryHandler,
            ICommandHandler<AddRequestCommand> addRequestCommandHandler,
            IDistributedCache cache)
        {
            _currentCurrencyQueryHandler = currentCurrencyQueryHandler;
            _historyCurrencyQueryHandler = historyCurrencyQueryHandler;
            _requestQueryHandler = requestQueryHandler;
            _addRequestCommandHandler = addRequestCommandHandler;
            _cache = cache;
        }

        [HttpPost("command")]
        public async Task<ActionResult<CommandResponse>> Current([FromBody] CommandRequest request)
        {
            var isDuplicate = await HandleDuplicateRequestsAsync(request.Id, request.Get.Consumer);

            if (isDuplicate)
            {
                return BadRequest(string.Format(DuplicateMessateString, request.Id));
            }

            var latestRecord = await _currentCurrencyQueryHandler.HandleAsync(new CurrentCurrencyQuery(request.Get.Currency));

            return new CommandResponse
            {
                TimeStamp = latestRecord.Timestamp,
                Currency = latestRecord.Currency,
                Amount = latestRecord.Amount,
            };
        }

        [HttpPost("history")]
        public async Task<ActionResult<List<HistoryResponse>>> History([FromBody] HistoryRequest request)
        {
            var isDuplicate = await HandleDuplicateRequestsAsync(request.Id, request.History.Consumer);

            if (isDuplicate)
            {
                return BadRequest(string.Format(DuplicateMessateString, request.Id));
            }

            var history = await _historyCurrencyQueryHandler.HandleAsync(new HistoryCurrencyQuery(
                                                                            request.History.Currency,
                                                                            request.History.Period));

            var result = new List<HistoryResponse>();

            foreach (var item in history)
            {
                result.Add(new HistoryResponse()
                {
                    Currency = item.currency,
                    Amount = item.amount,
                    Date = item.date,
                });
            }

            return result;
        }

        private async Task<bool> HandleDuplicateRequestsAsync(string requestId, string clientId)
        {
            var cacheKey = string.Format(RequestKey, requestId);

            var cacheRequest = await _cache.GetRecordAsync<string>(cacheKey);

            if (cacheRequest is null)
            {
                var existingRequest = await _requestQueryHandler.HandleAsync(new RequestQuery(requestId));

                if (!existingRequest.IsExisting)
                {
                    await _addRequestCommandHandler.HandleAsync(new AddRequestCommand(ServiceName, requestId, DateTime.UtcNow, clientId));

                    await _cache.SetRecordAsync(cacheKey, requestId);

                    return false;
                }

                await _cache.SetRecordAsync(cacheKey, existingRequest.RequestId);
            }

            return true;
        }
    }
}
