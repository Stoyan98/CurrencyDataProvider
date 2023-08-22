using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Core.Currency;
using CurrencyDataProvider.Core.Request;
using CurrencyDataProvider.XmlApi.DataContracts;
using CurrencyDataProvider.XmlApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

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

        private readonly IDistributedCache _cache;

        public CurrencyRateController(
            IQueryHandler<CurrentCurrencyQuery, CurrentCurrencyQueryResult> currentCurrencyQueryHandler,
            IQueryHandler<HistoryCurrencyQuery, List<HistoryCurrencyQueryResult>> historyCurrencyQueryHandler,
            IQueryHandler<RequestQuery, RequestQueryResult> requestQueryHandler,
            ICommandHandler<AddRequestCommand> addRequestCommandHandler,
            IDistributedCache cache)
        {
            CurrentCurrencyQueryHandler = currentCurrencyQueryHandler;
            HistoryCurrencyQueryHandler = historyCurrencyQueryHandler;
            RequestQueryHandler = requestQueryHandler;
            AddRequestCommandHandler = addRequestCommandHandler;
            _cache = cache;
        }

        public IQueryHandler<CurrentCurrencyQuery, CurrentCurrencyQueryResult> CurrentCurrencyQueryHandler { get; set; }

        public IQueryHandler<HistoryCurrencyQuery, List<HistoryCurrencyQueryResult>> HistoryCurrencyQueryHandler { get; set; }

        public IQueryHandler<RequestQuery, RequestQueryResult> RequestQueryHandler { get; set; }

        public ICommandHandler<AddRequestCommand> AddRequestCommandHandler { get; set; }

        [HttpPost("command")]
        public async Task<ActionResult<CommandResponse>> Current([FromBody] CommandRequest request)
        {
            var isDuplicate = await HandleDuplicateRequestsAsync(request.Id, request.Get.Consumer);

            if (isDuplicate)
            {
                return BadRequest(string.Format(DuplicateMessateString, request.Id));
            }

            var latestRecord = await CurrentCurrencyQueryHandler.HandleAsync(new CurrentCurrencyQuery(request.Get.Currency));

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

            var history = await HistoryCurrencyQueryHandler.HandleAsync(new HistoryCurrencyQuery(
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
                var existingRequest = await RequestQueryHandler.HandleAsync(new RequestQuery(requestId));

                if (!existingRequest.IsExisting)
                {
                    await AddRequestCommandHandler.HandleAsync(new AddRequestCommand(ServiceName, requestId, DateTime.UtcNow, clientId));

                    await _cache.SetRecordAsync(cacheKey, requestId);

                    return false;
                }

                await _cache.SetRecordAsync(cacheKey, existingRequest.RequestId);
            }

            return true;
        }
    }
}
