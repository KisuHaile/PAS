using Application.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Context;
using Persistence.Identity;
using System.Data;

namespace Application.Features.Workflow.Approvers.Commands.AssignApprover;

public class AssignApproverCommandHandler : IRequestHandler<AssignApproverCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public AssignApproverCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext db)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
        _userManager = userManager;
        _db = db;
    }

    public async Task<Result<Guid>> Handle(AssignApproverCommand request, CancellationToken cancellationToken)
    {
        var workflow = await _context.ApprovalWorkflows
            .Include(w => w.Approvers)
            .FirstOrDefaultAsync(w => w.Id == request.WorkflowId && !w.IsDeleted, cancellationToken);

        if (workflow == null)
        {
            return Result<Guid>.Failure("Workflow not found.");
        }

        var identityUserId = request.UserId.ToString();
        var identityUser = await _userManager.FindByIdAsync(identityUserId);
        if (identityUser == null || !identityUser.IsActive)
        {
            return Result<Guid>.Failure("User not found or inactive.");
        }

        var approverUserId = request.UserId;

        var existingApprover = workflow.Approvers?
            .FirstOrDefault(a => a.UserId == approverUserId && !a.IsDeleted);

        if (existingApprover != null)
        {
            return Result<Guid>.Failure("User is already assigned to this workflow.");
        }

        var existingLevel = workflow.Approvers?
            .Any(a => a.ApprovalLevel == request.ApprovalLevel && !a.IsDeleted) == true;

        if (existingLevel)
        {
            return Result<Guid>.Failure($"Approval level {request.ApprovalLevel} is already assigned.");
        }

        var approver = new Approver(request.WorkflowId, approverUserId, request.ApprovalLevel);

        _context.Approvers.Add(approver);
        await _context.SaveChangesAsync(cancellationToken);

        if (_currentUser.UserGuid.HasValue)
        {
            var auditTrail = new AuditTrail(
                _currentUser.UserGuid.Value,
                "ASSIGN_APPROVER",
                nameof(Approver),
                approver.Id);
            _context.AuditTrails.Add(auditTrail);
            await _context.SaveChangesAsync(cancellationToken);
        }

        await _mediator.Publish(new EntityCreatedEvent<Approver>(approver, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(approver.Id);
    }

    private Guid? ResolveLegacyUserLoginId(string subject, string? username)
    {
        try
        {
            var conn = _db.Database.GetDbConnection();
            var shouldClose = conn.State != ConnectionState.Open;
            if (shouldClose)
            {
                conn.Open();
            }

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT TOP(1) [Id]
FROM [UserLogins]
WHERE [IsDeleted] = 0
  AND [IsActive] = 1
  AND (
      CAST([Id] AS nvarchar(36)) = @subject
      OR [AspNetUserId] = @subject
      OR [Username] = @username
  )";

            var p1 = cmd.CreateParameter();
            p1.ParameterName = "@subject";
            p1.Value = subject;
            cmd.Parameters.Add(p1);

            var p2 = cmd.CreateParameter();
            p2.ParameterName = "@username";
            p2.Value = (object?)username ?? DBNull.Value;
            cmd.Parameters.Add(p2);

            var value = cmd.ExecuteScalar();

            if (shouldClose)
            {
                conn.Close();
            }

            if (value != null && Guid.TryParse(value.ToString(), out var mappedId))
            {
                return mappedId;
            }
        }
        catch
        {
            // ignore mapping failures
        }

        return null;
    }
}