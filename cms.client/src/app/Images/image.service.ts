import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';


@Injectable({
  providedIn: 'root',
})
export class ImageService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getAllImages(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Image`);
  }

  getImagesByColorId(colorId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Image/by-color/${colorId}`);
  }

  uploadImage(formData: FormData): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Image/upload/`, formData);
  }

  deleteImage(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/Image/${id}`);
  }

  filterByColorName(colorName: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Image/filter-by-color/${colorName}`);
  }

  filterByClothName(clothName: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Image/filter-by-cloth-name/${clothName}`);
  }
}
