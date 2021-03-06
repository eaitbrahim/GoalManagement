import { Observable, of, forkJoin } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';

import { HrService } from './../_services/hr.service';
import { UserService } from './../_services/user.service';
import { AdminService } from './../_services/admin.service';
import { AlertifyService } from './../_services/alertify.service';

@Injectable()
export class ReportSheetsResolver implements Resolve<any> {
  pageNumber = 1;
  pageSize = 10;
  constructor(
    private hrService: HrService,
    private adminService: AdminService,
    private userService: UserService,
    private router: Router,
    private alertify: AlertifyService
  ) { }

  resolve(route: ActivatedRouteSnapshot): Observable<any> {

    return forkJoin(
      [
        this.hrService.getReportSheets(this.pageNumber, this.pageSize).pipe(
          catchError(error => {
            this.alertify.error('Problème lors de la récupération des données des étas');
            this.router.navigate(['/']);
            return of(null);
          })),

          this.adminService.getPoles(),
          this.userService.getYears(),
          this.hrService.getReportGoals(this.pageNumber, this.pageSize).pipe(
            catchError(error => {
              this.alertify.error('Problème lors de la récupération des données des étas');
              this.router.navigate(['/']);
              return of(null);
            })),
          this.adminService.getUserStatus(),

      ]).pipe(map(result => {
        return {
          sheets: result[0],
          poleList: result[1],
          yearList: result[2],
          goals: result[3],
          userStatusList: result[4],
        };
      }));
  }
}
