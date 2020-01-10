import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { faUser, faKey, faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons';

import { AlertifyService } from './../_services/alertify.service';
import { UserService } from './../_services/user.service';
import { AuthService } from './../_services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  model: any = {};
  public loading = false;
  faUser = faUser;
  faKey = faKey;
  faEye = faEye;
  faEyeSlash = faEyeSlash;
  togglePassword: boolean;
  inputType: string = 'password';

  constructor(private authService: AuthService, private alertify: AlertifyService,
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
        console.log('Error when login: ', error);
        this.alertify.error('L\'adresse email ou le mot de passe est incorrect.');
      },
      () => {
        this.router.navigate(['']);
      }
    );
  }

  togglePasswordOnClick() {
    this.togglePassword = !this.togglePassword;
    if (this.togglePassword) {
      this.inputType = 'text';
    } else {
      this.inputType = 'password';
    }
  }

}
