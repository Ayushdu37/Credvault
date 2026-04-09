import { Injectable, signal, computed } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TokenService {
    
    // Tokens live ONLY in memory — cleared on tab close
    private accessToken = signal<string | null>(null);
    private refreshTokenValue = signal<string | null>(null);
    private expiresAt = signal<Date | null>(null);

    // Read-only signals for consumers
    readonly token = this.accessToken.asReadonly();
    readonly refresh = this.refreshTokenValue.asReadonly();
    readonly isAuthenticated = computed(() => !!this.accessToken());

    setTokens(access: string, refresh: string, expiresAt: string): void {
    this.accessToken.set(access);
    this.refreshTokenValue.set(refresh);
    this.expiresAt.set(new Date(expiresAt));
    }

    updateAccessToken(access: string): void {
    this.accessToken.set(access);
    }

    clearTokens(): void {
    this.accessToken.set(null);
    this.refreshTokenValue.set(null);
    this.expiresAt.set(null);
    }

    isTokenExpired(): boolean {
    const exp = this.expiresAt();
    if (!exp) return true;
    return new Date() >= exp;
    }
}