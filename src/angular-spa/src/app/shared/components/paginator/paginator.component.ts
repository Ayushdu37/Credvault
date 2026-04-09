import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-paginator',
  standalone: true,
  template: `
    <div class="paginator" id="paginator">
      <span class="paginator__info">
        Showing {{ startItem }}–{{ endItem }} of {{ totalCount }}
      </span>

      <div class="paginator__controls">
        <button
          class="paginator__btn"
          [disabled]="currentPage <= 1"
          (click)="goToPage(currentPage - 1)"
          aria-label="Previous page">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor"
               stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <polyline points="15 18 9 12 15 6"/>
          </svg>
        </button>

        @for (page of visiblePages; track page) {
          @if (page === -1) {
            <span class="paginator__ellipsis">…</span>
          } @else {
            <button
              class="paginator__btn paginator__btn--page"
              [class.paginator__btn--active]="page === currentPage"
              (click)="goToPage(page)">
              {{ page }}
            </button>
          }
        }

        <button
          class="paginator__btn"
          [disabled]="currentPage >= totalPages"
          (click)="goToPage(currentPage + 1)"
          aria-label="Next page">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor"
               stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <polyline points="9 18 15 12 9 6"/>
          </svg>
        </button>
      </div>
    </div>
  `,
  styleUrl: './paginator.component.css',
})
export class PaginatorComponent {
  @Input() totalCount = 0;
  @Input() pageSize = 10;
  @Input() currentPage = 1;
  @Output() pageChange = new EventEmitter<number>();

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount / this.pageSize));
  }

  get startItem(): number {
    return Math.min((this.currentPage - 1) * this.pageSize + 1, this.totalCount);
  }

  get endItem(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalCount);
  }

  get visiblePages(): number[] {
    const total = this.totalPages;
    const current = this.currentPage;
    const pages: number[] = [];

    if (total <= 7) {
      for (let i = 1; i <= total; i++) pages.push(i);
      return pages;
    }

    pages.push(1);

    if (current > 3) pages.push(-1); // ellipsis

    const start = Math.max(2, current - 1);
    const end = Math.min(total - 1, current + 1);

    for (let i = start; i <= end; i++) pages.push(i);

    if (current < total - 2) pages.push(-1); // ellipsis

    pages.push(total);

    return pages;
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.currentPage) return;
    this.pageChange.emit(page);
  }
}
