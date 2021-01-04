import { map } from 'rxjs/operators';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';

import { PaginatedResult } from './../_models/pagination';
import { environment } from '../../environments/environment';
import { User } from '../_models/user';
import { Evaluator } from '../_models/evaluator';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getUsers(
    page?,
    itemsPerPage?,
    userParams?
  ): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<
      User[]
      >();
    let params = new HttpParams();
    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (userParams != null) {
      params = params.append('departmentId', userParams.departmentId);
      params = params.append('userStatusId', userParams.userStatusId);
      params = params.append('userToSearch', userParams.userToSearch);
      params = params.append('orderBy', userParams.orderBy);
    }

    return this.http
      .get<User[]>(this.baseUrl + 'users', { observe: 'response', params })
      .pipe(
        map(response => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(
              response.headers.get('Pagination')
            );
          }
          return paginatedResult;
        })
      );
  }

  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(user) {
    return this.http.put(this.baseUrl + 'admin/', user);
  }

  createUser(notifyUser: boolean, user: User) {
    return this.http.post(this.baseUrl + 'admin/register/' + notifyUser, user);
  }

  getUsersWithRoles(page?,
    itemsPerPage?, userParams?): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
    let params = new HttpParams();
    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (userParams != null && userParams.departmentId !== undefined && userParams.userStatusId !== undefined) {
      params = params.append('departmentId', userParams.departmentId);
      params = params.append('userStatusId', userParams.userStatusId);
      params = params.append('userToSearch', userParams.userToSearch);
      params = params.append('orderBy', userParams.orderBy);
    }

    return this.http
      .get<User[]>(this.baseUrl + 'admin/usersWithRoles', { observe: 'response', params })
      .pipe(
        map(response => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(
              response.headers.get('Pagination')
            );
          }
          return paginatedResult;
        })
      );
  }

  getDepartments() {
    return this.http.get(this.baseUrl + 'admin/departments');
  }

  getPoles() {
    return this.http.get(this.baseUrl + 'admin/poles');
  }

  getUserStatus() {
    return this.http.get(this.baseUrl + 'admin/userStatus');
  }

  updateUserRoles(user: User, roles: {}) {
    return this.http.post(this.baseUrl + 'admin/editRoles/' + user.id, roles);
  }

  getPhotosForApproval() {
    return this.http.get(this.baseUrl + 'admin/photosForModeration');
  }

  approvePhoto(photoId) {
    return this.http.post(this.baseUrl + 'admin/approvePhoto/' + photoId, {});
  }

  rejectPhoto(photoId) {
    return this.http.post(this.baseUrl + 'admin/rejectPhoto/' + photoId, {});
  }

  emailAlreadyExists(email: string) {
    return this.http.get(`${this.baseUrl}admin/emailAlreadyExists?email=${email}`);
  }

  employeeNumberAlreadyExists(employeeNumber: string) {
    return this.http.get(`${this.baseUrl}admin/employeeNumberAlreadyExists?employeeNumber=${employeeNumber}`);
  }

  searchUsers(searchTerm: { userToSearch: string, userStatusId: number }): Observable<User[]> {
    let params = new HttpParams();

    params = params.append('userToSearch', searchTerm.userToSearch);
    params = params.append('userStatusId', searchTerm.userStatusId.toString());

    return this.http.get<User[]>(this.baseUrl + 'admin/searchUsers', { params });
  }

  addEvaluatorToUser(evaluatedId: number, evaluatorIds: number[]) {
    return this.http.post(`${this.baseUrl}admin/addEvaluatorToUser/${evaluatedId}`, evaluatorIds);
  }

  addEvaluateeToUser(evaluatorId: number, evaluateeIds: number[]) {
    return this.http.post(`${this.baseUrl}admin/addEvaluateeToUser/${evaluatorId}`, evaluateeIds);
  }

  updateRankOfEvaluator(evaluatedId: number, evaluatorId: number, rank: number) {
    return this.http.put(`${this.baseUrl}admin/updateRankOfEvaluator/${evaluatedId}/${evaluatorId}/${rank}`, {});
  }

  loadEvaluators(evaluatedId: number) {
    return this.http.get<Evaluator[]>(`${this.baseUrl}admin/loadEvaluators/${evaluatedId}`);
  }

  loadEvaluatees(evaluatorId: number) {
    return this.http.get<Evaluator[]>(`${this.baseUrl}admin/loadEvaluatees/${evaluatorId}`);
  }

  deleteEvaluator(evaluatorId: number, evaluatedId: number) {
    return this.http.delete(`${this.baseUrl}admin/deleteEvaluator/${evaluatorId}/${evaluatedId}`);
  }

  deleteEvaluatee(evaluatedId: number, evaluatorId: number) {
    return this.http.delete(`${this.baseUrl}admin/deleteEvaluatee/${evaluatedId}/${evaluatorId}`);
  }
}
