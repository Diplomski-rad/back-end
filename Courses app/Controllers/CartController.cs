using Courses_app.Dto;
using Courses_app.Models;
using Courses_app.Services;
using Courses_app.Services.PayPalService;
using Microsoft.AspNetCore.Mvc;

namespace Courses_app.Controllers
{
    [ApiController]
    [Route("/api/cart")]
    public class CartController : ControllerBase
    {

        private readonly IPayPalService _payPalService;
        private readonly IPurchaseService _purchaseService;
        public CartController(IPayPalService payPalService, IPurchaseService purchaseService)
        {
            _payPalService = payPalService;
            _purchaseService = purchaseService;
        }

        [HttpPost("create-payment")]
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

                    var res = await _purchaseService.CreateMultiplePurchases(new CreatePurchaseModel { UserId = userId, CoursesIds = courseIds });
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
    }
}
