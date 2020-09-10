import { Observable, of, forkJoin } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';

import { Strategy } from '../_models/strategy';
import { HrService } from './../_services/hr.service';
import { AuthService } from './../_services/auth.service';
import { AlertifyService } from './../_services/alertify.service';

@Injectable()
export class StrategyListResolver implements Resolve<Strategy[]> {
  pageNumber = 1;
  pageSize = 10;

  constructor(
    private hrService: HrService,
    private authService: AuthService,
    private router: Router,
    private alertify: AlertifyService
  ) { }

  resolve(route: ActivatedRouteSnapshot): Observable<Strategy[]> {
    return this.hrService.getStrategies(this.authService.decodedToken.nameid, this.pageNumber, this.pageSize)
      .pipe(catchError(error => {
        this.alertify.error(`Problème lors de la récupération des données des strategies: ${error}`);
        this.router.navigate(['/home']);
        return of(null);
      }));
  }
}
