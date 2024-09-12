using Courses_app.Models;
using Microsoft.EntityFrameworkCore;
using Courses_app.Exceptions;

namespace Courses_app.Repository
{
    public class PayoutRepository : IPayoutRepository
    {
        private readonly CoursesAppDbContext _context;

        public PayoutRepository(CoursesAppDbContext context)
        {
            _context = context;
        }

        public async Task CreatePayouts()
        {
            try
            {
                var authorsWithUnpaidEarnings = await _context.AuthorEarning
                    .Where(ae => !ae.IsIncludedInPayout)
                    .Select(ae => ae.AuthorId)
                    .Distinct()
                    .ToListAsync();

                foreach(var authorId in authorsWithUnpaidEarnings)
                {
                    var authorEarnings = await _context.AuthorEarning
                        .Where(ae => ae.AuthorId == authorId && !ae.IsIncludedInPayout)
                        .Include(ae => ae.Author)
                        .ToListAsync();

                    var payout = new Payout(authorEarnings);

                    foreach(var earning in authorEarnings)
                    {
                        earning.IsIncludedInPayout = true;
                    }

                    await _context.AddAsync(payout);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task CompletePayout(string controlGuid, string item_id, string batch_id)
        {
            try
            {
                var payout = await _context.Payout.FirstOrDefaultAsync(p => p.ControlGuid == controlGuid);
                if(payout == null)
                {
                    throw new NotFoundException("Payout with given control Guid don't exist.");
                }

                payout.Status = PayoutStatus.Completed;
                payout.PayoutDate = DateTime.UtcNow;
                payout.Payout_item_id = item_id;
                payout.Payout_batch_id = batch_id;

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task PayoutFailed(string controlGuid, string item_id, string batch_id)
        {
            try
            {
                var payout = await _context.Payout.FirstOrDefaultAsync(p => p.ControlGuid == controlGuid);
                if (payout == null)
                {
                    throw new NotFoundException("Payout with given control Guid don't exist.");
                }

                payout.Status = PayoutStatus.Failed;
                payout.Payout_item_id = item_id;
                payout.Payout_batch_id = batch_id;

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Payout>> GetAllPending()
        {
            try
            {
                var payouts = await _context.Payout.Include(p => p.Author).Where(p => p.Status == PayoutStatus.Pending).ToListAsync();
                return payouts;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
