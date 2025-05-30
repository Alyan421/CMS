import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ClothService {
  private baseUrl = '/api/Cloth';

  constructor(private http: HttpClient) { }

  getAllCloths(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}`);
  }

  createCloth(cloth: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}`, cloth);
  }

  updateCloth(id: number, cloth: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/${id}`, cloth);
  }

  deleteCloth(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
