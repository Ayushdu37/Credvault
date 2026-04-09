import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [CommonModule, RouterModule, LucideAngularModule],
  templateUrl: './verify-email.component.html',
  styleUrls: ['../mfa/mfa.component.css'] // Reusing MFA layout CSS for consistency
})
export class VerifyEmailComponent implements OnInit {
  status: 'verifying' | 'success' | 'error' = 'verifying';
  errorMessage: string | null = null;
  token: string | null = null;
  email: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.token = this.route.snapshot.queryParamMap.get('token');
    this.email = this.route.snapshot.queryParamMap.get('email');

    if (!this.token || !this.email) {
      this.status = 'error';
      this.errorMessage = 'Invalid verification link. Missing token or email.';
      return;
    }

    this.verifyEmail();
  }

  verifyEmail() {
    // TODO: Wire to backend call [HttpPost("verify-email")]
    // Simulating API call
    setTimeout(() => {
      // Mock random success/failure
      const isSuccess = Math.random() > 0.1; // 90% success
      
      if (isSuccess) {
        this.status = 'success';
      } else {
        this.status = 'error';
        this.errorMessage = 'The verification link has expired or is invalid. Please request a new one.';
      }
    }, 1500);
  }
}
