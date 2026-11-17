import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ManagerDataService } from '../../../../core/services/manager-data.service';
import { WorkflowService } from '../../../../core/services/workflow.service';
import { CurrentUserService } from '../../../../core/services/current-user.service';
import { ServiceRequestService } from '../../../requisition/service-requests/services/service-request.service';

export interface PendingApproval {
  id?: string;
  srNumber: string;
  requester: string;
  priority: 'Urgent' | 'Medium' | 'Normal' | string;
  requestedDate: string;
  requiredDate: string;
  items: number;
  value: string;
  waiting: string;
}

export interface ApprovalHistory {
  srNumber: string;
  decision: 'Approved' | 'Rejected' | string;
  value: string;
  responseTime: string;
}

export interface WorkflowRule {
  rule: string;
}

@Component({
  selector: 'app-approval-queue-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './approval-queue-page.component.html',
  styleUrl: './approval-queue-page.component.scss',
})
export class ApprovalQueuePageComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly managerData = inject(ManagerDataService);
  private readonly workflowService = inject(WorkflowService);
  private readonly currentUserService = inject(CurrentUserService);
  private readonly serviceRequestService = inject(ServiceRequestService);

  dateFrom = new Date(Date.now() - 30 * 86400000).toLocaleDateString();
  dateTo = new Date().toLocaleDateString();
  priorityFilter = 'All';
  valueFilter = 'All';

  allPendingApprovals: PendingApproval[] = [];
  pendingApprovals: PendingApproval[] = [];
  approvalHistory: ApprovalHistory[] = [];

  workflowRules: WorkflowRule[] = [
    { rule: 'Requests under $1,000: Auto-approve after 48 hours if no action' },
    { rule: 'Requests $1,000 - $10,000: Manager approval required' },
    { rule: 'Requests over $10,000: Requires Director approval' },
    { rule: 'Urgent requests: Response required within 4 hours' },
    { rule: 'Escalation after: 5 days of no response' },
  ];

  ngOnInit(): void {
    this.refresh();
  }

  refresh(): void {
    this.managerData.syncServiceRequests().subscribe(() => {
      const pending = this.managerData.requestRows('pending');
      const approved = this.managerData.requestRows('approved');
      const rejected = this.managerData.requestRows('rejected');
      const now = Date.now();

      this.allPendingApprovals = pending.map((p) => {
        const reqDate = new Date(p.requestedDate).getTime();
        const diffHours = !isNaN(reqDate) ? Math.max(1, Math.round((now - reqDate) / (1000 * 60 * 60))) : 2;
        const waiting = diffHours < 24 ? `${diffHours} hours` : `${Math.round(diffHours / 24)} day(s)`;
        return {
          id: p.id,
          srNumber: p.requestNumber,
          requester: p.requesterName,
          priority: p.priority,
          requestedDate: p.requestedDate,
          requiredDate: p.requiredDate,
          items: p.itemCount,
          value: `$${p.estimatedValue.toLocaleString()}`,
          waiting,
        };
      });

      this.pendingApprovals = [...this.allPendingApprovals];

      this.approvalHistory = [...approved, ...rejected].map((r) => ({
        srNumber: r.requestNumber,
        decision: r.status === 'Approved' ? 'Approved' : 'Rejected',
        value: `$${r.estimatedValue.toLocaleString()}`,
        responseTime: '1-2 days',
      }));
    });
  }

  approveRequest(srNumber: string): void {
    const item = this.allPendingApprovals.find((p) => p.srNumber === srNumber);
    if (!item?.id) return;

    const user = this.currentUserService.getCurrentUserValue();
    const managerName = user?.fullName || user?.username || 'Manager';
    const queueId = this.workflowService.getManagerQueueIdForCurrentUser();

    this.workflowService.managerReviewRequest(
      item.id,
      'approve',
      'Approved via Approval Queue',
      queueId,
      managerName,
    );
    this.serviceRequestService
      .approveServiceRequest({ id: item.id, remarks: 'Approved via Approval Queue' })
      .subscribe(() => this.refresh());
  }

  rejectRequest(srNumber: string): void {
    const item = this.allPendingApprovals.find((p) => p.srNumber === srNumber);
    if (!item?.id) return;

    const reason = prompt(`Enter rejection reason for ${srNumber}:`);
    if (reason === null) return;

    const user = this.currentUserService.getCurrentUserValue();
    const managerName = user?.fullName || user?.username || 'Manager';
    const queueId = this.workflowService.getManagerQueueIdForCurrentUser();

    this.workflowService.managerReviewRequest(
      item.id,
      'reject',
      reason || 'Rejected via Approval Queue',
      queueId,
      managerName,
    );
    this.serviceRequestService
      .reject({ id: item.id, reason: reason || 'Rejected via Approval Queue' })
      .subscribe(() => this.refresh());
  }

  viewRequestDetails(srNumber: string): void {
    void this.router.navigate(['/manager/approvals/pending']);
  }

  applyFilters(): void {
    if (this.priorityFilter === 'All') {
      this.pendingApprovals = [...this.allPendingApprovals];
    } else {
      this.pendingApprovals = this.allPendingApprovals.filter(
        (p) => p.priority.toLowerCase() === this.priorityFilter.toLowerCase(),
      );
    }
  }

  viewFullHistory(): void {
    void this.router.navigate(['/manager/approvals/history']);
  }

  editWorkflowRules(): void {
    void this.router.navigate(['/manager/workflows/all']);
  }

  getPriorityIcon(priority: string): string {
    switch (priority) {
      case 'Urgent':
        return '🔴';
      case 'Medium':
        return '🟡';
      case 'Normal':
        return '🟢';
      default:
        return '⚪';
    }
  }

  getDecisionIcon(decision: string): string {
    return decision === 'Approved' ? '✅' : '❌';
  }
}
