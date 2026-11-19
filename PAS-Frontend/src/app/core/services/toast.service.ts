import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  success(message: string): void {
    console.log('✅ SUCCESS:', message);
    // You can integrate with a toast library here (e.g., ngx-toastr, primeng toast, etc.)
    this.showToast(message, 'success');
  }

  info(message: string, title?: string): void {
    const fullMessage = title ? `${title}: ${message}` : message;
    console.log('ℹ️ INFO:', fullMessage);
    this.showToast(fullMessage, 'info');
  }

  error(message: string): void {
    console.error('❌ ERROR:', message);
    // Disabled popup - only log to console
    // this.showToast(message, 'error');
  }

  warning(message: string): void {
    console.warn('⚠️ WARNING:', message);
    this.showToast(message, 'warning');
  }

  clearAll(): void {
    const toasts = document.querySelectorAll('div[style*="position: fixed"][style*="top: 20px"], .pas-bottom-toast');
    toasts.forEach((toast) => {
      if (toast.parentNode) {
        toast.parentNode.removeChild(toast);
      }
    });
  }

  showBottomToast(options: {
    title: string;
    message: string;
    type?: 'warning' | 'error' | 'info' | 'success';
    duration?: number;
    onClick?: () => void;
  }): void {
    if (typeof document === 'undefined') return;

    const duration = options.duration ?? 6000;
    const type = options.type ?? 'warning';

    const toast = document.createElement('div');
    toast.className = 'pas-bottom-toast';
    toast.style.cssText = `
      position: fixed;
      bottom: 24px;
      right: 24px;
      z-index: 10500;
      min-width: 320px;
      max-width: 440px;
      background: ${type === 'error' ? '#7f1d1d' : type === 'warning' ? '#78350f' : '#0f172a'};
      color: #ffffff;
      padding: 14px 18px;
      border-radius: 10px;
      box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.4), 0 8px 10px -6px rgba(0, 0, 0, 0.2);
      border-left: 5px solid ${type === 'error' ? '#ef4444' : type === 'warning' ? '#f59e0b' : '#3b82f6'};
      cursor: pointer;
      display: flex;
      flex-direction: column;
      gap: 6px;
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
      transition: transform 0.2s ease, opacity 0.25s ease;
    `;

    const iconClass = type === 'error' ? 'bi-shield-fill-exclamation' : 'bi-exclamation-triangle-fill';
    const iconColor = type === 'error' ? '#fca5a5' : '#fde68a';

    toast.innerHTML = `
      <div style="display: flex; align-items: center; justify-content: space-between; gap: 8px;">
        <div style="display: flex; align-items: center; gap: 8px; font-weight: 700; font-size: 0.95rem;">
          <i class="bi ${iconClass}" style="font-size: 1.1rem; color: ${iconColor};"></i>
          <span>${options.title}</span>
        </div>
        <span style="font-size: 0.75rem; background: rgba(255,255,255,0.2); padding: 2px 8px; border-radius: 999px;">View &rarr;</span>
      </div>
      <div style="font-size: 0.85rem; color: rgba(255, 255, 255, 0.9); line-height: 1.35; padding-left: 24px;">
        ${options.message}
      </div>
    `;

    let removed = false;
    const removeToast = () => {
      if (removed) return;
      removed = true;
      toast.style.opacity = '0';
      toast.style.transform = 'translateY(10px)';
      setTimeout(() => {
        if (toast.parentNode) {
          toast.parentNode.removeChild(toast);
        }
      }, 250);
    };

    toast.addEventListener('click', () => {
      removeToast();
      if (options.onClick) {
        options.onClick();
      }
    });

    document.body.appendChild(toast);
    setTimeout(removeToast, duration);
  }

  private showToast(message: string, type: 'success' | 'error' | 'info' | 'warning'): void {
    // Simple browser notification for now
    // You can replace this with your preferred toast library
    if ('Notification' in window && Notification.permission === 'granted') {
      new Notification(`PAS ${type.toUpperCase()}`, {
        body: message,
        icon: '/assets/images/africom-logo.png',
      });
    } else {
      // Fallback to console and could add a simple DOM toast
      const toastElement = this.createSimpleToast(message, type);
      document.body.appendChild(toastElement);

      // Auto remove after 5 seconds
      setTimeout(() => {
        if (toastElement.parentNode) {
          toastElement.parentNode.removeChild(toastElement);
        }
      }, 5000);
    }
  }

  private createSimpleToast(message: string, type: string): HTMLElement {
    const toast = document.createElement('div');
    toast.style.cssText = `
      position: fixed;
      top: 20px;
      right: 20px;
      padding: 12px 16px;
      border-radius: 4px;
      color: white;
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
      font-size: 14px;
      z-index: 10000;
      max-width: 300px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.15);
      cursor: pointer;
      transition: opacity 0.3s ease;
    `;

    // Set background color based on type
    switch (type) {
      case 'success':
        toast.style.backgroundColor = '#28a745';
        break;
      case 'error':
        toast.style.backgroundColor = '#dc3545';
        break;
      case 'warning':
        toast.style.backgroundColor = '#ffc107';
        toast.style.color = '#212529';
        break;
      case 'info':
      default:
        toast.style.backgroundColor = '#17a2b8';
        break;
    }

    toast.textContent = message;

    // Click to dismiss
    toast.addEventListener('click', () => {
      if (toast.parentNode) {
        toast.parentNode.removeChild(toast);
      }
    });

    return toast;
  }
}
