import { BehavioralSkill } from './../_models/behavioralSkill';
import { map, filter } from 'rxjs/operators';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';

import { PaginatedResult } from './../_models/pagination';
import { environment } from '../../environments/environment';
import { Strategy } from '../_models/strategy';
import { Axis } from '../_models/axis';
import { AxisPole } from '../_models/axisPole';
import { User } from '../_models/user';
import { EvaluationFile } from '../_models/evaluationFile';
import { EvaluationFileInstance } from '../_models/evaluationFileInstance';
import { EvaluationFileInstanceLog } from '../_models/evaluationFileInstanceLog';
import { Parameters } from '../_models/parameters';
import { ReportSheet } from '../_models/reportSheet';

@Injectable({
  providedIn: 'root'
})
export class HrService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getStrategy(id: number, ownerId) {
    let params = new HttpParams();
    params = params.append('ownerId', ownerId);
    return this.http.get<Strategy>(`${this.baseUrl}hr/strategy/${id}`, { params });
  }

  getStrategies(
    id,
    page?,
    itemsPerPage?,
    strategyParams?
  ): Observable<PaginatedResult<Strategy[]>> {
    const paginatedResult: PaginatedResult<Strategy[]> = new PaginatedResult<
      Strategy[]
      >();
    let params = new HttpParams();
    params = params.append('ownerId', id);
    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (strategyParams != null) {
      params = params.append('status', strategyParams.status);
      params = params.append('orderBy', strategyParams.orderBy);
    }

    return this.http
      .get<Strategy[]>(this.baseUrl + 'hr/strategy', { observe: 'response', params })
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

  getPublishedStrategies() {
    return this.http.get<Strategy[]>(`${this.baseUrl}hr/strategy/publishedStrategies`);
  }

  getPublishedBehavioralSkills() {
    return this.http.get<BehavioralSkill[]>(`${this.baseUrl}hr/behavioralSkill/publishedBehavioralSkills`);
  }

  createStrategy(ownerId: number, strategy: Strategy) {
    return this.http.post(`${this.baseUrl}hr/strategy/new/${ownerId}`, strategy);
  }

  cloneStrategy(ownerId: number, strategyId: number) {
    return this.http.post(`${this.baseUrl}hr/strategy/clone/${ownerId}/${strategyId}`, {});
  }

  deleteStrategy(id: number) {
    return this.http.delete(`${this.baseUrl}hr/strategy/delete/${id}`);
  }

  deleteStrategyDocument(id: number) {
    return this.http.post(this.baseUrl + 'hr/strategy/documentation/delete/' + id, {});
  }

  updateStrategy(ownerId: number, strategy: Strategy) {
    return this.http.put(`${this.baseUrl}hr/strategy/edit/${ownerId}`, strategy);
  }

  getAxisList(strategyId: number) {
    return this.http.get<Axis[]>(
      this.baseUrl + 'hr/axis/axisList/' + strategyId);
  }

  getAxisPoleList(axisId: number) {
    return this.http.get<AxisPole[]>(
      this.baseUrl + 'hr/axisPole/axisPoleList/' + axisId);
  }

  addAxis(axis: Axis) {
    return this.http.post(this.baseUrl + 'hr/axis/addAxis', axis);
  }

  updateAxis(id: number, axis: Axis) {
    return this.http.put(this.baseUrl + 'hr/axis/updateAxis/' + id, axis);
  }

  deleteAxis(id: number, userId: number) {
    return this.http.delete(`${this.baseUrl}hr/axis/${id}/delete/${userId}`);
  }

  updateAxisPoleWeigth(axisId: number, poleId: number, weight: number) {
    return this.http.put(`${this.baseUrl}hr/axisPole/updateAxisPole/${axisId}/${poleId}/${weight}`, {});
  }

  getBehavioralSkills(createdById, filters?) {
    let params = new HttpParams();
    params = params.append('ownerId', createdById);
    if (filters != null) {
      params = params.append('status', filters.status);
      params = params.append('orderBy', filters.orderBy);
    }
    return this.http.get<BehavioralSkill[]>(`${this.baseUrl}hr/behavioralSkill`, { params });
  }

  cloneBehavioralSkill(ownerId: number, behavioralSkillId: number) {
    return this.http.post(`${this.baseUrl}hr/behavioralSkill/clone/${ownerId}/${behavioralSkillId}`, {});
  }

  deleteBehavioralSkill(id: number) {
    return this.http.delete(`${this.baseUrl}hr/behavioralSkill/delete/${id}`);
  }

  updateBehavioralSkill(createdById: number, behavioralSkill: BehavioralSkill) {
    return this.http.put(`${this.baseUrl}hr/behavioralSkill/edit/${createdById}`, behavioralSkill)
  }

  getBehavioralSkill(id: number) {
    return this.http.get<BehavioralSkill>(`${this.baseUrl}hr/behavioralSkill/${id}`);
  }

  createBehavioralSkill(createdById: number, behavioralSkill: BehavioralSkill) {
    return this.http.post(`${this.baseUrl}hr/behavioralSkill/new/${createdById}`, behavioralSkill);
  }

  getEvaluationFiles(ownerId, filters?) {
    let params = new HttpParams();
    params = params.append('ownerId', ownerId);
    if (filters != null) {
      params = params.append('status', filters.status);
      params = params.append('orderBy', filters.orderBy);
    }
    return this.http.get<EvaluationFile[]>(`${this.baseUrl}hr/evaluationModel`, { params });
  }

  getEvaluationSheetLogs(sheetName?) {
    let params = new HttpParams();
    if (sheetName != null) {
      params = params.append('sheetName', sheetName);
    }
    return this.http.get<EvaluationFileInstanceLog[]>(`${this.baseUrl}hr/evaluationSheet/logs`, { params });
  }

  updateEvaluationFile(ownerId: number, evaluationFile: any) {
    return this.http.put(`${this.baseUrl}hr/evaluationModel/edit/${ownerId}`, evaluationFile)
  }

  getEvaluationFile(id: number) {
    return this.http.get<EvaluationFile>(`${this.baseUrl}hr/evaluationModel/${id}`);
  }

  createEvaluationFile(ownerId: number, newEvaluationFile: any) {
    return this.http.post(`${this.baseUrl}hr/evaluationModel/new/${ownerId}`, newEvaluationFile);
  }

  deleteEvaluationFile(id: number, userId: number) {
    return this.http.delete(`${this.baseUrl}hr/evaluationModel/${id}/delete/${userId}`);
  }

  generateEvaluationFile(evaluationFileId: number, users: User[]) {
    return this.http.post(`${this.baseUrl}hr/evaluationSheet/generate/${evaluationFileId}`, users);
  }

  getEvaluationFileInstancesByEvaluationFileId(evaluationFileId: number) {
    return this.http.get<EvaluationFileInstance[]>(`${this.baseUrl}hr/evaluationSheet/${evaluationFileId}`);
  }

  loadParameters(evaluationFileId: number) {
    return this.http.get<Parameters[]>(`${this.baseUrl}hr/evaluationModel/loadParameters/${evaluationFileId}`);
  }

  addParameters(newParameters: Parameters, userId: number) {
    return this.http.post(`${this.baseUrl}hr/evaluationModel/addParameters/${userId}`, newParameters);
  }

  deleteParameters(id: number, userId: number) {
    return this.http.delete(`${this.baseUrl}hr/evaluationModel/${id}/deleteParameters/${userId}`);
  }

  deleteEvaluationFileInstance(id: number, userId: number) {
    return this.http.delete(`${this.baseUrl}hr/evaluationSheet/${id}/delete/${userId}`);
  }

  getReportSheets(
    page?,
    itemsPerPage?,
    filters?
  ): Observable<PaginatedResult<ReportSheet[]>> {
    const paginatedResult: PaginatedResult<ReportSheet[]> = new PaginatedResult<
    ReportSheet[]
      >();
    let params = new HttpParams();
    let pageSize = 0;
    if (filters != null) {
      params = params.append('year', filters.year);
      params = params.append('userToSearch', filters.userToSearch);
      params = params.append('poleId', filters.poleId);
      pageSize = filters.pageSize;
    }

    if (page != null && itemsPerPage != null) {
      if(pageSize != 0) itemsPerPage = pageSize;
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    return this.http
      .get<ReportSheet[]>(this.baseUrl + 'hr/EvaluationSheet', { observe: 'response', params })
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
}
