import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';

import { User } from '../_models/user';
import { PaginatedResult } from './../_models/pagination';
import { Message } from '../_models/message';
import { EvaluationFileInstance } from '../_models/evaluationFileInstance';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  baseUrl = environment.apiUrl;
  unreadMessages = new BehaviorSubject<number>(0);
  currentUnreadMessages = this.unreadMessages.asObservable();

  constructor(private http: HttpClient) {}

  totalUnreadMessages(userId: number) {
    this.getMessages(userId, 1, 100, 'Unread').subscribe(
      (res: PaginatedResult<Message[]>) => {
        this.unreadMessages.next(res.result.length);
      },
      (error) => {
        this.unreadMessages.next(0);
      }
    );
  }

  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  loadAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + 'users/loadAllUsers');
  }

  updateProfile(id: number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  setMainPhoto(userId: number, id: number) {
    return this.http.post(
      this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain',
      {}
    );
  }

  deletePhoto(userId: number, id: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }

  getMessages(
    id: number,
    page?,
    itemsPerPage?,
    messageContainer?
  ): Observable<PaginatedResult<Message[]>> {
    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<
      Message[]
    >();
    let params = new HttpParams();
    params = params.append('MessageContainer', messageContainer);

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    return this.http
      .get<Message[]>(this.baseUrl + 'users/' + id + '/messages', {
        observe: 'response',
        params,
      })
      .pipe(
        map((response) => {
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

  getMessageThread(id: number, recipientId: number) {
    return this.http.get<Message[]>(
      this.baseUrl + 'users/' + id + '/messages/thread/' + recipientId
    );
  }

  sendMessage(id: number, message: Message) {
    return this.http.post(this.baseUrl + 'users/' + id + '/messages', message);
  }

  deleteMessage(id: number, userId: number) {
    return this.http.post(
      this.baseUrl + 'users/' + userId + '/messages/' + id,
      {}
    );
  }

  markAsRead(userId: number, messageId: number) {
    this.http
      .post(
        this.baseUrl + 'users/' + userId + '/messages/' + messageId + '/read',
        {}
      )
      .subscribe();
  }

  getMySheets(
    userId: number,
    page?,
    itemsPerPage?,
    filters?
  ): Observable<PaginatedResult<EvaluationFileInstance[]>> {
    const paginatedResult: PaginatedResult<
      EvaluationFileInstance[]
    > = new PaginatedResult<EvaluationFileInstance[]>();
    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (filters != null) {
      params = params.append('year', filters.year);
    }

    return this.http
      .get<EvaluationFileInstance[]>(
        this.baseUrl + 'users/' + userId + '/sheet',
        {
          observe: 'response',
          params,
        }
      )
      .pipe(
        map((response) => {
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

  getMyCollaboratorsSheets(userId: number, filters?) {
    let params = new HttpParams();
    if (filters != null) {
      params = params.append('year', filters.year);
    }

    return this.http.get<EvaluationFileInstance[]>(
      this.baseUrl + 'users/' + userId + '/sheet/myCollaboratorsSheets',
      {
        params,
      }
    );
  }

  getMySheet(id: number, userId: number) {
    return this.http.get<EvaluationFileInstance>(
      this.baseUrl + 'users/' + userId + '/sheet/mysheet/' + id
    );
  }

  getGoalsForAxis(userId: number, axisInstanceIds: number[]) {
    return this.http.post(
      `${this.baseUrl}users/${userId}/goal`,
      axisInstanceIds
    );
  }

  getGoalDetail(userId: number, goalId) {
    return this.http.get(
      `${this.baseUrl}users/${userId}/goal/goalWithChildren/${goalId}`
    );
  }

  createGoal(userId: number, sheetId: number, goal: any) {
    return this.http.post(
      `${this.baseUrl}users/${userId}/goal/createGoal/${sheetId}`,
      goal
    );
  }

  cascadeGoal(
    userId: number,
    golasForCascade: any,
    sheetId: number,
    modelId: number
  ) {
    return this.http.post(
      `${this.baseUrl}users/${userId}/goal/${modelId}/cascadeGoal/${sheetId}`,
      golasForCascade
    );
  }

  updateGoal(id: number, userId: number, goal: any) {
    return this.http.put(
      `${this.baseUrl}users/${userId}/goal/editGoal/${id}`,
      goal
    );
  }

  deleteGoal(id: number, userId: number) {
    return this.http.delete(
      `${this.baseUrl}users/${userId}/goal/deleteGoal/${id}`
    );
  }

  getGoalTypes(userId) {
    return this.http.get(`${this.baseUrl}users/${userId}/goal/goalTypes`);
  }

  getProjects(userId) {
    return this.http.get(`${this.baseUrl}users/${userId}/goal/projects`);
  }

  updateAxisInstance(
    userId: number,
    axisInstanceId: number,
    userWeight: number
  ) {
    return this.http.patch(
      `${this.baseUrl}users/${userId}/sheet/${axisInstanceId}/${userWeight}`,
      {}
    );
  }

  validateGoals(userId: number, sheetId: number, goals: any[]) {
    return this.http.put(
      `${this.baseUrl}users/${userId}/goal/validateGoals/${sheetId}`,
      goals
    );
  }

  getGoalEvaluations(userId: number, goalId: number) {
    return this.http.get(
      `${this.baseUrl}users/${userId}/goalEvaluation/goalEvaluations/${goalId}`
    );
  }

  addGoalEvaluations(userId: number, goalEval: any) {
    return this.http.post(
      `${this.baseUrl}users/${userId}/goalEvaluation/createGoalEvaluation`,
      goalEval
    );
  }

  getBehavioralSkillInstances(userId: number, sheetId: number) {
    return this.http.get(
      `${this.baseUrl}users/${userId}/behavioralSkillEvaluation/behavioralSkillInstancesForSheet/${sheetId}`
    );
  }

  addBehavioralSkillEvaluations(userId: number, evals: any[]) {
    return this.http.post(
      `${this.baseUrl}users/${userId}/behavioralSkillEvaluation/createBehavioralSkillEvaluation`,
      evals
    );
  }

  addFinalEvaluation(userId: number, sheetId: number, finalEvaluation: any) {
    return this.http.put(
      `${this.baseUrl}users/${userId}/sheet/addFinalEvaluation/${sheetId}`,
      finalEvaluation
    );
  }
}
