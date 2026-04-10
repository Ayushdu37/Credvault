import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe, DatePipe, NgClass, NgFor, NgIf, PercentPipe } from '@angular/common';
import { DashboardActions } from '../../store/dashboard/dashboard.actions';
import { selectDashboardSummary, selectDashboardLoading, selectDashboardError } from '../../store/dashboard/dashboard.selectors';
import { EChartsOption } from 'echarts';
import * as echarts from 'echarts';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import { CardComponent } from '../../shared/components/card/card.component';
import { ButtonComponent } from '../../shared/components/button/button.component';
import { LucideAngularModule } from 'lucide-angular';
import { SpinnerComponent } from '../../shared/components/spinner/spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state/empty-state.component';
import { ThemeService } from '../../core/services/theme.service';

@Component({
  standalone: true,
  selector: 'app-dashboard',
  imports: [
    AsyncPipe,
    CurrencyPipe,
    DatePipe,
    PercentPipe,
    NgClass,
    NgFor,
    NgIf,
    NgxEchartsDirective,
    CardComponent,
    ButtonComponent,
    LucideAngularModule,
    SpinnerComponent,
    EmptyStateComponent
  ],
  providers: [
    provideEchartsCore({ echarts }),
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  private store = inject(Store);
  private router = inject(Router);
  private themeService = inject(ThemeService);

  summary$ = this.store.select(selectDashboardSummary);
  loading$ = this.store.select(selectDashboardLoading);
  error$ = this.store.select(selectDashboardError);

  // Chart Options State
  utilizationChartOptions: EChartsOption = {};
  balanceChartOptions: EChartsOption = {};

  ngOnInit(): void {
    this.store.dispatch(DashboardActions.loadSummary());

    this.summary$.subscribe(summary => {
      if (summary) {
        const months = ['Oct', 'Nov', 'Dec', 'Jan', 'Feb', 'Mar'];
        const utilData = summary.recentBills.map(b =>
          b.totalAmount > 0 ? Math.round((b.amountPaid / b.totalAmount) * 100) : 0
        );
        const balanceData = summary.recentBills.map(b => b.remaining);
        this.initCharts(months, utilData, balanceData);
      }
    });
  }

  /** Read a CSS variable from the document root */
  private cssVar(name: string): string {
    return getComputedStyle(document.documentElement).getPropertyValue(name).trim();
  }

  private initCharts(months: string[], utilData: number[], balanceData: number[]): void {
    const textMuted = this.cssVar('--text-muted');
    const borderColor = this.cssVar('--border-light');
    const bgCard = this.cssVar('--bg-card');
    const textPrimary = this.cssVar('--text-primary');
    const accent = this.cssVar('--accent');

    // Shared grid line style — very faint
    const gridLineStyle = { color: borderColor, type: 'dashed' as const, opacity: 0.6 };

    // 1. Line Chart for Utilization (%)
    this.utilizationChartOptions = {
      tooltip: {
        trigger: 'axis',
        backgroundColor: bgCard,
        borderColor: borderColor,
        textStyle: { color: textPrimary, fontSize: 13 },
        extraCssText: 'border-radius: 8px; box-shadow: 0 4px 14px rgba(0,0,0,0.1);'
      },
      grid: { left: '3%', right: '4%', bottom: '3%', containLabel: true },
      xAxis: {
        type: 'category',
        boundaryGap: false,
        data: months,
        axisLabel: { color: textMuted, fontSize: 12 },
        axisLine: { lineStyle: { color: borderColor } },
        axisTick: { show: false }
      },
      yAxis: {
        type: 'value',
        axisLabel: { color: textMuted, fontSize: 12, formatter: '{value}%' },
        splitLine: { lineStyle: gridLineStyle },
        axisLine: { show: false },
        axisTick: { show: false }
      },
      series: [
        {
          name: 'Utilization',
          type: 'line',
          smooth: true,
          data: utilData,
          lineStyle: { width: 3 },
          symbolSize: 6,
          itemStyle: { color: accent },
          areaStyle: {
            color: {
              type: 'linear',
              x: 0, y: 0, x2: 0, y2: 1,
              colorStops: [
                { offset: 0, color: 'rgba(220, 38, 38, 0.18)' },
                { offset: 1, color: 'rgba(220, 38, 38, 0.0)' }
              ]
            }
          }
        }
      ]
    };

    // 2. Bar Chart for Balance
    this.balanceChartOptions = {
      tooltip: {
        trigger: 'axis',
        axisPointer: { type: 'shadow' },
        backgroundColor: bgCard,
        borderColor: borderColor,
        textStyle: { color: textPrimary, fontSize: 13 },
        extraCssText: 'border-radius: 8px; box-shadow: 0 4px 14px rgba(0,0,0,0.1);'
      },
      grid: { left: '3%', right: '4%', bottom: '3%', containLabel: true },
      xAxis: {
        type: 'category',
        data: months,
        axisLabel: { color: textMuted, fontSize: 12 },
        axisLine: { lineStyle: { color: borderColor } },
        axisTick: { show: false }
      },
      yAxis: {
        type: 'value',
        axisLabel: { color: textMuted, fontSize: 12 },
        splitLine: { lineStyle: gridLineStyle },
        axisLine: { show: false },
        axisTick: { show: false }
      },
      series: [
        {
          name: 'Balance',
          type: 'bar',
          barWidth: '55%',
          data: balanceData,
          itemStyle: { color: '#3B82F6', borderRadius: [4, 4, 0, 0] }
        }
      ]
    };
  }

  navigateToStatements(): void {
    this.router.navigate(['/billing']);
  }

  navigateToMakePayment(): void {
    this.router.navigate(['/payments/pay']);
  }
}