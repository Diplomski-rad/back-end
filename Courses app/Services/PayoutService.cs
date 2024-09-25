using Courses_app.Repository;
using Courses_app.Services.PayPalService;

namespace Courses_app.Services
{
    public class PayoutService : IPayoutService
    {
        public readonly IPayoutRepository _payoutRepository;
        public readonly IPayPalService _payPalService;

        public PayoutService(IPayoutRepository payoutRepository, IPayPalService payPalService)
        {
            _payoutRepository = payoutRepository;
            _payPalService = payPalService;
        }

        //public async Task CreatePayouts()
        //{
        //    await _payoutRepository.CreatePayouts();
        //}

        public async Task ProcessPayouts()
        {
            try
            {
                await _payoutRepository.CreatePayouts();
                var pendingPayouts = await _payoutRepository.GetAllPending();
                await _payPalService.MakePayoutAsync(pendingPayouts);

            }catch (Exception ex)
            {
                throw;
            }
        }

        public async Task CompletePayout(string controlGuid, string item_id, string batch_id)
        {
            try
            {
                await _payoutRepository.CompletePayout(controlGuid, item_id, batch_id);
            }catch (Exception ex)
            {
                throw;
            }
        }

        public async Task PayoutFailed(string controlGuid, string item_id, string batch_id)
        {
            try
            {
                await _payoutRepository.PayoutFailed(controlGuid, item_id, batch_id);
            }catch (Exception ex)
            {
                throw;
            }
        }
    }
}
