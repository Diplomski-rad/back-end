using Courses_app.Dto;
using Courses_app.Models;
using Courses_app.Services;
using Courses_app.Services.PayPalService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courses_app.Controllers
{
    [ApiController]
    [Route("/api/cart")]
    public class CartController : ControllerBase
    {

        private readonly IPayPalService _payPalService;
        private readonly IPurchaseService _purchaseService;
        private readonly IPayoutService _payoutService;
        public CartController(IPayPalService payPalService, IPurchaseService purchaseService, IPayoutService payoutService)
        {
            _payPalService = payPalService;
            _purchaseService = purchaseService;
            _payoutService = payoutService;
        }

        [HttpPost("create-payment")]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentModel model)
        {
            try
            {
                var payment = await _payPalService.CreatePayment(model.coursesIds);
                return Ok(new { paymentId = payment.id, approvalUrl = payment.GetApprovalUrl() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        [HttpPost("checkout")]
        [Authorize(Policy = "UserOnly")]
        public async Task<IActionResult> Checkout([FromBody] PaymentExecutionModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid payment execution details");
            }

            try
            {
                var payment = await _payPalService.ExecutePayment(model.PaymentId, model.PayerId);
                var transaction = payment.transactions.FirstOrDefault();
                if (transaction != null && !string.IsNullOrEmpty(transaction.custom))
                {
                    var courseIds = transaction.custom.Split(',').Select(id => long.Parse(id)).ToList();

                    var userIdClaim = HttpContext.User.FindFirst("id");
                    if (userIdClaim == null)
                    {
                        return Unauthorized("User not found");
                    }
                    var userId = long.Parse(userIdClaim.Value);

                    var res = await _purchaseService.CreateMultiplePurchases(new CreatePurchaseModel 
                                                                                { 
                                                                                    UserId = userId,
                                                                                    CoursesIds = courseIds,
                                                                                    PaymentId = payment.id,
                                                                                    PayerId = payment.payer.payer_info.payer_id,
                                                                                    PaymentMethod = payment.payer.payment_method 
                                                                                });
                    }
                else
                {
                    return BadRequest("No course IDs found in transaction.");
                }

                return Ok("Successful payment");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message, StackTrace = ex.StackTrace });
            }

        }

        [HttpPost("payout")]
        public async Task<IActionResult> CreatePayouts()
        {
            try
            {
                await _payoutService.CreatePayouts();
                return Ok();
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("pay")]
        public async Task<IActionResult> Pay()
        {
            try
            {
                var recipients = new List<(string Email, decimal Amount)>
                {
                    ("sb-3slp232650645@personal.example.com", 20.00m),
                    ("sb-nxudp32650692@personal.example.com", 10.00m),
                };
                var response = await _payPalService.MakePayoutAsync(recipients);
                return Ok(response);
            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process()
        {
            try
            {
                await _payoutService.ProcessPayouts();
                return Ok("Successfully processed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
