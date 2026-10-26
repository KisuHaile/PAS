using Domain.Requisition;

namespace Persistence.Repositories.Interfaces
{
    public interface IServiceRequestRepository : IGenericRepository<ServiceRequest>
    {
        Task<ServiceRequest?> GetRequestWithDetailsAsync(Guid id);
        Task<IEnumerable<ServiceRequest>> GetRequestsByStatusAsync(string status);
        Task<IEnumerable<ServiceRequest>> GetRequestsByRequesterAsync(Guid requesterId);
        Task<string> GenerateSRNumberAsync();
        Task<bool> IsSRNumberUniqueAsync(string srNumber);
        Task<IEnumerable<ServiceRequest>> GetPendingApprovalRequestsAsync();
    }
}