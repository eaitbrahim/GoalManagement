import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';

import { UserService } from './../_services/user.service';
import { AuthService } from './../_services/auth.service';
import { AlertifyService } from './../_services/alertify.service';
import { Goal } from '../_models/goal';

@Injectable()
export class MyCollaboratorGoalsResolver implements Resolve<Goal> {
  constructor(
    private userService: UserService,
    private authService: AuthService,
    private router: Router,
    private alertify: AlertifyService
  ) { }

  resolve(route: ActivatedRouteSnapshot): Observable<Goal> {
    const axisInstanceIds = route.queryParams['axisInstanceIds'];

    return this.userService.getGoalsForAxis(this.authService.decodedToken.nameid, axisInstanceIds).pipe(
      catchError(error => {
        this.alertify.error(`Problème lors de la récupération des données des objectifs: ${error}`);
        this.router.navigate(['/sheets']);
        return of(null);
      })
    );
  }
}
