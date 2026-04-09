import { Injectable } from '@angular/core';
import { Observable, of, delay } from 'rxjs';
import { map } from 'rxjs/operators';

export type CardStatus = 'Active' | 'Locked' | 'Blocked' | 'Expired';
export type CardNetwork = 'Visa' | 'Mastercard' | 'Amex' | 'Discover';

export interface CreditCard {
  id: string;
  cardholderName: string;
  last4Digits: string;
  network: CardNetwork;
  expiryMonth: number;
  expiryYear: number;
  creditLimit: number;
  currentBalance: number;
  status: CardStatus;
  isVirtual: boolean;
  isDefault: boolean;
  isVerified: boolean;
}

export interface RequestCardPayload {
  cardType: 'Physical' | 'Virtual';
  deliveryAddress?: string;
}

export interface AddCardPayload {
  cardNumber: string;
  cardHolderName: string;
  expiryMonth: number;
  expiryYear: number;
  issuer: string;
  creditLimit: number;
  billingCycleStartDay: number;
  nickname: string;
}

@Injectable({ providedIn: 'root' })
export class CardsService {

  getCards(): Observable<CreditCard[]> {
    const mockCards: CreditCard[] = [
      {
        id: 'card-001',
        cardholderName: 'Rajan Mehta',
        last4Digits: '4242',
        network: 'Visa',
        expiryMonth: 12,
        expiryYear: 2027,
        creditLimit: 15000,
        currentBalance: 3250.75,
        status: 'Active',
        isVirtual: false,
        isDefault: true,
        isVerified: true,
      },
      {
        id: 'card-002',
        cardholderName: 'Rajan Mehta',
        last4Digits: '8888',
        network: 'Mastercard',
        expiryMonth: 6,
        expiryYear: 2026,
        creditLimit: 10000,
        currentBalance: 1000,
        status: 'Locked',
        isVirtual: false,
        isDefault: false,
        isVerified: true,
      },
      {
        id: 'card-003',
        cardholderName: 'Rajan Mehta',
        last4Digits: '0099',
        network: 'Visa',
        expiryMonth: 3,
        expiryYear: 2028,
        creditLimit: 5000,
        currentBalance: 0,
        status: 'Active',
        isVirtual: true,
        isDefault: false,
        isVerified: false,
      },
    ];
    return of(mockCards).pipe(delay(500));
  }

  getCardById(id: string): Observable<CreditCard | undefined> {
    return this.getCards().pipe(
      map(cards => cards.find(c => c.id === id)),
      delay(200),
    );
  }

  lockCard(id: string): Observable<{ success: boolean }> {
    return of({ success: true }).pipe(delay(400));
  }

  unlockCard(id: string): Observable<{ success: boolean }> {
    return of({ success: true }).pipe(delay(400));
  }

  setDefault(id: string): Observable<{ success: boolean }> {
    return of({ success: true }).pipe(delay(400));
  }

  verifyCard(id: string): Observable<{ success: boolean }> {
    return of({ success: true }).pipe(delay(400));
  }

  deleteCard(id: string): Observable<{ success: boolean }> {
    return of({ success: true }).pipe(delay(500));
  }

  addCard(payload: AddCardPayload): Observable<{ success: boolean; message: string }> {
    return of({ success: true, message: 'Card added successfully.' }).pipe(delay(600));
  }

  requestNewCard(payload: RequestCardPayload): Observable<{ success: boolean; message: string }> {
    return of({ success: true, message: 'Your card request has been submitted.' }).pipe(delay(600));
  }
}
