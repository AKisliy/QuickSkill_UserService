using AutoMapper;
using MassTransit;
using UserService.Core.Interfaces;
using UserService.Core.Models;
using Shared;

namespace UserService.Infrastructure.Consumers
{
    public class BotsCreatedConsumer: IConsumer<BotsCreatedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public BotsCreatedConsumer(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task Consume(ConsumeContext<BotsCreatedEvent> context)
        {
            System.Console.WriteLine("Wow i got it");
            var createdBots = context.Message;
            await _userRepository.AddBots(createdBots.Bots.Select(b => _mapper.Map<User>(b)).ToList());
            await Task.CompletedTask;
        }
    }
}