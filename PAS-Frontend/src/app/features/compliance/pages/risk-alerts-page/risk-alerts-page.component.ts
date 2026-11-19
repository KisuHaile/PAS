import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RiskAlert, RiskAlertStatus, RiskSeverity } from '../../../../core/models/risk-alert.model';
import { RiskAlertService } from '../../../../core/services/risk-alert.service';

@Component({
  selector: 'app-risk-alerts-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './risk-alerts-page.component.html',
  styleUrl: './risk-alerts-page.component.scss',
})
export class RiskAlertsPageComponent {
  private readonly riskAlertService = inject(RiskAlertService);
  private readonly router = inject(Router);

  // Filters state
  readonly searchQuery = signal('');
  readonly selectedSeverity = signal<'All' | RiskSeverity>('All');
  readonly selectedStatus = signal<'All' | RiskAlertStatus>('All');

  // Modal inspection state
  readonly selectedAlertForDetails = signal<RiskAlert | null>(null);

  // Alerts from RiskAlertService
  readonly allAlerts = this.riskAlertService.alerts;
  readonly newCount = this.riskAlertService.newCount;

  // Severity summary counts
  readonly criticalCount = computed(() => this.allAlerts().filter((a) => a.severity === 'Critical').length);
  readonly highCount = computed(() => this.allAlerts().filter((a) => a.severity === 'High').length);
  readonly mediumCount = computed(() => this.allAlerts().filter((a) => a.severity === 'Medium').length);
  readonly lowCount = computed(() => this.allAlerts().filter((a) => a.severity === 'Low').length);

  readonly acknowledgedCount = computed(() => this.allAlerts().filter((a) => a.status === 'Acknowledged').length);
  readonly resolvedCount = computed(() => this.allAlerts().filter((a) => a.status === 'Resolved').length);

  // Filtered alerts
  readonly filteredAlerts = computed<RiskAlert[]>(() => {
    let list = this.allAlerts();

    const query = this.searchQuery().trim().toLowerCase();
    if (query) {
      list = list.filter(
        (a) =>
          a.title.toLowerCase().includes(query) ||
          a.description.toLowerCase().includes(query) ||
          (a.relatedLabel && a.relatedLabel.toLowerCase().includes(query)) ||
          a.relatedType.toLowerCase().includes(query),
      );
    }

    const sev = this.selectedSeverity();
    if (sev !== 'All') {
      list = list.filter((a) => a.severity === sev);
    }

    const st = this.selectedStatus();
    if (st !== 'All') {
      list = list.filter((a) => a.status === st);
    }

    return list;
  });

  setSeverityFilter(severity: 'All' | RiskSeverity): void {
    this.selectedSeverity.set(severity);
  }

  setStatusFilter(status: 'All' | RiskAlertStatus): void {
    this.selectedStatus.set(status);
  }

  onSearchChange(val: string): void {
    this.searchQuery.set(val);
  }

  resetFilters(): void {
    this.searchQuery.set('');
    this.selectedSeverity.set('All');
    this.selectedStatus.set('All');
  }

  acknowledgeAlert(id: string, event?: Event): void {
    event?.stopPropagation();
    this.riskAlertService.acknowledge(id);
    if (this.selectedAlertForDetails()?.id === id) {
      const updated = this.riskAlertService.getAlertById(id);
      if (updated) {
        this.selectedAlertForDetails.set(updated);
      }
    }
  }

  resolveAlert(id: string, event?: Event): void {
    event?.stopPropagation();
    this.riskAlertService.resolve(id);
    if (this.selectedAlertForDetails()?.id === id) {
      const updated = this.riskAlertService.getAlertById(id);
      if (updated) {
        this.selectedAlertForDetails.set(updated);
      }
    }
  }

  openDetails(alert: RiskAlert): void {
    this.selectedAlertForDetails.set(alert);
  }

  closeDetails(): void {
    this.selectedAlertForDetails.set(null);
  }

  refreshAlerts(): void {
    this.riskAlertService.refreshAlerts();
  }

  navigateToRelated(alert: RiskAlert): void {
    this.closeDetails();
    if (alert.relatedType === 'ServiceRequest') {
      void this.router.navigate(['/compliance-officer/dashboard']);
    } else {
      void this.router.navigate(['/compliance-officer/dashboard']);
    }
  }

  getSeverityIcon(severity: RiskSeverity): string {
    switch (severity) {
      case 'Critical':
        return 'bi-exclamation-octagon-fill';
      case 'High':
        return 'bi-exclamation-triangle-fill';
      case 'Medium':
        return 'bi-info-circle-fill';
      case 'Low':
      default:
        return 'bi-check-circle-fill';
    }
  }

  getStatusIcon(status: RiskAlertStatus): string {
    switch (status) {
      case 'New':
        return 'bi-bell-fill';
      case 'Acknowledged':
        return 'bi-eye-fill';
      case 'Resolved':
        return 'bi-check2-circle';
    }
  }
}
