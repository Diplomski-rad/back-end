using Courses_app.Models;
using PayPal;
using PayPal.Api;
using System.Diagnostics;

namespace Courses_app.Services.PayPalService
{
    public class PayPalService : IPayPalService
    {
        private readonly IConfiguration _configuration;
        private readonly ICourseService _courseService;

        public PayPalService(IConfiguration configuration, ICourseService courseService)
        {
            _configuration = configuration;
            _courseService = courseService;
        }

        private static APIContext GetApiContext(string accessToken)
        {
            return new APIContext(accessToken);
        }

        private string GetAccessToken()
        {
            var clientId = _configuration["PayPal:ClientId"];
            var clientSecret = _configuration["PayPal:ClientSecret"];
            var accessToken = new OAuthTokenCredential(clientId, clientSecret).GetAccessToken();
            return accessToken;
        }

        public async Task<Payment> CreatePayment(List<long> courseIds)
        {
            var accessToken = GetAccessToken();
            var apiContext = GetApiContext(accessToken);

            List<Course> courses = await _courseService.GetCoursesByIds(courseIds);



            decimal totalAmount = 0;
            var items = new List<Item>();

            foreach (Course course in courses)
            {
                totalAmount += (decimal)course.Price;

                items.Add(new Item
                {
                    name = course.Name,
                    currency = "USD",
                    price = course.Price.ToString("F2"),
                    quantity = "1"
                });
            }

            var payer = new Payer { payment_method = "paypal" };
            var redirectUrls = new RedirectUrls
            {
                cancel_url = "http://localhost:3000/cancel",
                return_url = "http://localhost:3000/payment-success"
            };
            var details = new Details
            {
                subtotal = totalAmount.ToString("F2"),
            };
            var amountObj = new Amount
            {
                currency = "USD",
                total = totalAmount.ToString("F2"),
                details = details
            };

            var itemList = new ItemList
            {
                items = items
            };

            var transaction = new Transaction
            {
                description = "Purchase of online course",
                amount = amountObj,
                item_list = itemList,
                custom = string.Join(",", courseIds)
            };
            var payment = new Payment
            {
                intent = "sale",
                payer = payer,
                transactions = new List<Transaction> { transaction },
                redirect_urls = redirectUrls
            };

            return payment.Create(apiContext);
        }

        public async Task<Payment> ExecutePayment(string paymentId, string payerId)
        {
            var accessToken = GetAccessToken();
            var apiContext = GetApiContext(accessToken);
            var payment = new Payment { id = paymentId };
            var paymentExecution = new PaymentExecution { payer_id = payerId };
            Payment payment1 = payment.Execute(apiContext, paymentExecution);
            return payment1;
        }
    }
}
