using PayPal.Api;

namespace Courses_app.Services.PayPalService
{
    public interface IPayPalService
    {
        public Task<Payment> CreatePayment(List<long> courseIds);
        public Task<Payment> ExecutePayment(string paymentId, string payerId);
        public Task<PayoutBatch> MakePayoutAsync(List<(string Email, decimal Amount)> recipients);
        public Task<PayoutBatch> MakePayoutAsync(List<Models.Payout> recipients);

    }
}
