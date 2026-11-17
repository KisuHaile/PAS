import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import {
  NotificationFeatureService,
  Notification,
} from '../../../common/notifications/services/notification-feature.service';
import { ManagerDataService } from '../../../../core/services/manager-data.service';
import { WorkflowService } from '../../../../core/services/workflow.service';
import { CurrentUserService } from '../../../../core/services/current-user.service';

export interface ApprovalRequest {
  id?: string;
  srNumber: string;
  requester: string;
  priority: string;
  value: string;
}

export interface DepartmentAlert {
  message: string;
}

export interface BudgetAlert {
  message: string;
}

@Component({
  selector: 'app-manager-notifications-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './manager-notifications-page.component.html',
  styleUrl: './manager-notifications-page.component.scss',
})
export class ManagerNotificationsPageComponent {
  private readonly router = inject(Router);
  private readonly notificationService = inject(NotificationFeatureService);
  private readonly managerData = inject(ManagerDataService);
  private readonly workflowService = inject(WorkflowService);
  private readonly currentUserService = inject(CurrentUserService);

  searchQuery = '';
  filterType = 'all';

  notifications: Notification[] = [];
  approvalRequests: ApprovalRequest[] = [];
  departmentAlerts: DepartmentAlert[] = [];
  budgetAlerts: BudgetAlert[] = [];

  constructor() {
    this.loadNotifications();
    this.loadPendingApprovalRequests();
  }

  loadNotifications(): void {
    this.notificationService.getNotifications().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.notifications = response.data.notifications;
        }
      },
      error: (err) => console.error('Error loading notifications:', err)
    });
  }

  loadPendingApprovalRequests(): void {
    this.managerData.syncServiceRequests().subscribe(() => {
      const pending = this.managerData.requestRows('pending');
      this.approvalRequests = pending.map((p) => ({
        id: p.id,
        srNumber: p.requestNumber,
        requester: p.requesterName,
        priority: p.priority,
        value: `$${p.estimatedValue.toLocaleString()}`,
      }));

      // Generate context alerts if there are high value or urgent requests
      const urgentRequests = pending.filter((p) => p.priority === 'Urgent' || p.priority === 'High');
      if (urgentRequests.length > 0) {
        this.departmentAlerts = [
          { message: `${urgentRequests.length} urgent service request(s) require immediate manager attention.` },
        ];
      }

      const highValue = pending.filter((p) => p.estimatedValue > 1000);
      if (highValue.length > 0) {
        this.budgetAlerts = [
          { message: `${highValue.length} request(s) exceed $1,000 threshold and need budget review.` },
        ];
      }
    });
  }

  get filteredNotifications(): Notification[] {
    let filtered = this.notifications;

    if (this.filterType !== 'all') {
      if (this.filterType === 'approval') {
        filtered = filtered.filter((n) => {
          const msg = n.message.toLowerCase();
          return msg.includes('approval') || msg.includes('request') || msg.includes('requisition');
        });
      } else if (this.filterType === 'department') {
        filtered = filtered.filter((n) => n.message.toLowerCase().includes('department'));
      } else if (this.filterType === 'budget') {
        filtered = filtered.filter((n) => n.message.toLowerCase().includes('budget') || n.message.toLowerCase().includes('cost'));
      }
    }

    if (this.searchQuery) {
      filtered = filtered.filter((n) =>
        n.message.toLowerCase().includes(this.searchQuery.toLowerCase())
      );
    }

    return filtered;
  }

  get unreadCount(): number {
    return this.notifications.filter((n) => !n.isRead).length;
  }

  markAllAsRead(): void {
    this.notificationService.markAllAsRead().subscribe(() => {
      this.notifications.forEach((n) => (n.isRead = true));
    });
  }

  markAsRead(id: string): void {
    this.notificationService.markAsRead(id).subscribe(() => {
      const notification = this.notifications.find((n) => n.id === id);
      if (notification) {
        notification.isRead = true;
      }
    });
  }

  dismiss(id: string): void {
    this.notificationService.deleteNotification(id).subscribe(() => {
      this.notifications = this.notifications.filter((n) => n.id !== id);
    });
  }

  getNotificationIcon(message: string): string {
    const msg = message.toLowerCase();
    if (msg.includes('approval') || msg.includes('request')) return '✅';
    if (msg.includes('department')) return '🏢';
    if (msg.includes('budget')) return '💰';
    return '🔔';
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleString();
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

  approveRequest(srNumber?: string): void {
    if (srNumber) {
      const item = this.approvalRequests.find((r) => r.srNumber === srNumber);
      if (item?.id) {
        const user = this.currentUserService.getCurrentUserValue();
        this.workflowService.managerReviewRequest(
          item.id,
          'approve',
          'Approved via notifications',
          this.workflowService.getManagerQueueIdForCurrentUser(),
          user?.fullName || user?.username || 'Manager',
        );
      }
      this.approvalRequests = this.approvalRequests.filter((r) => r.srNumber !== srNumber);
    }
  }

  rejectRequest(srNumber?: string): void {
    if (srNumber) {
      const item = this.approvalRequests.find((r) => r.srNumber === srNumber);
      if (item?.id) {
        const user = this.currentUserService.getCurrentUserValue();
        this.workflowService.managerReviewRequest(
          item.id,
          'reject',
          'Rejected via notifications',
          this.workflowService.getManagerQueueIdForCurrentUser(),
          user?.fullName || user?.username || 'Manager',
        );
      }
      this.approvalRequests = this.approvalRequests.filter((r) => r.srNumber !== srNumber);
    }
  }

  viewRequestDetails(srNumber?: string): void {
    void this.router.navigate(['/manager/approvals/pending']);
  }

  viewDepartmentRequest(): void {
    void this.router.navigate(['/manager/dashboard']);
  }

  viewBudget(): void {
    void this.router.navigate(['/manager/dashboard']);
  }

  viewAllApprovalRequests(): void {
    void this.router.navigate(['/manager/approvals/pending']);
  }
}
