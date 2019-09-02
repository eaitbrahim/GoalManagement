import { Observable, of, forkJoin } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';

import { User } from '../_models/user';
import { Department } from './../_models/department';
import { AdminService } from './../_services/admin.service';
import { AlertifyService } from './../_services/alertify.service';

@Injectable()
export class CollaboratorListResolver implements Resolve<any> {
  pageNumber = 1;
  pageSize = 10;
  users: User[];
  departments: Department[]

  constructor(
    private adminService: AdminService,
    private router: Router,
    private alertify: AlertifyService
  ) { }

  resolve(route: ActivatedRouteSnapshot): Observable<any> {
    return forkJoin(
      [
        this.adminService.getDepartments(),
        this.adminService.getUsers(this.pageNumber, this.pageSize)
          .pipe(catchError(error => {
            this.alertify.error('Problème lors de la récupération des données des utilisateurs');
            this.router.navigate(['/home']);
            return of(null);
          }))
      ]).pipe(map(result => {
        return {
          departments: result[0],
          users: result[1]
        };
      }));
  }
}
