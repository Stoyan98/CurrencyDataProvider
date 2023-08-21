using System.Threading.Tasks;
using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Data;

namespace CurrencyDataProvider.Core.Request
{
    internal class AddRequestCommandHandler : ICommandHandler<AddRequestCommand>
    {
        public AddRequestCommandHandler(IRequestRepository requestRepository)
        {
            RequestRepository = requestRepository;
        }

        public IRequestRepository RequestRepository { get; set; }

        public async Task HandleAsync(AddRequestCommand command)
        {
            await RequestRepository.SaveRequestAsync(
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
