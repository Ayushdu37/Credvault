import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { CardComponent } from '../../../shared/components/card/card.component';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { PaymentsService } from '../services/payments.service';
import { CardsService } from '../../cards/services/cards.service';
import { BillingService } from '../../billing/services/billing.service';
import { PaymentStatus } from '../../../core/models/payment.model';
import { Subject, takeUntil, catchError, of, switchMap, forkJoin } from 'rxjs';

@Component({
  selector: 'app-payment-detail',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, DatePipe, LucideAngularModule, CardComponent, ButtonComponent],
  templateUrl: './payment-detail.component.html',
  styleUrls: ['./payment-detail.component.css']
})
export class PaymentDetailComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private paymentsService = inject(PaymentsService);
  private cardsService = inject(CardsService);
  private billingService = inject(BillingService);
  
  private destroy$ = new Subject<void>();

  paymentId = '';
  loading = true;
  payment: any = null;

  ngOnInit(): void {
    this.paymentId = this.route.snapshot.paramMap.get('id') || '';
    if (!this.paymentId) {
      this.goBack();
      return;
    }
    this.loadPaymentDetails();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadPaymentDetails(): void {
    this.paymentsService.getPaymentById(this.paymentId).pipe(
      takeUntil(this.destroy$),
      switchMap(paymentData => {
        // Fetch card and bill details in parallel if possible, catch errors to not break UI
        return forkJoin({
          payment: of(paymentData),
          card: this.cardsService.getCardById(paymentData.cardId).pipe(catchError(() => of(null))),
          bill: this.billingService.getBillById(paymentData.billId).pipe(catchError(() => of(null)))
        });
      })
    ).subscribe({
      next: ({ payment, card, bill }) => {
        // Resolve method name
        const methodMap: Record<string, string> = {
          '0': 'UPI', '1': 'Bank Account', '2': 'Debit Card', '4': 'Credit Card',
          'UPI': 'UPI', 'BankTransfer': 'Bank Account', 'DebitCard': 'Debit Card', 'CreditCard': 'Credit Card'
        };
        const methodStr = methodMap[payment.paymentMethod] || payment.paymentMethod || 'Wallet';

        // Resolve status string
        const statusMap: Record<number, string> = {
          0: 'Pending', 1: 'Completed', 2: 'Failed', 3: 'Refunded'
        };
        const statusStr = typeof payment.status === 'number' 
          ? statusMap[payment.status] || 'Unknown'
          : payment.status;

        // Construct paidFrom string
        let paidFrom = methodStr;
        let cardLast4 = 'XXXX';
        
        if (card) {
          cardLast4 = card.maskedNumber.slice(-4);
          if (methodStr === 'Credit Card') {
             paidFrom = `${card.issuerName || 'Credit Card'} ending in ${cardLast4}`;
          }
        }

        // Construct bill details
        let billMonth = 'Current';
        let description = 'Payment Processed';
        
        if (bill) {
          const d = new Date(bill.billingMonth + '-01');
          billMonth = d.toLocaleString('en-US', { month: 'long', year: 'numeric' });
          description = `Payment towards ${bill.cardId === payment.cardId && card ? card.issuerName : 'Credit Card'} Bill`;
        } else {
          // If bill doesn't exist, it might be a general category payment
          description = 'Payment towards selected category';
        }

        this.payment = {
          id: payment.id,
          date: payment.createdAt,
          amount: payment.amount,
          status: statusStr,
          method: methodStr,
          referenceNumber: payment.transactionReference || `REF-${payment.id.substring(0, 8).toUpperCase()}`,
          description,
          cardLast4,
          billMonth,
          paidFrom
        };
        this.loading = false;
      },
      error: () => {
        // Fallback or navigate away
        this.loading = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/payments']);
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      Completed: 'status--completed',
      Pending: 'status--pending',
      Failed: 'status--failed',
    };
    return map[status] || '';
  }

  getMethodIcon(method: string): string {
    const icons: Record<string, string> = {
      'Bank Account': 'landmark',
      'Debit Card': 'credit-card',
      'Credit Card': 'credit-card',
      'UPI': 'smartphone',
    };
    return icons[method] || 'wallet';
  }

  downloadReceipt(): void {
    window.print();
  }
}
