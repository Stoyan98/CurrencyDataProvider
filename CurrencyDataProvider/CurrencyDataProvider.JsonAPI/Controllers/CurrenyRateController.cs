using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Core.Currency;
using CurrencyDataProvider.JsonAPI.DataContracts;

namespace CurrencyDataProvider.JsonAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrenyRateController : Controller
    {
        public CurrenyRateController(IQueryHandler<CurrentCurrencyQuery, CurrentCurrencyQueryResult> currentCurrencyQueryHandler)
        {
            CurrentCurrencyQueryHandler = currentCurrencyQueryHandler;
        }

        public IQueryHandler<CurrentCurrencyQuery, CurrentCurrencyQueryResult> CurrentCurrencyQueryHandler { get; set; }

        [HttpPost("current")]
        public async Task<CurrentCurrencyQueryResult> Current([FromBody]CurrentRequest request)
        {
            return await CurrentCurrencyQueryHandler.HandleAsync(new CurrentCurrencyQuery(request.Currency));
        }

        [HttpPost("history")]
        public IActionResult History(HistoryRequest request)
        {
            return View();
        }
    }
}
