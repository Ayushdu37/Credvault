// Mirrors: CredVault.Shared.Contracts.Common.ApiResponse<T>
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T | null;
  errors: string[];
}

// Mirrors: CredVault.Shared.Contracts.Common.PaginatedResult<T>
export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}