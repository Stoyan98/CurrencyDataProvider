using System.Threading.Tasks;

using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Data;

namespace CurrencyDataProvider.Core.Request
{
    public class AddRequestCommandHandler : ICommandHandler<AddRequestCommand>
    {
        private readonly IRequestRepository _requestRepository;

        public AddRequestCommandHandler(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task HandleAsync(AddRequestCommand command)
        {
            await _requestRepository.SaveRequestAsync(
                new Domain.Request
                {
                    ServiceName = command.ServiceName,
                    RequestId = command.RequestId,
                    RequestDateUtc = command.RequestDateUtc,
                    ClientId = command.ClientId,
                });
        }
    }
}
