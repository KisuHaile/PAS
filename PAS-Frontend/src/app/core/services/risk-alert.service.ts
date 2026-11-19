import { Injectable, computed, inject, signal } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Router } from '@angular/router';
import { RiskAlert, RiskAlertStatus, RiskSeverity } from '../models/risk-alert.model';
import { STALE_PENDING_DAYS, ServiceRequest, WorkflowService } from './workflow.service';
import { ToastService } from './toast.service';

const STORAGE_KEY_ALERT_STATUSES = 'pas_risk_alert_statuses_v1';

@Injectable({
  providedIn: 'root',
})
export class RiskAlertService {
  private readonly workflowService = inject(WorkflowService);
  private readonly toastService = inject(ToastService);
  private readonly router = inject(Router);

  // Core signals
  private readonly _alerts = signal<RiskAlert[]>([]);
  readonly alerts = this._alerts.asReadonly();

  readonly newCount = computed(() => {
    return this._alerts().filter((a) => a.status === 'New').length;
  });

  // Observable counterparts for interop
  private readonly alertsSubject = new BehaviorSubject<RiskAlert[]>([]);
  readonly alerts$: Observable<RiskAlert[]> = this.alertsSubject.asObservable();

  private readonly newCountSubject = new BehaviorSubject<number>(0);
  readonly newCount$: Observable<number> = this.newCountSubject.asObservable();

  // Alert tracking for real-time toasts
  private knownAlertIds = new Set<string>();
  private isInitialized = false;

  constructor() {
    this.reloadAlerts();

    // Subscribe to workflow updates to detect real-time changes
    this.workflowService.getRequestUpdates().subscribe(() => {
      this.reloadAlerts();
    });

    this.workflowService.getNotificationUpdates().subscribe(() => {
      this.reloadAlerts();
    });
  }

  /**
   * Mark a risk alert as Acknowledged.
   */
  acknowledge(id: string): void {
    this.updateAlertStatus(id, 'Acknowledged');
  }

  /**
   * Mark a risk alert as Resolved.
   */
  resolve(id: string): void {
    this.updateAlertStatus(id, 'Resolved');
  }

  /**
   * Public reload/sync trigger.
   */
  refreshAlerts(): void {
    this.reloadAlerts();
  }

  /**
   * Helper to fetch an alert by its ID.
   */
  getAlertById(id: string): RiskAlert | undefined {
    return this._alerts().find((a) => a.id === id);
  }

  /**
   * Internal reload orchestrator.
   */
  private reloadAlerts(): void {
    const freshAlerts = this.fetchRiskAlertsFromSource();

    // Check for newly created alerts while logged in to trigger bottom toast
    if (this.isInitialized) {
      for (const alert of freshAlerts) {
        if (!this.knownAlertIds.has(alert.id) && alert.status === 'New') {
          this.triggerNewAlertToast(alert);
        }
      }
    }

    // Update known IDs
    freshAlerts.forEach((a) => this.knownAlertIds.add(a.id));
    this.isInitialized = true;

    // Update signal & observables
    this._alerts.set(freshAlerts);
    this.alertsSubject.next(freshAlerts);

    const newCountVal = freshAlerts.filter((a) => a.status === 'New').length;
    this.newCountSubject.next(newCountVal);
  }

  /**
   * Updates an alert status locally and persists the override to localStorage.
   */
  private updateAlertStatus(id: string, newStatus: RiskAlertStatus): void {
    const statuses = this.loadSavedStatuses();
    statuses[id] = {
      status: newStatus,
      updatedAt: new Date().toISOString(),
    };
    this.saveStatuses(statuses);

    const updated = this._alerts().map((alert) =>
      alert.id === id ? { ...alert, status: newStatus } : alert,
    );

    this._alerts.set(updated);
    this.alertsSubject.next(updated);

    const count = updated.filter((a) => a.status === 'New').length;
    this.newCountSubject.next(count);
  }

  /**
   * DATA SOURCE LOADER (All data-loading code is contained in this ONE private method).
   * Can be seamlessly replaced with an API call (e.g. HttpClient GET /api/compliance/risk-alerts)
   * in the future without modifying any components.
   */
  private fetchRiskAlertsFromSource(): RiskAlert[] {
    const savedStatuses = this.loadSavedStatuses();
    const alerts: RiskAlert[] = [];
    const seenRequestIds = new Set<string>();

    // 1. Pending requests exceeding SLA with a manager
    const staleRequests = this.workflowService.getStalePendingRequests(STALE_PENDING_DAYS);
    for (const req of staleRequests) {
      const id = `risk-stale-${req.id}`;
      seenRequestIds.add(req.id);

      const pendingDays = this.workflowService.getPendingDays(req);
      const isUrgent = req.priority === 'Urgent';
      const severity: RiskSeverity =
        pendingDays >= 10 || isUrgent ? 'Critical' : pendingDays >= 7 || req.priority === 'High' ? 'High' : 'Medium';

      const status: RiskAlertStatus = savedStatuses[id]?.status ?? 'New';

      alerts.push({
        id,
        title: `Manager Approval SLA Breach: ${req.srNumber}`,
        description: `Requisition ${req.srNumber} (${req.department}) has been pending manager review for ${pendingDays} days without action, exceeding the ${STALE_PENDING_DAYS}-day SLA.`,
        severity,
        relatedType: 'ServiceRequest',
        relatedId: req.id,
        relatedLabel: req.srNumber,
        detectedAt: req.staleEscalatedAt || req.submittedDate,
        status,
      });
    }

    // 2. Requests in 'Compliance Review' (or high-value compliance holds)
    const complianceRequests = this.workflowService.getRequestsForCompliance();
    for (const req of complianceRequests) {
      if (seenRequestIds.has(req.id)) {
        continue;
      }
      seenRequestIds.add(req.id);

      const id = `risk-compliance-${req.id}`;
      const cost = req.estimatedCost ?? 0;
      const isCritical = cost >= 25000 || req.priority === 'Urgent';
      const isHigh = cost >= 10000 || req.priority === 'High' || req.status === 'Compliance Review';
      const severity: RiskSeverity = isCritical ? 'Critical' : isHigh ? 'High' : 'Medium';

      const status: RiskAlertStatus = savedStatuses[id]?.status ?? 'New';

      alerts.push({
        id,
        title: `Compliance Review Audit Hold: ${req.srNumber}`,
        description: `Requisition ${req.srNumber} (${req.department}) requires compliance verification prior to procurement authorization (Estimated: $${cost.toLocaleString()}).`,
        severity,
        relatedType: 'ServiceRequest',
        relatedId: req.id,
        relatedLabel: req.srNumber,
        detectedAt: req.managerReviewDate || req.submittedDate,
        status,
      });
    }

    // Sort: Critical first, then High, Medium, Low; secondary sort newest detected first
    const severityRank: Record<RiskSeverity, number> = {
      Critical: 4,
      High: 3,
      Medium: 2,
      Low: 1,
    };

    return alerts.sort((a, b) => {
      const rankDiff = severityRank[b.severity] - severityRank[a.severity];
      if (rankDiff !== 0) return rankDiff;
      return new Date(b.detectedAt).getTime() - new Date(a.detectedAt).getTime();
    });
  }

  /**
   * Displays the actionable bottom toast when a new alert is detected.
   */
  private triggerNewAlertToast(alert: RiskAlert): void {
    const isHighOrCritical = alert.severity === 'Critical' || alert.severity === 'High';
    this.toastService.showBottomToast({
      title: `New Risk Alert: ${alert.severity}`,
      message: alert.title,
      type: isHighOrCritical ? 'error' : 'warning',
      duration: 6000,
      onClick: () => {
        void this.router.navigate(['/compliance-officer/risk-alerts']);
      },
    });
  }

  private loadSavedStatuses(): Record<string, { status: RiskAlertStatus; updatedAt: string }> {
    if (typeof localStorage === 'undefined') return {};
    try {
      const raw = localStorage.getItem(STORAGE_KEY_ALERT_STATUSES);
      return raw ? JSON.parse(raw) : {};
    } catch {
      return {};
    }
  }

  private saveStatuses(statuses: Record<string, { status: RiskAlertStatus; updatedAt: string }>): void {
    if (typeof localStorage === 'undefined') return;
    try {
      localStorage.setItem(STORAGE_KEY_ALERT_STATUSES, JSON.stringify(statuses));
    } catch {
      /* ignore */
    }
  }
}
