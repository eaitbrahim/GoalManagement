import { Observable, of, forkJoin } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';

import { User } from '../_models/user';
import { AdminService } from './../_services/admin.service';
import { AlertifyService } from './../_services/alertify.service';

@Injectable()
export class CollaboratorListResolver implements Resolve<any> {
  pageNumber = 1;
  pageSize = 10;
  users: User[];

  constructor(
    private adminService: AdminService,
    private router: Router,
    private alertify: AlertifyService
  ) { }

  resolve(route: ActivatedRouteSnapshot): Observable<any> {
    const departmentList = localStorage.getItem('departmentList');
    const userStatusList = localStorage.getItem('userStatusList');

    if (departmentList && userStatusList) {
      this.adminService.getUsersWithRoles(this.pageNumber, this.pageSize)
        .pipe(map(result => {
          return {
            departmentList: JSON.parse(departmentList),
            userStatusList: JSON.parse(userStatusList),
            users: result
          };
        }),
          catchError(error => {
            this.alertify.error(`Problème lors de la récupération des données des utilisateurs: ${error}`);
            this.router.navigate(['/home']);
            return of(null);
          }));
    }

    return forkJoin(
      [
        this.adminService.getDepartments(),
        this.adminService.getUserStatus(),
        this.adminService.getUsersWithRoles(this.pageNumber, this.pageSize)
          .pipe(catchError(error => {
            this.alertify.error('Problème lors de la récupération des données des utilisateurs');
            this.router.navigate(['/home']);
            return of(null);
          }))
      ]).pipe(map(result => {
        localStorage.setItem('departmentList', JSON.stringify(result[0]));
        localStorage.setItem('userStatusList', JSON.stringify(result[1]));
        return {
          departmentList: result[0],
          userStatusList: result[1],
          users: result[2]
        };
      }));
  }
}
