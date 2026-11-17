import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  ManagerDataService,
  ManagerRequestRow,
} from '../../../../core/services/manager-data.service';

@Component({
  selector: 'app-all-requests',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './all-requests.component.html',
  styleUrls: ['./all-requests.component.scss']
})
export class AllRequestsComponent implements OnInit {
  private readonly managerData = inject(ManagerDataService);

  protected readonly requests = signal<ManagerRequestRow[]>([]);
  protected readonly searchQuery = signal('');
  protected readonly statusFilter = signal('All');
  protected readonly selectedRequest = signal<ManagerRequestRow | null>(null);

  protected readonly filteredRequests = computed(() => {
    let list = this.requests();
    const status = this.statusFilter();
    if (status !== 'All') {
      list = list.filter((r) => r.status.toLowerCase() === status.toLowerCase());
    }
    const q = this.searchQuery().toLowerCase().trim();
    if (q) {
      list = list.filter(
        (r) =>
          r.requestNumber.toLowerCase().includes(q) ||
          r.requesterName.toLowerCase().includes(q) ||
          r.department.toLowerCase().includes(q) ||
          r.description.toLowerCase().includes(q),
      );
    }
    return list;
  });

  ngOnInit(): void {
    this.loadRequests();
  }

  loadRequests(): void {
    this.managerData.syncServiceRequests().subscribe(() => {
      this.requests.set(this.managerData.requestRows());
    });
  }

  viewDetails(req: ManagerRequestRow): void {
    this.selectedRequest.set(req);
  }

  closeDetails(): void {
    this.selectedRequest.set(null);
  }
}
