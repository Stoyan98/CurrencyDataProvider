using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Core.Currency;
using CurrencyDataProvider.XmlApi.DataContracts;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyDataProvider.XmlApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/xml")]
    [Consumes("application/xml")]
    public class CurrencyRateController : Controller
    {
        public CurrencyRateController(
            IQueryHandler<CurrentCurrencyQuery, CurrentCurrencyQueryResult> currentCurrencyQueryHandler,
            IQueryHandler<HistoryCurrencyQuery, List<HistoryCurrencyQueryResult>> historyCurrencyQueryHandler)
        {
            CurrentCurrencyQueryHandler = currentCurrencyQueryHandler;
            HistoryCurrencyQueryHandler = historyCurrencyQueryHandler;
        }

        public IQueryHandler<CurrentCurrencyQuery, CurrentCurrencyQueryResult> CurrentCurrencyQueryHandler { get; set; }

        public IQueryHandler<HistoryCurrencyQuery, List<HistoryCurrencyQueryResult>> HistoryCurrencyQueryHandler { get; set; }

        [HttpPost("command")]
        public async Task<CommandResponse> Current([FromBody] CommandRequest request)
        {
            var latestRecord = await CurrentCurrencyQueryHandler.HandleAsync(new CurrentCurrencyQuery(request.Get.FirstOrDefault().Currency));

            return new CommandResponse
            {
                TimeStamp = latestRecord.Timestamp,
                Currency = latestRecord.Currency,
                Amount = latestRecord.Amount,
            };
        }

        [HttpPost("history")]
        public async Task<List<HistoryResponse>> History([FromBody] HistoryRequest? request)
        {
            var history = await HistoryCurrencyQueryHandler.HandleAsync(
                                                                new HistoryCurrencyQuery(
                                                                    request?.History?.Currency,
                                                                    request?.History?.Period));

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
    }
}
