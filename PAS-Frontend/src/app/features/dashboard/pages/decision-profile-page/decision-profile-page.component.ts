import { Component, inject, signal, ViewChild, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { EditProfileModalComponent } from '../../components/edit-profile-modal/edit-profile-modal.component';
import { ChangePasswordModalComponent } from '../../components/change-password-modal/change-password-modal.component';
import { AuthService } from '../../../../core/services/auth.service';
import { FaceDetectionService } from '../../../../core/services/face-detection.service';

@Component({
  selector: 'app-decision-profile-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './decision-profile-page.component.html',
  styleUrl: './decision-profile-page.component.scss',
})
export class DecisionProfilePageComponent implements OnInit, OnDestroy {
  private readonly modalService = inject(NgbModal);
  private readonly sanitizer = inject(DomSanitizer);
  private readonly authService = inject(AuthService);

  @ViewChild('videoElement') videoElement!: ElementRef<HTMLVideoElement>;
  @ViewChild('canvasElement') canvasElement!: ElementRef<HTMLCanvasElement>;

  profilePhoto = signal<SafeUrl | string | null>(null);
  isSelectingPhoto = signal(false);
  isCameraActive = signal(false);
  isValidatingPhoto = signal(false);
  cameraStream = signal<MediaStream | null>(null);

  private readonly faceDetectionService = inject(FaceDetectionService);

  ngOnInit(): void {
    // Check current 2FA status when component loads
    this.checkTwoFactorStatus();
  }

  private checkTwoFactorStatus(): void {
    // In a real application, you would call an API to check the current 2FA status
    // For now, we'll assume it's disabled by default
    // You can extend this to call a backend API that returns the current 2FA status
    const user = this.authService.getCurrentUser();
    if (user) {
      // Check if user has 2FA enabled (this would come from user profile/settings API)
      // For demo purposes, we'll check if there's any stored 2FA preference
      const stored2FAStatus = localStorage.getItem('user_2fa_enabled');
      const stored2FAMethod = localStorage.getItem('user_2fa_method') as 'sms' | 'email' | 'app';
      
      if (stored2FAStatus === 'true') {
        this.twoFactorEnabled.set(true);
        this.twoFactorSetupComplete.set(true);
        if (stored2FAMethod) {
          this.twoFactorMethod.set(stored2FAMethod);
        }
      }
    }
  }

  profile = {
    name: 'Sarah Smith',
    title: 'IT Department Manager',
    employeeCode: 'EMP-002',
    department: 'IT Department',
    email: 'sarah.smith@afrocom.com',
    phone: '+251-912-345678',
    joinDate: 'Mar 01, 2019',
  };

  twoFactorEnabled = signal(false);
  twoFactorMethod = signal<'sms' | 'email' | 'app'>('sms');
  phoneNumber = signal('');
  twoFactorSetupComplete = signal(false);
  twoFactorVerificationSent = signal(false);
  twoFactorVerificationCode = signal('');
  isEnabling2FA = signal(false);
  isDisabling2FA = signal(false);
  isInSetupMode = signal(false);
  isSendingCode = signal(false);

  approvalLimits = {
    singleRequestLimit: '$10,000',
    monthlyDepartmentLimit: '$50,000',
    requireEscalationAbove: '$10,000',
    escalationApprover: 'Director of Operations',
  };

  departmentScope = {
    department: 'IT Department',
    staffCount: 12,
    subDepartments: 'None',
    approvalAuthority: 'All requests from IT Department',
  };

  notificationPreferences = {
    emailEveryApproval: true,
    smsUrgent: true,
    dailySummary: true,
    weeklyReport: true,
    budgetAlerts: false,
  };

  editProfile(): void {
    const modalRef = this.modalService.open(EditProfileModalComponent, {
      centered: true,
      backdrop: 'static',
    });

    modalRef.result
      .then((result) => {
        if (result) {
          this.profile.name = result.fullName;
          this.profile.email = result.email;
          this.profile.phone = result.phone;
          this.profile.department = result.department;
          this.profile.title = result.position;
          alert('Profile updated successfully!');
        }
      })
      .catch(() => {});
  }

  changePhoto(): void {
    this.isSelectingPhoto.set(true);
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.handleFile(input.files[0]);
    }
  }

  private async handleFile(file: File): Promise<void> {
    if (file.size > 20 * 1024 * 1024) {
      alert('File too large. Max 20MB.');
      return;
    }

    if (!file.type.startsWith('image/')) {
      alert('Please select an image file.');
      return;
    }

    this.isValidatingPhoto.set(true);

    try {
      const result = await this.faceDetectionService.validateProfilePhoto(file);

      // Delay for premium feel
      await new Promise((resolve) => setTimeout(resolve, 1500));

      this.isValidatingPhoto.set(false);

      if (!result.valid) {
        alert(`Security Alert: ${result.message}`);
        return;
      }

      const reader = new FileReader();
      reader.onload = (e: ProgressEvent<FileReader>) => {
        const base64 = e.target?.result as string;
        this.profilePhoto.set(this.sanitizer.bypassSecurityTrustUrl(base64));
        this.isSelectingPhoto.set(false);
        alert('Face verified! Profile photo updated.');
      };
      reader.readAsDataURL(file);
    } catch (error) {
      console.error('Photo validation error:', error);
      this.isValidatingPhoto.set(false);
      alert('Failed to analyze photo. Please try again.');
    }
  }

  async startCamera(): Promise<void> {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({
        video: { facingMode: 'user', width: 400, height: 400 },
      });
      this.cameraStream.set(stream);
      this.isCameraActive.set(true);

      setTimeout(() => {
        if (this.videoElement?.nativeElement) {
          this.videoElement.nativeElement.srcObject = stream;
        }
      }, 100);
    } catch (err) {
      console.error('Camera error:', err);
      alert('Could not access camera.');
    }
  }

  capturePhoto(): void {
    const video = this.videoElement.nativeElement;
    const canvas = this.canvasElement.nativeElement;
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    const ctx = canvas.getContext('2d');
    if (ctx) {
      ctx.drawImage(video, 0, 0);
      canvas.toBlob(async (blob) => {
        if (blob) {
          const file = new File([blob], 'photo.jpg', { type: 'image/jpeg' });
          await this.handleFile(file);
          this.stopCamera();
        }
      }, 'image/jpeg');
    }
  }

  stopCamera(): void {
    this.cameraStream()
      ?.getTracks()
      .forEach((t) => t.stop());
    this.cameraStream.set(null);
    this.isCameraActive.set(false);
  }

  cancelUpload(): void {
    this.stopCamera();
    this.isSelectingPhoto.set(false);
  }

  ngOnDestroy(): void {
    this.stopCamera();
  }

  changePassword(): void {
    const modalRef = this.modalService.open(ChangePasswordModalComponent, {
      centered: true,
      backdrop: 'static',
      scrollable: true,
    });

    modalRef.result
      .then((result) => {
        if (result && result.currentPassword && result.newPassword) {
          this.authService.changePassword(result).subscribe({
            next: (res) => {
              if (res.succeeded) {
                alert('Password changed successfully!');
              } else {
                alert('Failed to change password: ' + res.message);
              }
            },
            error: (err) => {
              console.error('Password change error', err);
              alert('An error occurred while changing your password.');
            },
          });
        }
      })
      .catch(() => {});
  }

  toggle2FA(): void {
    if (this.twoFactorEnabled()) {
      this.disable2FA();
    } else {
      this.enable2FA();
    }
  }

  enable2FA(): void {
    // Start setup mode instead of immediately trying to enable
    this.isInSetupMode.set(true);
    this.twoFactorEnabled.set(false);
    this.twoFactorSetupComplete.set(false);
  }

  sendVerificationCode(): void {
    const method = this.twoFactorMethod();
    let contactInfo: string | undefined;

    if (method === 'sms') {
      if (!this.phoneNumber()) {
        alert('Please enter a phone number first.');
        return;
      }
      contactInfo = this.phoneNumber();
    } else if (method === 'email') {
      contactInfo = this.profile.email;
    }

    this.isSendingCode.set(true);

    // Simulate sending verification code
    setTimeout(() => {
      this.isSendingCode.set(false);
      this.twoFactorVerificationSent.set(true);
      alert(`Verification code sent to ${contactInfo}!`);
    }, 1000);
  }

  completeSetup(): void {
    const method = this.twoFactorMethod();
    let contactInfo: string | undefined;

    // Validate required information based on method
    if (method === 'sms') {
      if (!this.phoneNumber()) {
        alert('Please enter a phone number to enable SMS 2FA.');
        return;
      }
      contactInfo = this.phoneNumber();
    } else if (method === 'email') {
      contactInfo = this.profile.email;
    }

    this.isEnabling2FA.set(true);

    this.authService.enable2FA(method, contactInfo).subscribe({
      next: (response) => {
        this.isEnabling2FA.set(false);
        if (response.succeeded) {
          this.twoFactorEnabled.set(true);
          this.twoFactorSetupComplete.set(true);
          this.isInSetupMode.set(false);
          // Store 2FA status locally for persistence
          localStorage.setItem('user_2fa_enabled', 'true');
          localStorage.setItem('user_2fa_method', method);
          
          let successMessage = `2FA enabled successfully using ${method.toUpperCase()}!`;
          if (method === 'sms') {
            successMessage += ` Verification codes will be sent to ${contactInfo}.`;
          } else if (method === 'email') {
            successMessage += ` Verification codes will be sent to ${contactInfo}.`;
          } else if (method === 'app') {
            successMessage += ` Please scan the QR code with your authenticator app.`;
          }
          
          alert(successMessage);
        } else {
          alert(`Failed to enable 2FA: ${response.message}`);
        }
      },
      error: (error) => {
        this.isEnabling2FA.set(false);
        console.error('2FA enable error:', error);
        alert('An error occurred while enabling 2FA. Please try again.');
      }
    });
  }

  cancel2FASetup(): void {
    this.isInSetupMode.set(false);
    this.twoFactorEnabled.set(false);
    this.twoFactorSetupComplete.set(false);
    this.phoneNumber.set('');
    this.twoFactorMethod.set('sms');
    this.twoFactorVerificationSent.set(false);
  }

  disable2FA(): void {
    if (!confirm('Are you sure you want to disable 2FA? This will make your account less secure.')) {
      return;
    }

    this.isDisabling2FA.set(true);

    this.authService.disable2FA().subscribe({
      next: (response) => {
        this.isDisabling2FA.set(false);
        if (response.succeeded) {
          this.twoFactorEnabled.set(false);
          this.twoFactorMethod.set('sms');
          this.phoneNumber.set('');
          this.twoFactorVerificationSent.set(false);
          this.twoFactorVerificationCode.set('');
          this.twoFactorSetupComplete.set(false);
          this.isInSetupMode.set(false);
          // Remove 2FA status from local storage
          localStorage.removeItem('user_2fa_enabled');
          localStorage.removeItem('user_2fa_method');
          alert(`2FA disabled successfully! ${response.message}`);
        } else {
          alert(`Failed to disable 2FA: ${response.message}`);
        }
      },
      error: (error) => {
        this.isDisabling2FA.set(false);
        console.error('2FA disable error:', error);
        alert('An error occurred while disabling 2FA. Please try again.');
      }
    });
  }

  updateTwoFactorMethod(event: any): void {
    this.twoFactorMethod.set(event.target?.value ?? 'sms');
  }

  twoFactorPhone(): string {
    return this.phoneNumber();
  }

  updateTwoFactorPhone(event: any): void {
    this.phoneNumber.set(event.target?.value ?? '');
  }

  saveTwoFactorPhone(): void {
    if (!this.phoneNumber()) {
      alert('Please enter a valid phone number.');
      return;
    }
    
    // For SMS method, we'll enable 2FA directly with the phone number
    this.enable2FA();
  }

  updateTwoFactorVerificationCode(event: any): void {
    this.twoFactorVerificationCode.set(event.target?.value ?? '');
  }

  verifyTwoFactorCode(): void {
    // This method can be used for additional verification if needed
    // For now, we'll simulate verification since the backend handles the actual verification
    if (this.twoFactorVerificationCode().length >= 4) {
      this.twoFactorSetupComplete.set(true);
      this.twoFactorVerificationSent.set(false);
      alert('2FA setup completed successfully!');
    } else {
      alert('Please enter a valid verification code.');
    }
  }

  savePreferences(): void {
    console.log('Saving preferences:', this.notificationPreferences);
    alert('Preferences saved successfully!');
  }
}
