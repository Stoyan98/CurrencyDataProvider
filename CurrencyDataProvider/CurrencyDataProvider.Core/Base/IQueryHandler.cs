using System.Threading.Tasks;

namespace CurrencyDataProvider.Core.Base
{
    public interface IQueryHandler<in T, TResult>
    {
        /// <summary>
        /// Execute the query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        ValueTask<TResult> HandleAsync(T query);
    }
}
