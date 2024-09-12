using PayPal.Api;

namespace Courses_app.Dto
{
    public class WebhookResource
    {
        public string transaction_id { get; set; }
        public PayoutItemFee payout_item_fee { get; set; }
        public string transaction_status { get; set; }
        public string sender_batch_id { get; set; }
        public string time_processed { get; set; }
        public string activity_id { get; set; }
        public PayoutItem payout_item { get; set; }
        public string payout_item_id { get; set; }
        public string payout_batch_id { get; set; }

        public WebhookResource()
        {
            
        }
    }

    public class PayoutItem
    {
        public string recipient_type { get; set; }
        public Currency amount { get; set; }
        public string note { get; set; }
        public string receiver { get; set; }
        public string sender_item_id { get; set; }
        public string recipient_wallet { get; set; }

        public PayoutItem()
        {
            
        }
    }

    public class PayoutItemFee
    {
        public string currency { get; set; }
        public string value { get; set; }

        public PayoutItemFee()
        {
            
        }
    }


}
