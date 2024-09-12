using Courses_app.Dto;
using Courses_app.Services;
using Courses_app.Services.PayPalService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayPal.Api;
using System.Threading.Tasks;

namespace Courses_app.Controllers.PayPalController
{
    [Route("api/paypal")]
    public class PayPalController : Controller
    {

        private readonly IPayoutService _payoutService;


        public PayPalController(IPayoutService payoutService)
        {
            _payoutService = payoutService;

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

        [HttpPost]
        [Route("webhook/payout")]
        public async Task<IActionResult> PayPalWebhook([FromBody] WebhookEvent webhookEvent)
        {
            try
            {
                var resourceJson = webhookEvent.resource.ToString();
                var resource = JsonConvert.DeserializeObject<WebhookResource>(resourceJson);

                if (resource == null || resource.payout_item == null)
                {
                    return BadRequest("Invalid resource data");
                }

                var senderItemId = resource.payout_item.sender_item_id;
                var payout_item_id = resource.payout_item_id;
                var payout_batch_id = resource.payout_batch_id;

                

                switch (webhookEvent.event_type)
                {
                    case "PAYMENT.PAYOUTS-ITEM.SUCCEEDED":
                        await _payoutService.CompletePayout(senderItemId, payout_item_id, payout_batch_id);
                        break;

                    case "PAYMENT.PAYOUTS-ITEM.UNCLAIMED":
                        await _payoutService.PayoutFailed(senderItemId, payout_item_id, payout_batch_id);
                        break;

                    case "PAYMENT.PAYOUTS-ITEM.FAILED":
                        await _payoutService.PayoutFailed(senderItemId, payout_item_id, payout_batch_id);
                        break;

                    case "PAYMENT.PAYOUTS-ITEM.DENIED":
                        await _payoutService.PayoutFailed(senderItemId, payout_item_id, payout_batch_id);
                        break;


                    default:
                        return BadRequest("Unsupported event type");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
            

            
        }

        [HttpPost]
        [Route("webhook/test")]
        public async Task<IActionResult> Test()
        {


            return Ok("Hello world");
        }
    }

}
