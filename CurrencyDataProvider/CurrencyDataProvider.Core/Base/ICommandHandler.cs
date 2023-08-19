using System.Threading.Tasks;

namespace CurrencyDataProvider.Core.Base
{
    public interface ICommandHandler<in T>
    {
        /// <summary>
        /// Execute the command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task HandleAsync(T command);
    }
}
