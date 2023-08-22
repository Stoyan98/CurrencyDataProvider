using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Core.Currency;
using CurrencyDataProvider.JsonAPI.DataContracts;
using System.Collections.Generic;
using CurrencyDataProvider.Core.Request;
using System;

namespace CurrencyDataProvider.JsonAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyRateController : Controller
    {
        private readonly string DuplicateMessateString = "Duplicare requestId: {0}";
        private readonly string ServiceName = "JSON_Service";

        public CurrencyRateController(
            IQueryHandler<CurrentCurrencyQuery, CurrentCurrencyQueryResult> currentCurrencyQueryHandler,
            IQueryHandler<HistoryCurrencyQuery, List<HistoryCurrencyQueryResult>> historyCurrencyQueryHandler,
            IQueryHandler<RequestQuery, RequestQueryResult> requestQueryHandler,
            ICommandHandler<AddRequestCommand> addRequestCommandHandler)
        {
            CurrentCurrencyQueryHandler = currentCurrencyQueryHandler;
            HistoryCurrencyQueryHandler = historyCurrencyQueryHandler;
            RequestQueryHandler = requestQueryHandler;
            AddRequestCommandHandler = addRequestCommandHandler;
        }

        public IQueryHandler<CurrentCurrencyQuery, CurrentCurrencyQueryResult> CurrentCurrencyQueryHandler { get; set; }

        public IQueryHandler<HistoryCurrencyQuery, List<HistoryCurrencyQueryResult>> HistoryCurrencyQueryHandler { get; set; }

        public IQueryHandler<RequestQuery, RequestQueryResult> RequestQueryHandler { get; set; }

        public ICommandHandler<AddRequestCommand> AddRequestCommandHandler { get; set; }

        [HttpPost("current")]
        public async Task<ActionResult<CurrentCurrencyQueryResult>> Current([FromBody] CurrentRequest request)
        {
            var isDuplicate = await HandleDuplicateRequestsAsync(request);

            if (isDuplicate)
            {
                return BadRequest(string.Format(DuplicateMessateString, request.RequestId));
            }

            return await CurrentCurrencyQueryHandler.HandleAsync(new CurrentCurrencyQuery(request.Currency));
        }

        [HttpPost("history")]
        public async Task<ActionResult<List<HistoryCurrencyQueryResult>>> History([FromBody] HistoryRequest request)
        {
            var isDuplicate = await HandleDuplicateRequestsAsync(request);

            if (isDuplicate)
            {
                return BadRequest(string.Format(DuplicateMessateString, request.RequestId));
            }

            return await HistoryCurrencyQueryHandler.HandleAsync(new HistoryCurrencyQuery(request.Currency, request.Period));
        }

        private async Task<bool> HandleDuplicateRequestsAsync(IRequest request)
        {
            var existingRequest = await RequestQueryHandler.HandleAsync(new RequestQuery(request.RequestId));

            if (!existingRequest.IsExisting)
            {
                await AddRequestCommandHandler.HandleAsync(new AddRequestCommand(ServiceName, request.RequestId, DateTime.UtcNow, request.Client));
                return false;
            }

            return true;
        }
    }
}
