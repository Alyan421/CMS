import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ImageService {
  private baseUrl = '/api/Image';

  constructor(private http: HttpClient) { }

  getAllImages(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}`);
  }

  getImagesByColorId(colorId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/color/${colorId}`);
  }

  uploadImage(formData: FormData): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/upload/`, formData);
  }

  deleteImage(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  filterByClothName(clothName: string): Observable<any[]> {
  return this.http.get<any[]>(`${this.baseUrl}/filter-by-cloth-name/${clothName}`);
}

filterByColorName(colorName: string): Observable<any[]> {
  return this.http.get<any[]>(`${this.baseUrl}/filter-by-color/${colorName}`);
}
}
