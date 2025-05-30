import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ColorService {
  private baseUrl = '/api/Color';

  constructor(private http: HttpClient) { }

  getAllColors(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}`);
  }

  createColor(color: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}`, color);
  }

  updateColor(id: number, color: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/${id}`, color);
  }

  deleteColor(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
