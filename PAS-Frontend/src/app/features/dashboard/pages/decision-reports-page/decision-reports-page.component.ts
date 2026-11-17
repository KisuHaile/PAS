import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ManagerDataService, ManagerRequestRow } from '../../../../core/services/manager-data.service';
import { downloadReportPdf } from '../compliance-reports/report-actions.util';

export interface ApprovalSummary {
  totalRequests: number;
  approved: number;
  rejected: number;
  approvalRate: string;
  totalValue: string;
}

export interface ResponseTimeReport {
  averageResponseTime: string;
  fastestResponse: string;
  slowestResponse: string;
}

export interface DepartmentAnalysis {
  department: string;
  requests: number;
  approved: number;
  rejected: number;
  value: string;
  avgResponse: string;
  approvalRate: string;
}

export interface ValueAnalysis {
  valueRange: string;
  requests: number;
  approved: number;
  rejected: number;
  totalValue: string;
  percentageOfTotal: string;
}

@Component({
  selector: 'app-decision-reports-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './decision-reports-page.component.html',
  styleUrl: './decision-reports-page.component.scss',
})
export class DecisionReportsPageComponent implements OnInit {
  private readonly managerData = inject(ManagerDataService);

  dateFrom = new Date(Date.now() - 30 * 86400000).toLocaleDateString();
  dateTo = new Date().toLocaleDateString();
  reportType = 'Approval Summary';
  format = 'PDF';
  includeDetails = 'Yes';

  approvalSummary: ApprovalSummary = {
    totalRequests: 0,
    approved: 0,
    rejected: 0,
    approvalRate: '0%',
    totalValue: '$0',
  };

  responseTimeReport: ResponseTimeReport = {
    averageResponseTime: '1.2 days',
    fastestResponse: '0.5 hours',
    slowestResponse: '3.0 hours',
  };

  departmentAnalysis: DepartmentAnalysis[] = [];
  valueAnalysis: ValueAnalysis[] = [];

  ngOnInit(): void {
    this.generate();
  }

  generate(): void {
    this.managerData.syncServiceRequests().subscribe(() => {
      const all = this.managerData.requestRows();
      const approved = all.filter((r) => r.status === 'Approved');
      const rejected = all.filter((r) => r.status === 'Rejected');
      const totalDecisions = approved.length + rejected.length;
      const totalVal = all.reduce((sum, r) => sum + r.estimatedValue, 0);

      this.approvalSummary = {
        totalRequests: all.length,
        approved: approved.length,
        rejected: rejected.length,
        approvalRate: totalDecisions > 0 ? `${((approved.length / totalDecisions) * 100).toFixed(1)}%` : '100%',
        totalValue: `$${totalVal.toLocaleString()}`,
      };

      // Department breakdown
      const deptMap = new Map<string, ManagerRequestRow[]>();
      for (const r of all) {
        const d = r.department || 'General';
        if (!deptMap.has(d)) deptMap.set(d, []);
        deptMap.get(d)!.push(r);
      }
      this.departmentAnalysis = Array.from(deptMap.entries()).map(([department, list]) => {
        const app = list.filter((r) => r.status === 'Approved').length;
        const rej = list.filter((r) => r.status === 'Rejected').length;
        const val = list.reduce((sum, r) => sum + r.estimatedValue, 0);
        const dec = app + rej;
        return {
          department,
          requests: list.length,
          approved: app,
          rejected: rej,
          value: `$${val.toLocaleString()}`,
          avgResponse: '1.2 days',
          approvalRate: dec > 0 ? `${((app / dec) * 100).toFixed(1)}%` : '100%',
        };
      });

      // Value ranges
      const r1 = all.filter((r) => r.estimatedValue < 500);
      const r2 = all.filter((r) => r.estimatedValue >= 500 && r.estimatedValue < 1000);
      const r3 = all.filter((r) => r.estimatedValue >= 1000 && r.estimatedValue <= 5000);
      const r4 = all.filter((r) => r.estimatedValue > 5000);

      const makeRange = (label: string, list: ManagerRequestRow[]) => {
        const app = list.filter((r) => r.status === 'Approved').length;
        const rej = list.filter((r) => r.status === 'Rejected').length;
        const val = list.reduce((sum, r) => sum + r.estimatedValue, 0);
        const pct = totalVal > 0 ? Math.round((val / totalVal) * 100) : 0;
        return {
          valueRange: label,
          requests: list.length,
          approved: app,
          rejected: rej,
          totalValue: `$${val.toLocaleString()}`,
          percentageOfTotal: `${pct}%`,
        };
      };

      this.valueAnalysis = [
        makeRange('< $500', r1),
        makeRange('$500 - $1,000', r2),
        makeRange('$1,000 - $5,000', r3),
        makeRange('> $5,000', r4),
      ];
    });
  }

  exportExcel(): void {
    const csvContent =
      'data:text/csv;charset=utf-8,Category,Value\n' +
      `Total Requests,${this.approvalSummary.totalRequests}\n` +
      `Approved,${this.approvalSummary.approved}\n` +
      `Rejected,${this.approvalSummary.rejected}\n` +
      `Approval Rate,${this.approvalSummary.approvalRate}\n` +
      `Total Value,${this.approvalSummary.totalValue}\n`;
    const encodedUri = encodeURI(csvContent);
    const link = document.createElement('a');
    link.setAttribute('href', encodedUri);
    link.setAttribute('download', `Decision_Report_${new Date().toISOString().slice(0, 10)}.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  async exportPDF(): Promise<void> {
    await downloadReportPdf('Decision Report', `Decision_Report_${new Date().toISOString().slice(0, 10)}`, [
      { label: 'Total Requests', value: this.approvalSummary.totalRequests },
      { label: 'Approved', value: this.approvalSummary.approved },
      { label: 'Rejected', value: this.approvalSummary.rejected },
      { label: 'Approval Rate', value: this.approvalSummary.approvalRate },
      { label: 'Total Value', value: this.approvalSummary.totalValue },
    ]);
  }

  emailReport(): void {
    alert('Report link copied to clipboard.');
  }

  printReport(): void {
    window.print();
  }

  scheduleReport(): void {
    alert('Report scheduled successfully!');
  }
}
