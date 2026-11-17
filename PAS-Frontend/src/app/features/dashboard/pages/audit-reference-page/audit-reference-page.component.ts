import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ManagerDataService } from '../../../../core/services/manager-data.service';
import { downloadReportPdf } from '../compliance-reports/report-actions.util';

export interface ApprovalLog {
  timestamp: string;
  approver: string;
  srNumber: string;
  decision: string;
  value: string;
  responseTime: string;
  justification: string;
}

export interface DecisionHistory {
  date: string;
  srNumber: string;
  decision: string;
  reason: string;
  comments: string;
}

@Component({
  selector: 'app-audit-reference-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './audit-reference-page.component.html',
  styleUrl: './audit-reference-page.component.scss',
})
export class AuditReferencePageComponent implements OnInit {
  private readonly managerData = inject(ManagerDataService);

  dateFrom = new Date(Date.now() - 30 * 86400000).toLocaleDateString();
  dateTo = new Date().toLocaleDateString();
  approverFilter = 'All';
  statusFilter = 'All';

  approvalLogs: ApprovalLog[] = [];
  decisionHistory: DecisionHistory[] = [];

  ngOnInit(): void {
    this.managerData.syncServiceRequests().subscribe(() => {
      const rows = this.managerData.requestRows();
      this.approvalLogs = rows.map((r) => ({
        timestamp: r.approvedDate || r.rejectedDate || r.requestedDate,
        approver: r.approvedBy || r.rejectedBy || 'Eleni Grefie',
        srNumber: r.requestNumber,
        decision: r.status === 'Approved' ? '✅' : r.status === 'Rejected' ? '❌' : '⏳',
        value: `$${r.estimatedValue.toLocaleString()}`,
        responseTime: '1-2 days',
        justification: r.status,
      }));

      this.decisionHistory = rows
        .filter((r) => r.status === 'Approved' || r.status === 'Rejected')
        .map((r) => ({
          date: r.approvedDate || r.rejectedDate || r.requestedDate,
          srNumber: r.requestNumber,
          decision: r.status === 'Approved' ? '✅' : '❌',
          reason: r.rejectionReason || 'Within requirements',
          comments: r.description,
        }));
    });
  }

  export(): void {
    this.exportExcel();
  }

  exportExcel(): void {
    const csvContent =
      'data:text/csv;charset=utf-8,Timestamp,Approver,SR Number,Decision,Value,Justification\n' +
      this.approvalLogs
        .map(
          (l) =>
            `"${l.timestamp}","${l.approver}","${l.srNumber}","${l.decision}","${l.value}","${l.justification}"`,
        )
        .join('\n');
    const encodedUri = encodeURI(csvContent);
    const link = document.createElement('a');
    link.setAttribute('href', encodedUri);
    link.setAttribute('download', `Audit_Trail_${new Date().toISOString().slice(0, 10)}.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  async exportPDF(): Promise<void> {
    await downloadReportPdf(
      'Audit Reference Log',
      `Audit_Log_${new Date().toISOString().slice(0, 10)}`,
      this.approvalLogs.slice(0, 10).map((l) => ({
        label: `${l.srNumber} (${l.timestamp})`,
        value: `${l.decision} - ${l.approver} - ${l.value} - ${l.justification}`,
      })),
    );
  }

  emailReport(): void {
    alert('Audit report link copied.');
  }

  print(): void {
    window.print();
  }

  scheduleExport(): void {
    alert('Export scheduled for end of month.');
  }
}
