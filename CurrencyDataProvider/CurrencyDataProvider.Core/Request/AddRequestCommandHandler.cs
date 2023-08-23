using System.Threading.Tasks;

using CurrencyDataProvider.Core.Base;
using CurrencyDataProvider.Core.RabbitMQ;
using CurrencyDataProvider.Data;

namespace CurrencyDataProvider.Core.Request
{
    public class AddRequestCommandHandler : ICommandHandler<AddRequestCommand>
    {
        private readonly IRequestRepository _requestRepository;
        private readonly RabbitSender _rabbitSender;

        public AddRequestCommandHandler(IRequestRepository requestRepository, RabbitSender rabbitSender)
        {
            _requestRepository = requestRepository;
            _rabbitSender = rabbitSender;

        }

        public async Task HandleAsync(AddRequestCommand command)
        {
            _rabbitSender.PublishMessage<AddRequestCommand>(command, "add.request");

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
