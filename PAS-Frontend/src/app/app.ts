import { Component, signal, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected readonly title = signal('AFRICOM Frontend');

  ngOnInit() {
    // Clear any existing toast messages on app initialization
    this.clearToasts();
  }

  private clearToasts() {
    if (typeof document === 'undefined') {
      return;
    }

    // Clear any existing error toasts
    const errorToasts = document.querySelectorAll('div[style*="background-color: #dc3545"]');
    errorToasts.forEach(toast => {
      if (toast.parentNode) {
        toast.parentNode.removeChild(toast);
      }
    });
  }
}

