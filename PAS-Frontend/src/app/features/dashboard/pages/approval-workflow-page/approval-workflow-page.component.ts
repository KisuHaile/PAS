import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ManagerDataService } from '../../../../core/services/manager-data.service';
import { WorkflowService } from '../../../../core/services/workflow.service';
import { CurrentUserService } from '../../../../core/services/current-user.service';
import { ServiceRequestService } from '../../../requisition/service-requests/services/service-request.service';

export interface PendingAtMyLevel {
  id?: string;
  srNumber: string;
  requester: string;
  priority: string;
  value: string;
  waiting: string;
}

export interface CompletedAtMyLevel {
  srNumber: string;
  decision: string;
  value: string;
  responseTime: string;
}

export interface WorkflowRule {
  rule: string;
}

@Component({
  selector: 'app-approval-workflow-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './approval-workflow-page.component.html',
  styleUrl: './approval-workflow-page.component.scss',
})
export class ApprovalWorkflowPageComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly managerData = inject(ManagerDataService);
  private readonly workflowService = inject(WorkflowService);
  private readonly currentUserService = inject(CurrentUserService);
  private readonly serviceRequestService = inject(ServiceRequestService);

  pendingAtMyLevel: PendingAtMyLevel[] = [];
  completedAtMyLevel: CompletedAtMyLevel[] = [];

  workflowRules: WorkflowRule[] = [
    { rule: 'Under $1,000: Auto-approve after 48 hours' },
    { rule: '$1,000-$10,000: Manager approval required' },
    { rule: 'Over $10,000: Director approval required' },
    { rule: 'Urgent requests: Response within 4 hours' },
    { rule: 'Escalation after: 5 days no response' },
  ];

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.managerData.syncServiceRequests().subscribe(() => {
      const pending = this.managerData.requestRows('pending');
      const approved = this.managerData.requestRows('approved');
      const rejected = this.managerData.requestRows('rejected');
      const now = Date.now();

      this.pendingAtMyLevel = pending.map((p) => {
        const reqDate = new Date(p.requestedDate).getTime();
        const diffHours = !isNaN(reqDate) ? Math.max(1, Math.round((now - reqDate) / (1000 * 60 * 60))) : 2;
        const waiting = diffHours < 24 ? `${diffHours} hours` : `${Math.round(diffHours / 24)} days`;
        return {
          id: p.id,
          srNumber: p.requestNumber,
          requester: p.requesterName,
          priority: p.priority,
          value: `$${p.estimatedValue.toLocaleString()}`,
          waiting,
        };
      });

      this.completedAtMyLevel = [...approved, ...rejected].map((r) => ({
        srNumber: r.requestNumber,
        decision: r.status === 'Approved' ? 'Approved' : 'Rejected',
        value: `$${r.estimatedValue.toLocaleString()}`,
        responseTime: '1-2 days',
      }));
    });
  }

  approveRequest(srNumber: string): void {
    const item = this.pendingAtMyLevel.find((p) => p.srNumber === srNumber);
    if (!item?.id) return;

    const user = this.currentUserService.getCurrentUserValue();
    const managerName = user?.fullName || user?.username || 'Manager';
    const queueId = this.workflowService.getManagerQueueIdForCurrentUser();

    this.workflowService.managerReviewRequest(
      item.id,
      'approve',
      'Approved via Approval Workflow Page',
      queueId,
      managerName,
    );
    this.serviceRequestService
      .approveServiceRequest({ id: item.id, remarks: 'Approved via Approval Workflow Page' })
      .subscribe(() => this.loadData());
  }

  rejectRequest(srNumber: string): void {
    const item = this.pendingAtMyLevel.find((p) => p.srNumber === srNumber);
    if (!item?.id) return;

    const reason = prompt(`Enter rejection reason for ${srNumber}:`);
    if (reason === null) return;

    const user = this.currentUserService.getCurrentUserValue();
    const managerName = user?.fullName || user?.username || 'Manager';
    const queueId = this.workflowService.getManagerQueueIdForCurrentUser();

    this.workflowService.managerReviewRequest(
      item.id,
      'reject',
      reason || 'Rejected via Approval Workflow Page',
      queueId,
      managerName,
    );
    this.serviceRequestService
      .reject({ id: item.id, reason: reason || 'Rejected via Approval Workflow Page' })
      .subscribe(() => this.loadData());
  }

  viewRequestDetails(srNumber: string): void {
    void this.router.navigate(['/manager/approvals/pending']);
  }

  editWorkflowRules(): void {
    void this.router.navigate(['/manager/workflows/all']);
  }
}
