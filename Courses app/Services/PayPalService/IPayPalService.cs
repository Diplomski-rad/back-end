using PayPal.Api;

namespace Courses_app.Services.PayPalService
{
    public interface IPayPalService
    {
        public Task<Payment> CreatePayment(long courseId);
        public Task<Payment> ExecutePayment(string paymentId, string payerId);

    }
}
