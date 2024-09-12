namespace Courses_app.Services
{
    public interface IPayoutService
    {
        public Task CreatePayouts();
        public Task ProcessPayouts();
        public Task PayoutFailed(string controlGuid, string item_id, string batch_id);
        public Task CompletePayout(string controlGuid, string item_id, string batch_id);
    }
}
