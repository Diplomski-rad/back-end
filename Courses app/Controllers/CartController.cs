using Courses_app.Dto;
using Courses_app.Models;
using Courses_app.Services;
using Courses_app.Services.PayPalService;
using Courses_app.WebSocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Courses_app.Controllers
{
    [ApiController]
    [Route("/api/cart")]
    public class CartController : ControllerBase
    {

        private readonly IPayPalService _payPalService;
        private readonly IPurchaseService _purchaseService;
        private readonly IPayoutService _payoutService;
        private readonly IHubContext<CourseHub> _hubContext;
        public CartController(IPayPalService payPalService, IPurchaseService purchaseService, IPayoutService payoutService, IHubContext<CourseHub> hubContext)
        {
            _payPalService = payPalService;
            _purchaseService = purchaseService;
            _payoutService = payoutService;
            _hubContext = hubContext;
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


        [HttpGet("websocket")]
        public async Task<IActionResult> Test()
        {
            await _hubContext.Clients.All.SendAsync("VideoPublished");
            return Ok();
        }
        

    }
}
