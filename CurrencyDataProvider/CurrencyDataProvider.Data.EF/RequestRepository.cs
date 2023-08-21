using System.Threading.Tasks;
using CurrencyDataProvider.Domain;
using Microsoft.EntityFrameworkCore;

namespace CurrencyDataProvider.Data.EF
{
    public class RequestRepository : IRequestRepository
    {
        public RequestRepository(CurrencyDataProviderDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public CurrencyDataProviderDbContext DbContext { get; set; }

        public async Task<Request> GetRequestByRequestIdAsync(string requestId)
        {
            return await DbContext.Requests.FirstOrDefaultAsync(x => x.RequestId == requestId);
        }

        public async Task SaveRequestAsync(Request request)
        {
            await DbContext.AddAsync(request);
            await DbContext.SaveChangesAsync();
        }
    }
}
