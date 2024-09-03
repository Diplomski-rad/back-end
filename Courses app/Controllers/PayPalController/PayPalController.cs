using Courses_app.Dto;
using Courses_app.Services.PayPalService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Courses_app.Controllers.PayPalController
{
    [Route("api/paypal")]
    public class PayPalController : Controller
    {

        private readonly PayPalService _payPalService;

        public PayPalController(PayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        //[HttpPost("create-payment")]
        //public IActionResult CreatePayment()
        //{
        //    try
        //    {
        //        var payment = _payPalService.CreatePayment(1);
        //        return Ok(new { paymentId = payment.id, approvalUrl = payment.GetApprovalUrl() });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Message = ex.Message, StackTrace = ex.StackTrace });
        //    }
        //}

        //[HttpPost("execute-payment")]
        //public IActionResult ExecutePayment([FromBody] PaymentExecutionModel model)
        //{
        //    if (string.IsNullOrEmpty(model.PaymentId) || string.IsNullOrEmpty(model.PayerId))
        //    {
        //        return BadRequest("PaymentId and PayerId cannot be null or empty.");
        //    }

        //    try
        //    {
        //        var payment = _payPalService.ExecutePayment(model.PaymentId, model.PayerId);
        //        return Ok(payment);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Message = ex.Message, StackTrace = ex.StackTrace });
        //    }
        //}
    }

}
