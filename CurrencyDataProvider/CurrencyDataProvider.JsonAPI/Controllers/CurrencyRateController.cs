using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Core.Currency;
using CurrencyDataProvider.JsonAPI.DataContracts;
using CurrencyDataProvider.Core.Request;
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

        [HttpPost("current")]
        public async Task<ActionResult<CurrentCurrencyQueryResult>> Current([FromBody] CurrentRequest request)
        {
            var isDuplicate = await HandleDuplicateRequestsAsync(request.RequestId, request.Client);

            if (isDuplicate)
            {
                return BadRequest(string.Format(DuplicateMessateString, request.RequestId));
            }

            return await _currentCurrencyQueryHandler.HandleAsync(new CurrentCurrencyQuery(request.Currency));
        }

        [HttpPost("history")]
        public async Task<ActionResult<List<HistoryCurrencyQueryResult>>> History([FromBody] HistoryRequest request)
        {
            var isDuplicate = await HandleDuplicateRequestsAsync(request.RequestId, request.Client);

            if (isDuplicate)
            {
                return BadRequest(string.Format(DuplicateMessateString, request.RequestId));
            }

            return await _historyCurrencyQueryHandler.HandleAsync(new HistoryCurrencyQuery(request.Currency, request.Period));
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
