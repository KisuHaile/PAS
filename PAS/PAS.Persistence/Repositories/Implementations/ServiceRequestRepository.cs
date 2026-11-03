using Microsoft.EntityFrameworkCore;
using Domain.Requisition;
using Persistence.Context;
using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.Implementations
{
    public class ServiceRequestRepository : GenericRepository<ServiceRequest>, IServiceRequestRepository
    {
        public ServiceRequestRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ServiceRequest?> GetRequestWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(s => s.Details)
                    .ThenInclude(d => d.Item)
                .Include(s => s.Details)
                    .ThenInclude(d => d.ShelfLocation)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<ServiceRequest>> GetRequestsByStatusAsync(string status)
        {
            return await _dbSet
                .Where(s => s.Status == status)
                .Include(s => s.Details)
                .OrderByDescending(s => s.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceRequest>> GetRequestsByRequesterAsync(Guid requesterId)
        {
            return await _dbSet
                .Where(s => s.RequesterId == requesterId)
                .Include(s => s.Details)
                .OrderByDescending(s => s.RequestDate)
                .ToListAsync();
        }

        public async Task<string> GenerateSRNumberAsync()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month.ToString("00");

            var lastRequest = await _dbSet
                .Where(s => s.SRNumber.StartsWith($"SR-{year}{month}"))
                .OrderByDescending(s => s.SRNumber)
                .FirstOrDefaultAsync();

            if (lastRequest == null)
            {
                return $"SR-{year}{month}-0001";
            }

            var lastNumber = int.Parse(lastRequest.SRNumber.Split('-').Last());
            return $"SR-{year}{month}-{(lastNumber + 1):D4}";
        }

        public async Task<bool> IsSRNumberUniqueAsync(string srNumber)
        {
            return !await _dbSet.AnyAsync(s => s.SRNumber == srNumber);
        }

        public async Task<IEnumerable<ServiceRequest>> GetPendingApprovalRequestsAsync()
        {
            return await _dbSet
                .Where(s => s.Status == "Pending" || s.Status == "Pending Approval")
                .Include(s => s.Details)
                .OrderBy(s => s.RequestDate)
                .ToListAsync();
        }
    }
}