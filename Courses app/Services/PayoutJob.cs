using Quartz;

namespace Courses_app.Services
{
    public class PayoutJob : IJob
    {
        private readonly ILogger<PayoutJob> _logger;
        private readonly IPayoutService _payoutService;

        public PayoutJob(ILogger<PayoutJob> logger, IPayoutService payoutService)
        {
            _logger = logger;
            _payoutService = payoutService;

        }
        public async Task Execute(IJobExecutionContext context)
        {
            await _payoutService.ProcessPayouts();
            _logger.LogInformation($"{nameof(PayoutJob)} payout completed");
        }
    }
}
