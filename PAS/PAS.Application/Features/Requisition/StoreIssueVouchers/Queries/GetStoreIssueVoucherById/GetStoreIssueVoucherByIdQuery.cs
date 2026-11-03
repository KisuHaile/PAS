using Application.Common.Security;
using Application.Features.Requisition.StoreIssueVouchers.Dtos;
using MediatR;

namespace Application.Features.Requisition.StoreIssueVouchers.Queries;

[Authorize(Permissions = Permissions.Requisitions.View)]
public record GetStoreIssueVoucherByIdQuery(Guid Id) : IRequest<Result<StoreIssueVoucherDetailDto>>;