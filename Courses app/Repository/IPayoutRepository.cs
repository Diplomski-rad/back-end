using Courses_app.Models;

namespace Courses_app.Repository
{
    public interface IPayoutRepository
    {
        public Task CreatePayouts();
        public Task CompletePayout(string controlGuid, string item_id, string batch_id);
        public Task PayoutFailed(string controlGuid, string item_id, string batch_id);
        public Task<List<Payout>> GetAllPending();
    }
}
