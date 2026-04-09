// Mirrors: CredVault.Shared.Contracts.Card.Responses.CardResponse
// Mirrors: CredVault.Shared.Contracts.Card.Responses.CardSummaryResponse
import { CardIssuer } from './enums.model';
export { CardIssuer };

export interface CardResponse {
  id: string;
  maskedNumber: string;
  cardHolderName: string;
  issuer: CardIssuer;
  issuerName: string;
  nickname: string | null;
  expiryMonth: number;
  expiryYear: number;
  creditLimit: number;
  outstandingBalance: number;
  availableCredit: number;
  billingCycleStartDay: number;
  isDefault: boolean;
  isVerified: boolean;
  addedAt: string;
}

export interface CardSummaryResponse {
  totalCards: number;
  totalCreditLimit: number;
  totalOutstandingBalance: number;
  totalAvailableCredit: number;
  utilizationPercentage: number;
}

// Frontend-friendly card shape (what templates expect)
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

// Request models
export interface AddCardRequest {
  cardNumber: string;
  cardHolderName: string;
  expiryMonth: number;
  expiryYear: number;
  issuer: CardIssuer;
  creditLimit: number;
  billingCycleStartDay: number;
  nickname: string;
}

export interface UpdateCardLimitRequest {
  newLimit: number;
}

// Mapper: backend CardResponse → frontend CreditCard
export function mapCardResponseToCreditCard(card: CardResponse): CreditCard {
  const issuerName = card.issuerName || CardIssuer[card.issuer] || 'Visa';
  return {
    id: card.id,
    cardholderName: card.cardHolderName,
    last4Digits: card.maskedNumber.slice(-4),
    network: issuerName as CardNetwork,
    expiryMonth: card.expiryMonth,
    expiryYear: card.expiryYear,
    creditLimit: card.creditLimit,
    currentBalance: card.outstandingBalance,
    status: 'Active',
    isVirtual: false,
    isDefault: card.isDefault,
    isVerified: card.isVerified,
  };
}
