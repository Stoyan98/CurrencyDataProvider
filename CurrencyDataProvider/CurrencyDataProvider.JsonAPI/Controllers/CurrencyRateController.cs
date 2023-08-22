using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Core.Currency;
using CurrencyDataProvider.JsonAPI.DataContracts;
using System.Collections.Generic;
using CurrencyDataProvider.Core.Request;
using System;
using Microsoft.Extensions.Caching.Distributed;
using CurrencyDataProvider.JsonAPI.Helpers;

namespace CurrencyDataProvider.JsonAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyRateController : Controller
    {
        private readonly string DuplicateMessateString = "Duplicare requestId: {0}";
        private readonly string ServiceName = "JSON_Service";
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

        [HttpPost("current")]
        public async Task<ActionResult<CurrentCurrencyQueryResult>> Current([FromBody] CurrentRequest request)
        {
            var isDuplicate = await HandleDuplicateRequestsAsync(request.RequestId, request.Client);

            if (isDuplicate)
            {
                return BadRequest(string.Format(DuplicateMessateString, request.RequestId));
            }

            return await CurrentCurrencyQueryHandler.HandleAsync(new CurrentCurrencyQuery(request.Currency));
        }

        [HttpPost("history")]
        public async Task<ActionResult<List<HistoryCurrencyQueryResult>>> History([FromBody] HistoryRequest request)
        {
            var isDuplicate = await HandleDuplicateRequestsAsync(request.RequestId, request.Client);

            if (isDuplicate)
            {
                return BadRequest(string.Format(DuplicateMessateString, request.RequestId));
            }

            return await HistoryCurrencyQueryHandler.HandleAsync(new HistoryCurrencyQuery(request.Currency, request.Period));
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
