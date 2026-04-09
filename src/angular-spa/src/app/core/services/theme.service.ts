import { Injectable, signal } from '@angular/core';

export type Theme = 'light' | 'dark';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly THEME_KEY = 'credvault-theme';
  
  // Using an Angular Signal for reactive theme state
  public currentTheme = signal<Theme>('light');

  constructor() {
    this.initializeTheme();
  }

  private initializeTheme() {
    const savedTheme = localStorage.getItem(this.THEME_KEY) as Theme;
    // Default to light theme if none saved
    const theme = savedTheme === 'light' || savedTheme === 'dark' ? savedTheme : 'light';
    
    this.currentTheme.set(theme);
    this.applyTheme(theme);
  }

  public toggleTheme() {
    const newTheme = this.currentTheme() === 'dark' ? 'light' : 'dark';
    this.currentTheme.set(newTheme);
    localStorage.setItem(this.THEME_KEY, newTheme);
    this.applyTheme(newTheme);
  }

  public setLight() {
    if (this.currentTheme() !== 'light') {
      this.currentTheme.set('light');
      localStorage.setItem(this.THEME_KEY, 'light');
      this.applyTheme('light');
    }
  }

  public setDark() {
    if (this.currentTheme() !== 'dark') {
      this.currentTheme.set('dark');
      localStorage.setItem(this.THEME_KEY, 'dark');
      this.applyTheme('dark');
    }
  }

  private applyTheme(theme: Theme) {
    if (theme === 'dark') {
      document.documentElement.setAttribute('data-theme', 'dark');
    } else {
      document.documentElement.removeAttribute('data-theme');
    }
  }
}
