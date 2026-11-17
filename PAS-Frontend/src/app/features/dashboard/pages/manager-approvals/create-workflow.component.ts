import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApprovalWorkflowService } from '../../../workflow/approval-workflows/services/approval-workflow.service';

@Component({
  selector: 'app-create-workflow',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-workflow.component.html',
  styleUrls: ['./create-workflow.component.scss']
})
export class CreateWorkflowComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly workflowService = inject(ApprovalWorkflowService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  protected readonly isEditMode = signal(false);
  protected readonly workflowId = signal<string | null>(null);
  protected readonly submitted = signal(false);
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);

  protected readonly workflowForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    description: ['', [Validators.required, Validators.minLength(10)]],
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.workflowId.set(id);
      this.loading.set(true);
      this.workflowService.getWorkflowById(id).subscribe({
        next: (response) => {
          this.loading.set(false);
          if (response.succeeded && response.data) {
            this.workflowForm.patchValue({
              name: response.data.workflowName,
              description: response.data.description,
            });
          }
        },
        error: () => {
          this.loading.set(false);
          this.errorMessage.set('Failed to load workflow for editing.');
        },
      });
    }
  }

  protected submit(): void {
    this.submitted.set(true);
    
    if (this.workflowForm.invalid) {
      return;
    }
    
    this.loading.set(true);
    
    const workflowData = {
      workflowName: this.workflowForm.value.name!,
      description: this.workflowForm.value.description!
    };
    
    if (this.isEditMode() && this.workflowId()) {
      this.workflowService.updateWorkflow(this.workflowId()!, workflowData).subscribe({
        next: (response) => {
          this.loading.set(false);
          if (response.succeeded) {
            this.router.navigate(['/manager/workflows/all']);
          } else {
            this.errorMessage.set(response.message || 'Failed to update workflow.');
          }
        },
        error: (error) => {
          this.loading.set(false);
          this.errorMessage.set(error.error?.message || 'Error updating workflow.');
        }
      });
    } else {
      this.workflowService.createWorkflow(workflowData).subscribe({
        next: (response) => {
          this.loading.set(false);
          if (response.succeeded) {
            this.router.navigate(['/manager/workflows/all']);
          } else {
            this.errorMessage.set(response.message || 'Failed to create workflow.');
          }
        },
        error: (error) => {
          this.loading.set(false);
          this.errorMessage.set(error.error?.message || 'Error creating workflow.');
        }
      });
    }
  }

  protected cancel(): void {
    this.workflowForm.reset();
    this.submitted.set(false);
    this.router.navigate(['/manager/workflows/all']);
  }
}
