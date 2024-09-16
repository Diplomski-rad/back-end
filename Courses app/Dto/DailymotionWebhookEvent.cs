namespace Courses_app.Dto
{
    public class DailymotionWebhookEvent
    {

            public string Type { get; set; }
            public long Timestamp { get; set; }
            public VideoData Data { get; set; }

        public DailymotionWebhookEvent()
        {
            
        }
    }
    public class VideoData
    {
        public string Owner_id { get; set; }
        public string Video_id { get; set; }
        public string? PresetName { get; set; }
        public string? Status { get; set; }
        public int? Progress { get; set; }

        public VideoData()
        {
            
        }
    }
}
