using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using CurrencyDataProvider.Domain;

namespace CurrencyDataProvider.Data.EF
{
    public class RequestRepository : IRequestRepository
    {
        private readonly CurrencyDataProviderDbContext _dbContext;

        public RequestRepository(CurrencyDataProviderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Request> GetRequestByRequestIdAsync(string requestId)
        {
            return await _dbContext.Requests.FirstOrDefaultAsync(x => x.RequestId == requestId);
        }

        public async Task SaveRequestAsync(Request request)
        {
            await _dbContext.AddAsync(request);
            await _dbContext.SaveChangesAsync();
        }
    }
}
