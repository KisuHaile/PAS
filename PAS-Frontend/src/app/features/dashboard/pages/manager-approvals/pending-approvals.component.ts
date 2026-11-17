import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription, take } from 'rxjs';
import {
  ManagerDataService,
  ManagerRequestRow,
} from '../../../../core/services/manager-data.service';
import { WorkflowService } from '../../../../core/services/workflow.service';
import { CurrentUserService } from '../../../../core/services/current-user.service';
import { ServiceRequestService } from '../../../requisition/service-requests/services/service-request.service';

@Component({
  selector: 'app-pending-approvals',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pending-approvals.component.html',
  styleUrls: ['./pending-approvals.component.scss']
})
export class PendingApprovalsComponent implements OnInit, OnDestroy {
  private readonly managerData = inject(ManagerDataService);
  private readonly workflowService = inject(WorkflowService);
  private readonly currentUserService = inject(CurrentUserService);
  private readonly serviceRequestService = inject(ServiceRequestService);
  private readonly subs: Subscription[] = [];

  protected readonly pendingRequests = signal<ManagerRequestRow[]>([]);
  protected readonly selectedRequest = signal<ManagerRequestRow | null>(null);
  protected readonly rejectingRequest = signal<ManagerRequestRow | null>(null);
  protected rejectionReason = '';
  protected readonly feedbackMessage = signal<{ text: string; type: 'success' | 'error' } | null>(null);

  ngOnInit(): void {
    this.loadRequests();
    this.subs.push(this.workflowService.getRequestUpdates().subscribe(() => this.loadRequests()));
  }

  ngOnDestroy(): void {
    this.subs.forEach((sub) => sub.unsubscribe());
  }

  protected approve(id: string): void {
    const user = this.currentUserService.getCurrentUserValue();
    const managerName = user?.fullName || user?.username || 'Manager';
    const queueId = this.workflowService.getManagerQueueIdForCurrentUser();

    this.workflowService.managerReviewRequest(
      id,
      'approve',
      'Approved from pending approvals',
      queueId,
      managerName,
    );
    this.serviceRequestService
      .approveServiceRequest({ id, remarks: 'Approved from pending approvals' })
      .pipe(take(1))
      .subscribe({
        next: () => {
          this.setFeedback('Request approved successfully!', 'success');
          this.loadRequests();
        },
        error: () => {
          this.setFeedback('Request approved in local workflow.', 'success');
          this.loadRequests();
        },
      });
  }

  protected openRejectModal(request: ManagerRequestRow): void {
    this.rejectionReason = '';
    this.rejectingRequest.set(request);
  }

  protected closeRejectModal(): void {
    this.rejectingRequest.set(null);
    this.rejectionReason = '';
  }

  protected confirmReject(): void {
    const request = this.rejectingRequest();
    if (!request) return;

    const user = this.currentUserService.getCurrentUserValue();
    const managerName = user?.fullName || user?.username || 'Manager';
    const queueId = this.workflowService.getManagerQueueIdForCurrentUser();
    const reason = this.rejectionReason.trim() || 'Rejected from pending approvals';

    this.workflowService.managerReviewRequest(
      request.id,
      'reject',
      reason,
      queueId,
      managerName,
    );
    this.serviceRequestService
      .reject({ id: request.id, reason })
      .pipe(take(1))
      .subscribe({
        next: () => {
          this.setFeedback('Request rejected.', 'error');
          this.closeRejectModal();
          this.loadRequests();
        },
        error: () => {
          this.setFeedback('Request rejected in local workflow.', 'error');
          this.closeRejectModal();
          this.loadRequests();
        },
      });
  }

  protected viewDetails(id: string): void {
    const request = this.pendingRequests().find((item) => item.id === id);
    if (request) {
      this.selectedRequest.set(request);
    }
  }

  protected closeDetails(): void {
    this.selectedRequest.set(null);
  }

  private setFeedback(text: string, type: 'success' | 'error'): void {
    this.feedbackMessage.set({ text, type });
    setTimeout(() => {
      this.feedbackMessage.set(null);
    }, 4000);
  }

  private loadRequests(): void {
    this.managerData.syncServiceRequests().subscribe(() => {
      this.pendingRequests.set(this.managerData.requestRows('pending'));
    });
  }
}
