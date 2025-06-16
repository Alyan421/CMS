import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';


@Injectable({
  providedIn: 'root',
})
export class ColorService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getColorById(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Color/${id}`);
  }

  getAllColors(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Color`);
  }

  createColor(color: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Color`, color);
  }

  updateColor(color: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/Color`, color);
  }

  deleteColor(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/Color/${id}`);
  }
}
