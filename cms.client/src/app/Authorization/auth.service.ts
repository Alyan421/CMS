import { Injectable } from '@angular/core';
import { jwtDecode} from 'jwt-decode';
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private tokenKey = 'authToken';

  // Save the token in localStorage
  saveToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  // Retrieve the token
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  // Remove the token on logout
  logout(): void {
    localStorage.removeItem(this.tokenKey);
  }

  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) return true;

    const decoded: any = jwtDecode(token);
    const currentTime = Math.floor(Date.now() / 1000);
    return decoded.exp < currentTime;
  }
}
