import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { faUser, faKey } from '@fortawesome/free-solid-svg-icons';

import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { UserService } from './../_services/user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  model: any = {};
  public loading = false;
  faUser = faUser;
  faKey = faKey;

  constructor(private authService: AuthService,
    private alertify: AlertifyService,
    private router: Router,
    private userService: UserService) { }

  ngOnInit() { }

  loggedIn() {
    return this.authService.loggedIn();
  }

  login() {
    this.loading = true;
    this.authService.login(this.model).subscribe(
      next => {
        this.loading = false;
        this.userService.totalUnreadMessages(this.authService.decodedToken.nameid);
        this.alertify.success('Connecté avec succès');
      },
      error => {
        this.loading = false;
        this.alertify.error(error);
      },
      () => {
        this.router.navigate(['']);
      }
    );
  }
}
