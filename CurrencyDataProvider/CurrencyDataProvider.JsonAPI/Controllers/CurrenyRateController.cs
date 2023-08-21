using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Core.Currency;
using CurrencyDataProvider.JsonAPI.DataContracts;
using System.Collections.Generic;

namespace CurrencyDataProvider.JsonAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrenyRateController : Controller
    {
        public CurrenyRateController(
            IQueryHandler<CurrentCurrencyQuery, CurrentCurrencyQueryResult> currentCurrencyQueryHandler,
            IQueryHandler<HistoryCurrencyQuery, List<HistoryCurrencyQueryResult>> historyCurrencyQueryHandler)
        {
            CurrentCurrencyQueryHandler = currentCurrencyQueryHandler;
            HistoryCurrencyQueryHandler = historyCurrencyQueryHandler;
        }

        public IQueryHandler<CurrentCurrencyQuery, CurrentCurrencyQueryResult> CurrentCurrencyQueryHandler { get; set; }

        public IQueryHandler<HistoryCurrencyQuery, List<HistoryCurrencyQueryResult>> HistoryCurrencyQueryHandler { get; set; }

        [HttpPost("current")]
        public async Task<CurrentCurrencyQueryResult> Current([FromBody]CurrentRequest request)
        {
            return await CurrentCurrencyQueryHandler.HandleAsync(new CurrentCurrencyQuery(request.Currency));
        }

        [HttpPost("history")]
        public async Task<List<HistoryCurrencyQueryResult>> History([FromBody]HistoryRequest request)
        {
            return await HistoryCurrencyQueryHandler.HandleAsync(new HistoryCurrencyQuery(request.Currency, request.Period));
        }
    }
}
