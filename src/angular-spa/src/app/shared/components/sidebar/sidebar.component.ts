import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { LucideAngularModule, LayoutDashboard, CreditCard, Receipt, ArrowRightLeft, Wallet, Bell, ChevronLeft, Star } from 'lucide-angular';
import { ThemeService } from '../../../core/services/theme.service';

interface NavItem {
  label: string;
  route: string;
  icon: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    RouterLink, 
    RouterLinkActive, 
    LucideAngularModule,
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css',
})
export class SidebarComponent {
  @Input() collapsed = false;
  @Output() toggle = new EventEmitter<void>();

  // Add theme service
  public themeService = inject(ThemeService);

  navItems: NavItem[] = [
    { label: 'Dashboard',        route: '/dashboard',          icon: 'layout-dashboard' },
    { label: 'Cards',            route: '/cards',              icon: 'credit-card' },
    { label: 'Billing',          route: '/billing',            icon: 'receipt' },
    { label: 'Payments',         route: '/payments',           icon: 'arrow-right-left' },
    { label: 'Rewards',          route: '/rewards',            icon: 'star' },
    { label: 'Payment Methods',  route: '/payment-methods',    icon: 'wallet' },
    { label: 'Notifications',    route: '/notifications',      icon: 'bell' },
  ];

  /* Icons registered via app.config.ts or global module picker to keep component clean */
}
