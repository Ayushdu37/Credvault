export interface PaymentCategory {
  id: string;
  label: string;
  icon: string;
  color: string;
  bgColor: string;
  borderColor: string;
  description: string;
  /** Whether this category uses a card dropdown instead of a text input */
  usesCardDropdown: boolean;
  /** Input field config — only used if usesCardDropdown is false */
  placeholder: string;
  inputLabel: string;
  inputType: 'text' | 'number' | 'tel' | 'email';
  /** Extra dropdown fields (e.g. Operator for Mobile Recharge) */
  extraDropdown?: {
    label: string;
    options: string[];
  };
}

export const PAYMENT_CATEGORIES: PaymentCategory[] = [
  {
    id: 'upi',
    label: 'UPI Transfer',
    icon: 'send',
    color: '#10B981',
    bgColor: 'rgba(16, 185, 129, 0.08)',
    borderColor: 'rgba(16, 185, 129, 0.3)',
    description: 'Send money to any UPI ID',
    usesCardDropdown: false,
    placeholder: 'name@upi',
    inputLabel: 'Recipient UPI ID',
    inputType: 'text',
  },
  {
    id: 'mobile',
    label: 'Mobile Recharge',
    icon: 'smartphone',
    color: '#3B82F6',
    bgColor: 'rgba(59, 130, 246, 0.08)',
    borderColor: 'rgba(59, 130, 246, 0.3)',
    description: 'Recharge prepaid mobile',
    usesCardDropdown: false,
    placeholder: '9876543210',
    inputLabel: 'Mobile Number',
    inputType: 'tel',
    extraDropdown: {
      label: 'Operator',
      options: ['Jio', 'Airtel', 'Vi (Vodafone Idea)', 'BSNL'],
    },
  },
  {
    id: 'credit',
    label: 'Credit Card',
    icon: 'credit-card',
    color: '#dc2626',
    bgColor: 'rgba(220, 38, 38, 0.08)',
    borderColor: 'rgba(220, 38, 38, 0.3)',
    description: 'Pay credit card bill',
    usesCardDropdown: true,
    placeholder: '',
    inputLabel: 'Select Card',
    inputType: 'text',
  },
  {
    id: 'electricity',
    label: 'Electricity',
    icon: 'zap',
    color: '#F59E0B',
    bgColor: 'rgba(245, 158, 11, 0.08)',
    borderColor: 'rgba(245, 158, 11, 0.3)',
    description: 'Pay electricity bill',
    usesCardDropdown: false,
    placeholder: 'e.g. 1234567890',
    inputLabel: 'Consumer Number',
    inputType: 'text',
  },
  {
    id: 'water',
    label: 'Water Bill',
    icon: 'droplet',
    color: '#06B6D4',
    bgColor: 'rgba(6, 182, 212, 0.08)',
    borderColor: 'rgba(6, 182, 212, 0.3)',
    description: 'Pay water bill',
    usesCardDropdown: false,
    placeholder: 'e.g. WTR-0012345',
    inputLabel: 'Connection ID',
    inputType: 'text',
  },
  {
    id: 'dth',
    label: 'DTH Recharge',
    icon: 'tv',
    color: '#8B5CF6',
    bgColor: 'rgba(139, 92, 246, 0.08)',
    borderColor: 'rgba(139, 92, 246, 0.3)',
    description: 'Recharge DTH connection',
    usesCardDropdown: false,
    placeholder: 'e.g. 30012345678',
    inputLabel: 'Subscriber ID',
    inputType: 'text',
  },
  {
    id: 'rent',
    label: 'Rent Payment',
    icon: 'home',
    color: '#EC4899',
    bgColor: 'rgba(236, 72, 153, 0.08)',
    borderColor: 'rgba(236, 72, 153, 0.3)',
    description: 'Pay monthly rent',
    usesCardDropdown: false,
    placeholder: 'landlord@upi or A/C number',
    inputLabel: 'Landlord UPI / Account Number',
    inputType: 'text',
  },
];

export const QUICK_AMOUNTS = [100, 500, 1000, 2000, 5000];
