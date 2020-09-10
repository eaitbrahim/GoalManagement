import { Observable, of, forkJoin } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';


import { UserService } from './../_services/user.service';
import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from '../_services/auth.service';

@Injectable()
export class SheetDetailResolver implements Resolve<any> {
  constructor(
    private userService: UserService,
    private authService: AuthService,
    private router: Router,
    private alertify: AlertifyService
  ) { }

  resolve(route: ActivatedRouteSnapshot): Observable<any> {
    const goalTypeList = localStorage.getItem('goalTypeList');
    const userId = this.authService.decodedToken.nameid;

    if (goalTypeList) {
      this.userService.getMySheet(route.params['id'], userId)
        .pipe(map(result => {
          return {
            goalTypeList: JSON.parse(goalTypeList),
            sheetDetail: result
          };
        }),
          catchError(error => {
            this.alertify.error(`Problème lors de la récupération des données de votre fiche d\'evaluation: ${error}`);
            this.router.navigate(['/sheets']);
            return of(null);
          }));
    }

    return forkJoin(
      [
        this.userService.getGoalTypes(userId),
        this.userService.getProjects(userId),
        this.userService.getMySheet(route.params['id'], userId)
          .pipe(catchError(error => {
            this.alertify.error(`Problème lors de la récupération des données de votre fiche d\'evaluation: ${error}`);
            this.router.navigate(['/sheets']);
            return of(null);
          }))
      ]).pipe(map(result => {
        localStorage.setItem('goalTypeList', JSON.stringify(result[0]));
        return {
          goalTypeList: result[0],
          projectList: result[1],
          sheetDetail: result[2]
        };
      }));
  }
}
